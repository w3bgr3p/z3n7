using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace z3nIO.Tools
{




    public static class ZpToCsx
    {
        static readonly Regex RxMacroVar   = new Regex(@"\{-Variable\.(\w+)-\}", RegexOptions.Compiled);
        static readonly Regex RxMacroOther = new Regex(@"\{-[^}]+-\}",           RegexOptions.Compiled);
        static readonly Regex RxXmlDecl    = new Regex(@"<\?xml[^?]*\?>",        RegexOptions.Compiled);
        
		private static string ExtractXml(string zpPath)
        {
            foreach (var asm in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                if (asm.GetName().Name != "ProjectMaker") continue;
                try
                {
                    var loaderType = asm.GetType("ZennoLab.TemplateManipulator.V4.ProjectLoaderV4");
                    var archiveType = asm.GetType("ZennoLab.TemplateManipulator.V4.ProjectArchiveV4");
                    var archive = System.Activator.CreateInstance(archiveType, zpPath);
                    var loader = System.Activator.CreateInstance(loaderType);
                    string xml = (string)loaderType.GetMethod("LoadFromBytesArray").Invoke(loader, new object[] { archive });
                    return xml;
                }
                catch(System.Reflection.TargetInvocationException ex)
                {
                    return (ex.InnerException != null ? ex.InnerException.ToString() : ex.ToString());
                }
            }
            return null;
        }
        
        public static string GenerateCsx(string zpPath)
        {

            string raw = ExtractXml(zpPath);
            
            raw = RxXmlDecl.Replace(raw, "");
            var doc  = XDocument.Parse(raw);
            var root = doc.Root;

            var sb = new StringBuilder();
            EmitReferences(sb, root);
            EmitUsings(sb, root);
            EmitInitVariables(sb, root);
            EmitCommonCode(sb, root);
            EmitExecute(sb, root);
            return sb.ToString();
        }

        // ── Emit sections ─────────────────────────────────────────────────────

        static void EmitReferences(StringBuilder sb, XElement root)
        {
            var refs = root.Descendants("Reference")
                .Select(r => r.Attribute("Include")?.Value ?? "")
                .Where(v => v.StartsWith("[external]"));

            foreach (var r in refs)
                sb.AppendLine($"#r \"{r.Replace("[external]", "").Trim()}.dll\"");

            sb.AppendLine();
        }

        static void EmitUsings(StringBuilder sb, XElement root)
        {
            var text = root.Descendants("OwnCodeUsings").FirstOrDefault()
                ?.Attribute("Text")?.Value ?? "";

            if (!string.IsNullOrWhiteSpace(text))
                sb.AppendLine(Decode(text).TrimEnd());

            sb.AppendLine();
        }

        static void EmitInitVariables(StringBuilder sb, XElement root)
        {
            var vars = root.Descendants("Variables").FirstOrDefault()
                ?.Elements("Variable").ToList() ?? new List<XElement>();

            if (vars.Count == 0) return;

            sb.AppendLine("void InitVariables(IZennoPosterProjectModel project)");
            sb.AppendLine("{");
            foreach (var v in vars)
            {
                var name  = v.Attribute("Name")?.Value  ?? "";
                var value = v.Attribute("Value")?.Value ?? "";
                sb.AppendLine($"    project.Variables[\"{name}\"].Value = \"{Escape(value)}\";");
            }
            sb.AppendLine("}");
            sb.AppendLine();
        }

        static void EmitCommonCode(StringBuilder sb, XElement root) { }

        static void EmitExecute(StringBuilder sb, XElement root)
        {
            var steps = root.Descendants("Step")
                .Where(s => s.Attribute("ID") != null)
                .ToDictionary(s => s.Attribute("ID").Value, s => s);

            var entry = ParseTarget(
                root.Descendants("Start").FirstOrDefault()?.Attribute("nextAction")?.Value ?? "");
            

            // Определяем наличие GoodEnd / BadEnd
            var goodEndTarget = ParseTarget(
                root.Descendants("GoodEnd").FirstOrDefault()?.Attribute("nextAction")?.Value ?? "");
            var badEndTarget = ParseTarget(
                root.Descendants("BadEnd").FirstOrDefault()?.Attribute("nextAction")?.Value ?? "");

            bool hasGoodEnd = goodEndTarget.StepId != null || goodEndTarget.BranchId != null;
            bool hasBadEnd  = badEndTarget.StepId  != null || badEndTarget.BranchId  != null;

            var terminalStepIds = new HashSet<string>();
            if (goodEndTarget.StepId != null) terminalStepIds.Add(goodEndTarget.StepId);
            if (badEndTarget.StepId  != null) terminalStepIds.Add(badEndTarget.StepId);

            var ctx = new EmitContext(hasGoodEnd, hasBadEnd, terminalStepIds);

            sb.AppendLine("void Execute(IZennoPosterProjectModel project, Instance instance)");
            sb.AppendLine("{");

            if (entry.StepId != null)
                sb.AppendLine($"    goto {GotoTarget(entry)};");
            else
                sb.AppendLine("    return; // no entry point");

            sb.AppendLine();

            // Обход шагов в порядке достижимости
            var emitted = new HashSet<string>();
            var queue   = new Queue<string>();
            if (entry.StepId != null) queue.Enqueue(entry.StepId);

            while (queue.Count > 0)
            {
                var id = queue.Dequeue();
                if (!emitted.Add(id) || !steps.TryGetValue(id, out var step)) continue;
                EmitStep(sb, step, ctx);
                foreach (var nid in CollectNextStepIds(step))
                    if (!emitted.Contains(nid) && steps.ContainsKey(nid))
                        queue.Enqueue(nid);
            }

            // Orphan шаги
            foreach (var kv in steps)
                if (!emitted.Contains(kv.Key))
                    EmitStep(sb, kv.Value, ctx);

            // GoodEnd / BadEnd терминаторы
            if (hasGoodEnd)
            {
                sb.AppendLine("    __good_end:;");
                sb.AppendLine($"    goto {GotoTarget(goodEndTarget)};");
                sb.AppendLine();
            }

            if (hasBadEnd)
            {
                sb.AppendLine("    __bad_end:;");
                sb.AppendLine($"    goto {GotoTarget(badEndTarget)};");
                sb.AppendLine();
            }

            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("InitVariables(project);");
            sb.AppendLine("Execute(project, instance);");
        }

        // ── Step / Branch ─────────────────────────────────────────────────────

        static void EmitStep(StringBuilder sb, XElement step, EmitContext ctx)
        {
            var id       = step.Attribute("ID").Value;
            var userText = step.Attribute("UserText")?.Value ?? "";
            var header   = string.IsNullOrEmpty(userText) ? id : $"{id} — {userText}";

            sb.AppendLine($"    // ╔═ {header}");
            sb.AppendLine($"    {Label(id)}:;");

            var branches = step.Elements("Branch").ToList();
            
            var isTerminal = ctx.IsTerminal(id);
            var stepCtx    = isTerminal
                ? new EmitContext(false, false, ctx.TerminalStepIds)  // терминальный — без GoodEnd/BadEnd
                : ctx;
            
            for (int i = 0; i < branches.Count; i++)
            {
                var branchId = branches[i].Attribute("ID")?.Value;
                if (!string.IsNullOrEmpty(branchId))
                    sb.AppendLine($"    {BranchLabel(branchId)}:;");

                EmitBranch(sb, branches[i], isLast: i == branches.Count - 1, stepCtx);
            }

            if (branches.Count == 0)
                sb.AppendLine(ctx.HasGoodEnd ? "    goto __good_end;" : "    return;");

            sb.AppendLine();
        }

        static void EmitBranch(StringBuilder sb, XElement branch, bool isLast, EmitContext ctx)
        {
            var type     = branch.Attribute("Type")?.Value   ?? "";
            var action   = branch.Attribute("Action")?.Value ?? "";
            var userText = branch.Attribute("UserText")?.Value ?? "";
            var comment  = branch.Attribute("Comment")?.Value ?? "";

            if (!string.IsNullOrEmpty(userText)) sb.AppendLine($"        // {userText}");
            if (!string.IsNullOrEmpty(comment))  sb.AppendLine($"        // {comment}");

            if (type == "OwnCode" && action == "CSharp")
                EmitCSharpBranch(sb, branch);
            else
                EmitStubBranch(sb, branch, type, action);

            EmitFlow(sb, branch, isLast, ctx);
        }

        static void EmitCSharpBranch(StringBuilder sb, XElement branch)
        {
            var code = branch.Element("Parameters")?.Element("Code")?.Value ?? "";
            code = Decode(code);
            code = ReplaceMacros(code);
            if (string.IsNullOrWhiteSpace(code)) return;
            foreach (var line in code.Split('\n'))
                sb.AppendLine("        " + line.TrimEnd('\r'));
        }

        static void EmitStubBranch(StringBuilder sb, XElement branch, string type, string action)
        {
            var parameters = branch.Element("Parameters");

            if (type == "Logic" && action == "Alert")
            {
                var rawText = parameters?.Element("AlertText")?.Value ?? "";
                var color   = parameters?.Element("LogBackColor")?.Value ?? "";
                var logType = color == "Red"    ? "LogType.Error"
                            : color == "Yellow" ? "LogType.Warning"
                            :                     "LogType.Info";

                string textExpr;
                if (RxMacroVar.IsMatch(rawText) || RxMacroOther.IsMatch(rawText))
                {
                    var interp = RxMacroVar.Replace(rawText,
                        m => $"\" + project.Variables[\"{m.Groups[1].Value}\"].Value + \"");
                    interp = RxMacroOther.Replace(interp,
                        m => "\" + /* " + m.Value + " */ + \"");
                    textExpr = $"\"{interp}\"";
                    textExpr = textExpr.Replace("\"\" + ", "").Replace(" + \"\"", "");
                }
                else
                {
                    textExpr = $"\"{Escape(rawText)}\"";
                }

                sb.AppendLine($"        project.SendToLog({textExpr}, {logType}, true, LogColor.Default);");
                return;
            }

            if (type == "Logic" && (action == "If" || action == "Switch"))
                return;

            if (type == "VariableOperations" && action == "SetValue")
            {
                var value     = ReplaceMacros(parameters?.Element("Value")?.Value ?? "");
                var outputVar = ReplaceMacros(branch.Element("Results")?.Element("OutputVariable")?.Value ?? "");
                if (!string.IsNullOrEmpty(outputVar))
                    sb.AppendLine($"        {outputVar} = {value};");
                return;
            }

            sb.AppendLine($"        // [{type}:{action}]");
            if (parameters == null) return;
            foreach (var param in parameters.Elements())
            {
                var val = param.Value.Trim();
                if (string.IsNullOrEmpty(val)) continue;
                sb.AppendLine($"        //   {param.Name.LocalName}: {Cap(ReplaceMacros(val), 300)}");
            }
        }

        // ── Flow ──────────────────────────────────────────────────────────────

        static void EmitFlow(StringBuilder sb, XElement branch, bool isLast, EmitContext ctx)
        {
            var type   = branch.Attribute("Type")?.Value   ?? "";
            var action = branch.Attribute("Action")?.Value ?? "";
            var results = branch.Element("Results");

            var onSuccess = ParseTarget(results?.Element("OnSuccess")?.Value ?? "");
            var onError   = ParseTarget(results?.Element("OnError")?.Value   ?? "");

            if (type == "Logic" && action == "Switch")
            {
                var cases = results?.Elements()
                    .Where(e => e.Name.LocalName.StartsWith("Case") || e.Name.LocalName == "Default")
                    .ToList() ?? new List<XElement>();
                EmitSwitchFlow(sb, branch, cases, ctx);
                return;
            }

            if (type == "Logic" && action == "If")
            {
                var expr        = ReplaceMacros(branch.Element("Parameters")?.Element("Expression")?.Value ?? "");
                var trueTarget  = GotoTarget(onSuccess);
                var falseTarget = GotoTarget(onError);

                // false ветка
                if (falseTarget != null)
                    sb.AppendLine($"        if (!({expr})) goto {falseTarget};");
                else if (ctx.HasBadEnd)
                    sb.AppendLine($"        if (!({expr})) goto __bad_end;");

                // true ветка (только если isLast — иначе fall-through на следующую Branch)
                if (isLast)
                {
                    if (trueTarget != null)
                        sb.AppendLine($"        goto {trueTarget};");
                    else if (ctx.HasGoodEnd)
                        sb.AppendLine("        goto __good_end;");
                    else
                        sb.AppendLine("        return;");
                }
                return;
            }

            // Не последняя ветка в шаге — fall-through, goto не нужен
            if (!isLast) return;

            var sg = GotoTarget(onSuccess);
            var eg = GotoTarget(onError);

            if (sg != null)
            {
                if (eg != null && type != "OwnCode")
                    sb.AppendLine($"        // OnError → {eg}");
                sb.AppendLine($"        goto {sg};");
            }
            else if (ctx.HasGoodEnd)
            {
                if (eg != null && type != "OwnCode")
                    sb.AppendLine($"        // OnError → {eg}");
                sb.AppendLine("        goto __good_end;");
            }
            else
            {
                if (eg != null && type != "OwnCode")
                    sb.AppendLine($"        // OnError → {eg}");
                sb.AppendLine("        return;");
            }
        }

        static void EmitSwitchFlow(StringBuilder sb, XElement branch, List<XElement> cases, EmitContext ctx)
        {
            var switchVarRaw = branch.Element("Parameters")?.Element("Variable")?.Value ?? "";
            var switchVar    = string.IsNullOrEmpty(switchVarRaw)
                ? "/* switch variable */"
                : ReplaceMacros(switchVarRaw);

            sb.AppendLine($"        switch ({switchVar})");
            sb.AppendLine("        {");

            foreach (var c in cases)
            {
                var isDefault = c.Name.LocalName == "Default";

                string key    = null;
                string rawVal = null;
                var encoded   = c.Value ?? "";
                if (!string.IsNullOrEmpty(encoded))
                {
                    try
                    {
                        var pair = XElement.Parse(encoded);
                        key    = pair.Element("Key")?.Value;
                        rawVal = pair.Element("Value")?.Value;
                    }
                    catch { rawVal = encoded; }
                }

                var target = ParseTarget(rawVal ?? "");
                var g      = GotoTarget(target);
                var fallback = ctx.HasBadEnd ? "goto __bad_end" : "return";

                if (isDefault)
                    sb.AppendLine(g != null ? $"            default: goto {g};" : $"            default: {fallback};");
                else
                {
                    var caseKey = string.IsNullOrEmpty(key) ? c.Name.LocalName : key;
                    sb.AppendLine(g != null
                        ? $"            case \"{caseKey}\": goto {g};"
                        : $"            case \"{caseKey}\": {fallback};");
                }
            }

            sb.AppendLine("        }");
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        static IEnumerable<string> CollectNextStepIds(XElement step)
        {
            foreach (var branch in step.Elements("Branch"))
            {
                var results = branch.Element("Results");
                if (results == null) continue;

                foreach (var el in results.Elements())
                {
                    var localName = el.Name.LocalName;

                    if (localName.StartsWith("Case") || localName == "Default")
                    {
                        var encoded = el.Value ?? "";
                        if (string.IsNullOrEmpty(encoded)) continue;
                        string stepId = null;
                        try
                        {
                            var pair = XElement.Parse(encoded);
                            stepId = ParseTarget(pair.Element("Value")?.Value ?? "").StepId;
                        }
                        catch { }
                        if (stepId != null) yield return stepId;
                        continue;
                    }

                    var target = ParseTarget(el.Value ?? "");
                    if (target.StepId != null) yield return target.StepId;
                }
            }
        }

        struct Target
        {
            public string StepId;
            public string BranchId;
            public Target(string stepId, string branchId) { StepId = stepId; BranchId = branchId; }
        }

        sealed class EmitContext
        {
            public bool HasGoodEnd { get; }
            public bool HasBadEnd  { get; }
            public HashSet<string> TerminalStepIds { get; }

            public EmitContext(bool hasGoodEnd, bool hasBadEnd, HashSet<string> terminalStepIds)
            {
                HasGoodEnd      = hasGoodEnd;
                HasBadEnd       = hasBadEnd;
                TerminalStepIds = terminalStepIds;
            }

            public bool IsTerminal(string stepId) => TerminalStepIds.Contains(stepId);
        }

        static Target ParseTarget(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return new Target(null, null);
            var parts    = raw.Trim().Split('|');
            var stepId   = Guid.TryParse(parts[0], out _) ? parts[0] : null;
            var branchId = parts.Length > 1 && Guid.TryParse(parts[1], out _) ? parts[1] : null;
            return new Target(stepId, branchId);
        }

        static string Label(string id)       => "node_"   + id.Replace("-", "_");
        static string BranchLabel(string id) => "branch_" + id.Replace("-", "_");

        static string GotoTarget(Target t)
        {
            if (t.BranchId != null) return BranchLabel(t.BranchId);
            if (t.StepId   != null) return Label(t.StepId);
            return null;
        }

        static string ReplaceMacros(string s)
        {
            s = RxMacroVar.Replace(s,   m => $"project.Variables[\"{m.Groups[1].Value}\"].Value");
            s = RxMacroOther.Replace(s, m => $"/* {m.Value} */");
            return s;
        }

        static string Decode(string s) =>
            s.Replace("&#xD;&#xA;", "\n").Replace("&#xD;", "\r").Replace("&#xA;", "\n")
             .Replace("&amp;", "&").Replace("&lt;", "<").Replace("&gt;", ">").Replace("&quot;", "\"");

        static string Escape(string s) =>
            s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "").Replace("\n", "\\n");

        static string Cap(string s, int max) => s.Length <= max ? s : s.Substring(0, max) + "…";
    }
}