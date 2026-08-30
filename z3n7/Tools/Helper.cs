using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.IO;
using ZennoLab.InterfacesLibrary.ProjectModel;
using System;


namespace z3n7
{
    public static class Helper
    {
        private static readonly System.Drawing.Font defaultFont = new System.Drawing.Font("Cascadia Mono", 9F);
        private static string ShowInputDialog(string prompt = "Enter text:", string title = "Input", string defaultValue = "")
        {
            string result = null;
            
            // Создаем форму
            var form = new System.Windows.Forms.Form();
            form.Text = title;
            form.Size = new System.Drawing.Size(350, 150);
            form.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            form.MaximizeBox = false;
            form.MinimizeBox = false;
            form.TopMost = true;
            
            // Создаем элементы управления
            var label = new System.Windows.Forms.Label();
            label.Text = prompt;
            label.Font = defaultFont;
            label.Location = new System.Drawing.Point(12, 15);
            label.Size = new System.Drawing.Size(310, 20);
            
            var textBox = new System.Windows.Forms.TextBox();
            textBox.Text = defaultValue;
            textBox.Location = new System.Drawing.Point(12, 40);
            textBox.Size = new System.Drawing.Size(310, 23);
            textBox.Font = defaultFont;
            
            
            var buttonOK = new System.Windows.Forms.Button();
            buttonOK.Text = "OK";
            buttonOK.Location = new System.Drawing.Point(167, 75);
            buttonOK.Size = new System.Drawing.Size(75, 25);
            buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            
            var buttonCancel = new System.Windows.Forms.Button();
            buttonCancel.Text = "Cancel";
            buttonCancel.Location = new System.Drawing.Point(247, 75);
            buttonCancel.Size = new System.Drawing.Size(75, 25);
            buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            
            // Настраиваем кнопки формы
            form.AcceptButton = buttonOK;
            form.CancelButton = buttonCancel;
            
            // Обработчики событий
            buttonOK.Click += (s, e) => 
            {
                result = textBox.Text;
                form.DialogResult = System.Windows.Forms.DialogResult.OK;
                form.Close();
            };
            
            buttonCancel.Click += (s, e) => 
            {
                result = null;
                form.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                form.Close();
            };
            
            // Обработка Enter и Escape
            textBox.KeyDown += (s, e) =>
            {
                if (e.KeyCode == System.Windows.Forms.Keys.Enter)
                {
                    result = textBox.Text;
                    form.DialogResult = System.Windows.Forms.DialogResult.OK;
                    form.Close();
                }
                else if (e.KeyCode == System.Windows.Forms.Keys.Escape)
                {
                    result = null;
                    form.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                    form.Close();
                }
            };
            
            // Добавляем элементы на форму
            form.Controls.Add(label);
            form.Controls.Add(textBox);
            form.Controls.Add(buttonOK);
            form.Controls.Add(buttonCancel);
            
            // Устанавливаем фокус на текстовое поле и выделяем весь текст
            form.Shown += (s, e) => 
            {
                textBox.Focus();
                textBox.SelectAll();
            };
            
            // Показываем диалог
            var dialogResult = form.ShowDialog();
            
            // Возвращаем результат
            return dialogResult == System.Windows.Forms.DialogResult.OK ? result : null;
        }
        private static void ShowForm(string textToShow)
        {
            // Создаем новую форму
            var form = new System.Windows.Forms.Form();
            form.TopMost = true;
            // Создаем TextBox для отображения текста (доступен для выделения и копирования)
            var textBox = new System.Windows.Forms.TextBox();
            
            // Настраиваем TextBox
            textBox.Text = textToShow;
            textBox.Multiline = true;
            textBox.ReadOnly = true;
            textBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            textBox.WordWrap = true;
            textBox.Font = defaultFont;//new System.Drawing.Font("Cascadia Mono", 9F);
            textBox.BackColor = System.Drawing.SystemColors.Window;
            //form.Font = new System.Drawing.Font("Cascadia Mono SemiBold", 10);
            // Вычисляем оптимальный размер на основе текста
            using (var g = form.CreateGraphics())
            {
                var textSize = g.MeasureString(textToShow, textBox.Font);
                
                // Определяем количество строк для более точного расчета высоты
                int lineCount = textToShow.Split('\n').Length;
                int lineHeight = (int)textBox.Font.GetHeight(g);
                
                // Рассчитываем размер с учетом отступов и скроллбаров
                int calculatedWidth = Math.Min((int)textSize.Width + 60, 800); // максимум 800px
                int calculatedHeight = Math.Min(lineCount * lineHeight + 80, 600); // максимум 600px
                
                // Устанавливаем минимальные размеры
                int formWidth = Math.Max(calculatedWidth, 300);
                int formHeight = Math.Max(calculatedHeight, 200);
                
                form.Size = new System.Drawing.Size(formWidth, formHeight);
            }
            
            // Настраиваем форму
            form.Text = "Text Viewer";
            form.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            form.MinimumSize = new System.Drawing.Size(300, 200);
            form.MaximizeBox = true;
            form.MinimizeBox = true;
            
            // Размещаем TextBox на всю область формы с отступами
            textBox.Dock = System.Windows.Forms.DockStyle.Fill;
            textBox.Margin = new System.Windows.Forms.Padding(10);
            
            // Создаем панель для отступов
            var panel = new System.Windows.Forms.Panel();
            panel.Dock = System.Windows.Forms.DockStyle.Fill;
            panel.Padding = new System.Windows.Forms.Padding(10);
            panel.Controls.Add(textBox);
            
            // Добавляем панель на форму
            form.Controls.Add(panel);
            
            // Добавляем контекстное меню для удобства копирования
            var contextMenu = new System.Windows.Forms.ContextMenuStrip();
            var selectAllItem = new System.Windows.Forms.ToolStripMenuItem("Select All");
            var copyItem = new System.Windows.Forms.ToolStripMenuItem("Copy");
            
            selectAllItem.Click += (s, e) => textBox.SelectAll();
            copyItem.Click += (s, e) => 
            {
                if (textBox.SelectedText.Length > 0)
                    System.Windows.Forms.Clipboard.SetText(textBox.SelectedText);
                else
                    System.Windows.Forms.Clipboard.SetText(textBox.Text);
            };
            
            contextMenu.Items.Add(selectAllItem);
            contextMenu.Items.Add(copyItem);
            textBox.ContextMenuStrip = contextMenu;
            
            // Добавляем горячие клавиши
            form.KeyPreview = true;
            form.KeyDown += (s, e) =>
            {
                if (e.Control && e.KeyCode == System.Windows.Forms.Keys.A)
                {
                    textBox.SelectAll();
                    e.Handled = true;
                }
                else if (e.Control && e.KeyCode == System.Windows.Forms.Keys.C)
                {
                    if (textBox.SelectedText.Length > 0)
                        System.Windows.Forms.Clipboard.SetText(textBox.SelectedText);
                    else
                        System.Windows.Forms.Clipboard.SetText(textBox.Text);
                    e.Handled = true;
                }
                else if (e.KeyCode == System.Windows.Forms.Keys.Escape)
                {
                    form.Close();
                }
            };
            
            // Показываем форму
            form.ShowDialog();
        }

        #region Help / API index

        private class MemberDoc
        {
            public string Id, Type, Name, Signature, Summary, Returns, Remarks, Source;
            public List<string> Params = new List<string>();
        }

        private static List<MemberDoc> _apiIndex;

        private static string DocText(XElement e)
        {
            if (e == null) return null;
            foreach (var see in e.Descendants("see").ToList())
                see.ReplaceWith(new XText((string)see.Attribute("cref") ?? ""));
            var s = System.Text.RegularExpressions.Regex.Replace(e.Value, @"\s+", " ").Trim();
            return s.Length == 0 ? null : s;
        }

        private static string ShortType(Type t)
        {
            return t == null ? "void" : t.Name;
        }

        private static string Signature(Type t, MemberInfo mi)
        {
            var ctor = mi as ConstructorInfo;
            if (ctor != null)
                return t.Name + "(" + string.Join(", ", ctor.GetParameters()
                    .Select(p => ShortType(p.ParameterType) + " " + p.Name)) + ")";

            var m = mi as MethodInfo;
            if (m != null)
                return ShortType(m.ReturnType) + " " + t.Name + "." + m.Name + "("
                    + string.Join(", ", m.GetParameters()
                        .Select(p => ShortType(p.ParameterType) + " " + p.Name)) + ")";

            var p2 = mi as PropertyInfo;
            if (p2 != null)
                return ShortType(p2.PropertyType) + " " + t.Name + "." + p2.Name
                    + " { " + (p2.CanRead ? "get; " : "") + (p2.CanWrite ? "set; " : "") + "}";

            var f = mi as FieldInfo;
            if (f != null)
                return t.IsEnum
                    ? "enum " + t.Name + "." + f.Name
                    : ShortType(f.FieldType) + " " + t.Name + "." + f.Name;

            var ev = mi as EventInfo;
            if (ev != null) return "event " + ShortType(ev.EventHandlerType) + " " + t.Name + "." + ev.Name;

            return t.Name + "." + mi.Name;
        }

        private static List<MemberDoc> BuildApiIndex()
        {
            var list = new List<MemberDoc>();
            var byKey = new Dictionary<string, MemberDoc>(StringComparer.OrdinalIgnoreCase);

            var exe = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            var dir = Path.GetDirectoryName(exe);

            // 1) XML-документация: всё, что лежит рядом с exe, а не захардкоженный список
            foreach (var path in Directory.GetFiles(dir, "*.xml"))
            {
                XDocument x;
                try { x = XDocument.Load(path); }
                catch { continue; }
                if (x.Root == null || x.Root.Name != "doc") continue;

                foreach (var m in x.Descendants("member"))
                {
                    var id = (string)m.Attribute("name");
                    if (string.IsNullOrEmpty(id) || id.Length < 3) continue;

                    var noArgs = id.Substring(2).Split('(')[0];
                    var dot = noArgs.LastIndexOf('.');
                    var name = dot < 0 ? noArgs : noArgs.Substring(dot + 1);
                    var typeFull = dot < 0 ? noArgs : noArgs.Substring(0, dot);

                    var d = new MemberDoc
                    {
                        Id = id,
                        Name = name,
                        Source = "xml",
                        Type = id[0] == 'T' ? name : typeFull.Substring(typeFull.LastIndexOf('.') + 1),
                        Summary = DocText(m.Element("summary")),
                        Returns = DocText(m.Element("returns")),
                        Remarks = DocText(m.Element("remarks")),
                    };
                    foreach (var p in m.Elements("param"))
                        d.Params.Add((string)p.Attribute("name") + ": " + DocText(p));

                    list.Add(d);
                    var key = d.Type + "." + d.Name;
                    if (!byKey.ContainsKey(key)) byKey[key] = d;
                }
            }

            // 2) рефлексия: реальный API, включая недокументированные члены
            const BindingFlags bf = BindingFlags.Public | BindingFlags.Instance
                                  | BindingFlags.Static | BindingFlags.DeclaredOnly;

            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!asm.GetName().Name.StartsWith("ZennoLab", StringComparison.OrdinalIgnoreCase)) continue;

                Type[] types;
                try { types = asm.GetExportedTypes(); }
                catch { continue; }

                foreach (var t in types)
                {
                    MemberInfo[] members;
                    try { members = t.GetMembers(bf); }
                    catch { continue; }

                    foreach (var mi in members)
                    {
                        var mt = mi as MethodInfo;
                        if (mt != null && mt.IsSpecialName) continue;
                        if (mi.Name == "value__") continue;

                        var key = t.Name + "." + mi.Name;
                        MemberDoc found;
                        if (byKey.TryGetValue(key, out found))
                        {
                            if (found.Signature == null) found.Signature = Signature(t, mi);
                            continue;
                        }

                        var d = new MemberDoc
                        {
                            Id = t.FullName + "." + mi.Name,
                            Type = t.Name,
                            Name = mi.Name,
                            Signature = Signature(t, mi),
                            Source = "reflection",
                        };
                        byKey[key] = d;
                        list.Add(d);
                    }
                }
            }

            return list;
        }

        public static void Help(this IZennoPosterProjectModel project, string toSearch = null)
        {
            if (_apiIndex == null) _apiIndex = BuildApiIndex();
            ShowApiBrowser(_apiIndex, toSearch ?? "");
        }

        private static int Score(MemberDoc d, string[] terms)
        {
            var best = 0;
            foreach (var term in terms)
            {
                int s;
                if (string.Equals(d.Name, term, StringComparison.OrdinalIgnoreCase)) s = 100;
                else if (d.Name != null && d.Name.StartsWith(term, StringComparison.OrdinalIgnoreCase)) s = 80;
                else if (d.Name != null && d.Name.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0) s = 60;
                else if (d.Type != null && d.Type.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0) s = 40;
                else if (d.Summary != null && d.Summary.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0) s = 20;
                else return -1;   // все термины обязательны
                if (s > best) best = s;
            }
            return best + (d.Summary != null ? 5 : 0);
        }

        private static string Details(MemberDoc d)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine(d.Signature ?? d.Id);
            sb.AppendLine();
            sb.AppendLine("id     : " + d.Id);
            sb.AppendLine("source : " + d.Source);
            sb.AppendLine();
            if (d.Summary != null) { sb.AppendLine("SUMMARY"); sb.AppendLine("  " + d.Summary); sb.AppendLine(); }
            if (d.Params.Count > 0)
            {
                sb.AppendLine("PARAMS");
                foreach (var p in d.Params) sb.AppendLine("  " + p);
                sb.AppendLine();
            }
            if (d.Returns != null) { sb.AppendLine("RETURNS"); sb.AppendLine("  " + d.Returns); sb.AppendLine(); }
            if (d.Remarks != null) { sb.AppendLine("REMARKS"); sb.AppendLine("  " + d.Remarks); sb.AppendLine(); }
            if (d.Summary == null && d.Params.Count == 0 && d.Returns == null && d.Remarks == null)
                sb.AppendLine("(нет XML-документации — сигнатура получена рефлексией)");
            return sb.ToString().Replace("\n", "\r\n");
        }


        private static void ShowApiBrowser(List<MemberDoc> index, string initialQuery)
        {
            var form = new System.Windows.Forms.Form();
            form.Text = "Zenno API browser";
            form.Size = new System.Drawing.Size(1150, 720);
            form.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            form.KeyPreview = true;

            var root = new System.Windows.Forms.TableLayoutPanel();
            root.Dock = System.Windows.Forms.DockStyle.Fill;
            root.ColumnCount = 1;
            root.RowCount = 3;
            root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));

            var top = new System.Windows.Forms.Panel();
            top.Dock = System.Windows.Forms.DockStyle.Fill;

            var search = new System.Windows.Forms.TextBox();
            search.Font = defaultFont;
            search.Dock = System.Windows.Forms.DockStyle.Fill;
            search.Text = initialQuery;

            var onlyDoc = new System.Windows.Forms.CheckBox();
            onlyDoc.Text = "только с документацией";
            onlyDoc.Font = defaultFont;
            onlyDoc.Dock = System.Windows.Forms.DockStyle.Right;
            onlyDoc.Width = 190;
            onlyDoc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            top.Controls.Add(search);
            top.Controls.Add(onlyDoc);

            var split = new System.Windows.Forms.SplitContainer();
            // размер задаётся ДО MinSize/SplitterDistance: иначе валидация падает
            // на дефолтной ширине 150 (SplitterDistance=50 < Panel1MinSize)
            split.Size = new System.Drawing.Size(1100, 620);
            split.Panel1MinSize = 200;
            split.Panel2MinSize = 200;
            split.SplitterDistance = 430;
            split.Dock = System.Windows.Forms.DockStyle.Fill;

            var tree = new System.Windows.Forms.TreeView();
            tree.Dock = System.Windows.Forms.DockStyle.Fill;
            tree.Font = defaultFont;
            tree.HideSelection = false;

            var details = new System.Windows.Forms.TextBox();
            details.Dock = System.Windows.Forms.DockStyle.Fill;
            details.Font = defaultFont;
            details.Multiline = true;
            details.ReadOnly = true;
            details.WordWrap = true;
            details.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            details.BackColor = System.Drawing.SystemColors.Window;

            split.Panel1.Controls.Add(tree);
            split.Panel2.Controls.Add(details);

            var status = new System.Windows.Forms.Label();
            status.Dock = System.Windows.Forms.DockStyle.Fill;
            status.Font = defaultFont;
            status.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            root.Controls.Add(top, 0, 0);
            root.Controls.Add(split, 0, 1);
            root.Controls.Add(status, 0, 2);
            form.Controls.Add(root);

            System.Windows.Forms.MethodInvoker rebuild = delegate
            {
                var q = search.Text.Trim();
                tree.BeginUpdate();
                tree.Nodes.Clear();

                if (q.Length < 2)
                {
                    status.Text = "введите минимум 2 символа";
                    tree.EndUpdate();
                    return;
                }

                var terms = q.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var scored = new List<KeyValuePair<int, MemberDoc>>();
                foreach (var d in index)
                {
                    if (onlyDoc.Checked && d.Summary == null) continue;
                    var s = Score(d, terms);
                    if (s >= 0) scored.Add(new KeyValuePair<int, MemberDoc>(s, d));
                }

                var groups = scored
                    .GroupBy(kv => kv.Value.Type ?? "?")
                    .Select(g => new
                    {
                        Type = g.Key,
                        Best = g.Max(kv => kv.Key),
                        Items = g.OrderByDescending(kv => kv.Key)
                                 .ThenBy(kv => kv.Value.Name, StringComparer.OrdinalIgnoreCase)
                                 .Select(kv => kv.Value).ToList()
                    })
                    .OrderByDescending(g => g.Best)
                    .ThenBy(g => g.Type, StringComparer.OrdinalIgnoreCase)
                    .ToList();

                foreach (var g in groups)
                {
                    var parent = new System.Windows.Forms.TreeNode(g.Type + "  (" + g.Items.Count + ")");
                    foreach (var d in g.Items)
                    {
                        var label = d.Name + (d.Summary != null ? "" : "  ·");
                        var child = new System.Windows.Forms.TreeNode(label);
                        child.Tag = d;
                        child.ToolTipText = d.Signature ?? d.Id;
                        parent.Nodes.Add(child);
                    }
                    tree.Nodes.Add(parent);
                }

                if (groups.Count > 0 && groups.Count <= 5) tree.Nodes[0].ExpandAll();
                tree.EndUpdate();

                status.Text = scored.Count + " член(ов) в " + groups.Count + " типах   ·   «·» = без XML-документации";
                if (scored.Count == 0) details.Text = "ничего не найдено по [" + q + "]";
            };

            search.TextChanged += delegate { rebuild(); };
            onlyDoc.CheckedChanged += delegate { rebuild(); };

            tree.AfterSelect += delegate (object s, System.Windows.Forms.TreeViewEventArgs e)
            {
                var d = e.Node.Tag as MemberDoc;
                details.Text = d == null ? "" : Details(d);
            };

            tree.NodeMouseDoubleClick += delegate (object s, System.Windows.Forms.TreeNodeMouseClickEventArgs e)
            {
                var d = e.Node.Tag as MemberDoc;
                if (d != null) System.Windows.Forms.Clipboard.SetText(d.Signature ?? d.Id);
            };

            form.KeyDown += delegate (object s, System.Windows.Forms.KeyEventArgs e)
            {
                if (e.KeyCode == System.Windows.Forms.Keys.Escape) form.Close();
                if (e.Control && e.KeyCode == System.Windows.Forms.Keys.F) search.Focus();
            };

            form.Shown += delegate
            {
                try
                {
                    var want = split.Width / 2;
                    if (want >= split.Panel1MinSize && want <= split.Width - split.Panel2MinSize)
                        split.SplitterDistance = want;
                }
                catch { }
                search.Focus();
                search.SelectAll();
                rebuild();
            };
            form.ShowDialog();
        }

        #endregion

    }
}