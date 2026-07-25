using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace z3n7.Tools
{
    
    public static class ZpToCsx
    {
        static readonly Regex RxMacroVar   = new Regex(@"\{-Variable\.(\w+)-\}", RegexOptions.Compiled);
        static readonly Regex RxMacroOther = new Regex(@"\{-[^}]+-\}",           RegexOptions.Compiled);
        static readonly Regex RxXmlDecl    = new Regex(@"<\?xml[^?]*\?>",        RegexOptions.Compiled);
        static readonly string _template =  "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";

        
        public static string ExtractXml(string zpPath)
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
        
        public static void XmlToZp(string xml, string zpPath)
        {
	        foreach (var asm in System.AppDomain.CurrentDomain.GetAssemblies())
	        {
	            if (asm.GetName().Name != "ProjectMaker") continue;
	            
	            var loaderType  = asm.GetType("ZennoLab.TemplateManipulator.V4.ProjectLoaderV4");
	            var archiveType = asm.GetType("ZennoLab.TemplateManipulator.V4.ProjectArchiveV4");
	            
		        string tmpPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".zp");
				        
		        byte[] zpBytes = Convert.FromBase64String(_template);
		        File.WriteAllBytes(tmpPath, zpBytes);
		        var archive = Activator.CreateInstance(archiveType, tmpPath);
	            var loader  = Activator.CreateInstance(loaderType);
                
		        byte[] bytes = (byte[])loaderType.GetMethod("ToByteArray")
	                               .Invoke(loader, new object[] { xml });
	        
		        archiveType.GetMethod("RemoveEntries")
		            .Invoke(archive, new object[] {(Func<string, bool>)(name => name.EndsWith(".xml"))});
	        
		        archiveType.GetMethod("SaveProject")
		            .Invoke(archive, new object[] { "Template.xml", bytes });
		        
		        archiveType.GetMethod("SaveToFile")
		            .Invoke(archive, new object[] { zpPath });
	            
		        File.Delete(tmpPath);
	            break;
	        }
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

        public static string Template()
        {
            return _template;
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