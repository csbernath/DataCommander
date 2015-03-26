namespace DataCommander.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Forms;
    using DataCommander.Foundation.Diagnostics;

    public sealed class QueryTextBox : System.Windows.Forms.UserControl
    {
        private List<KeyWordList> keyWordLists = new List<KeyWordList>();
        private int selectionStart = 0;
        private int selectionLength = 0;
        private int prevSelectionStart;
        private int prevSelectionLength;
        private bool changeEventEnabled = true;
        private ToolStripStatusLabel sbPanel;
        private IKeyboardHandler keyboardHandler;
        private int tabSize = 4;
        private bool insertSpaces = true;
        private int columnIndex;

        private RichTextBox richTextBox;

        public RichTextBox RichTextBox
        {
            get
            {
                return richTextBox;
            }
        }

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = new System.ComponentModel.Container();

        public QueryTextBox()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitForm call
            richTextBox.SelectionChanged += new EventHandler(richTextBox_SelectionChanged);
            richTextBox.DragEnter += new DragEventHandler(richTextBox_DragEnter);
            richTextBox.DragDrop += new DragEventHandler(richTextBox_DragDrop);
        }

        public void AddKeyWords(string[] keyWords, Color color)
        {
            if (keyWords != null)
            {
                KeyWordList keyWordList = new KeyWordList();
                keyWordList.KeyWords = new string[keyWords.Length];

                for (int i = 0; i < keyWords.Length; i++)
                {
                    keyWordList.KeyWords[i] = keyWords[i].ToUpper();
                }

                keyWordList.Color = color;

                keyWordLists.Add(keyWordList);
            }
        }

        public ToolStripStatusLabel CaretPositionPanel
        {
            set
            {
                sbPanel = value;
            }
        }

        public override string Text
        {
            get
            {
                return this.richTextBox.Text;
            }

            set
            {
                changeEventEnabled = false;
                richTextBox.Text = value;

                if (value != null)
                {
                    string text = richTextBox.Text;
                    Colorize(text, 0, text.Length - 1);
                }

                changeEventEnabled = true;
            }
        }

        public string SelectedText
        {
            get
            {
                return this.richTextBox.SelectedText;
            }
        }

        public void Paste()
        {
            var format = DataFormats.GetFormat(DataFormats.UnicodeText);
            richTextBox.Paste(format);
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.richTextBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // richTextBox
            // 
            this.richTextBox.AcceptsTab = true;
            this.richTextBox.AllowDrop = true;
            this.richTextBox.AutoWordSelection = true;
            this.richTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox.Location = new System.Drawing.Point(0, 0);
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.Size = new System.Drawing.Size(408, 150);
            this.richTextBox.TabIndex = 0;
            this.richTextBox.Text = "";
            this.richTextBox.WordWrap = false;
            this.richTextBox.TextChanged += new System.EventHandler(this.richTextBox_TextChanged);
            this.richTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.richTextBox_KeyDown);
            this.richTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.richTextBox_KeyPress);
            this.richTextBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.richTextBox_MouseUp);
            // 
            // QueryTextBox
            // 
            this.Controls.Add(this.richTextBox);
            this.Name = "QueryTextBox";
            this.Size = new System.Drawing.Size(408, 150);
            this.ResumeLayout(false);

        }

        #endregion

        private void SetColor(int startWord, int length, Color color)
        {
            MethodProfiler.BeginMethod();

            try
            {
                richTextBox.Select(startWord, length);
                richTextBox.SelectionColor = color;
            }
            finally
            {
                MethodProfiler.EndMethod();
            }
        }

        public static int GetLineIndex(
            RichTextBox richTextBox,
            int i)
        {
            return NativeMethods.SendMessage(richTextBox.Handle.ToInt32(), (int)NativeMethods.Message.EditBox.LineIndex, i, 0);
        }

        private int LineIndex(int i)
        {
            return GetLineIndex(richTextBox, i);
        }

        private void richTextBox_SelectionChanged(object sender, EventArgs e)
        {
            MethodProfiler.BeginMethod();

            try
            {
                prevSelectionStart = selectionStart;
                selectionStart = richTextBox.SelectionStart;
                prevSelectionLength = selectionLength;
                selectionLength = richTextBox.SelectionLength;

                int charIndex = selectionStart;
                int line = richTextBox.GetLineFromCharIndex(charIndex) + 1;
                int lineIndex = LineIndex(-1);
                int col = charIndex - lineIndex + 1;
                sbPanel.Text = "Ln " + line + " Col " + col;
                this.columnIndex = col;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                MethodProfiler.EndMethod();
            }
        }

        public int WordStart(string text, int index)
        {
            int i = index;
            bool wordFound = false;

            while (i >= 0)
            {
                char c = text[i];

                if (wordFound && IsSeparator(c))
                {
                    break;
                }
                else
                {
                    wordFound = true;
                }

                i--;
            }

            i++;

            return i;
        }

        public static string PrevWord(string text, ref int index)
        {
            int wordEnd = -1;
            string word = null;

            while (index >= 0)
            {
                char c = text[index];
                bool isSeparator = IsSeparator(c) || index == 0;

                if (wordEnd != -1 && isSeparator)
                {
                    if (index > 0)
                    {
                        index++;
                    }

                    word = text.Substring(index, wordEnd - index + 1);
                    break;
                }
                else if (wordEnd == -1 && !isSeparator)
                {
                    wordEnd = index;
                }

                index--;
            }

            return word;
        }

        public static int WordEnd(string text, int index)
        {
            int length = text.Length;
            int i = index;

            while (i < length)
            {
                char c = text[i];

                if (IsSeparator(c))
                {
                    break;
                }

                i++;
            }

            i--;

            return i;
        }

        public static int NextWordStart(string text, int index)
        {
            int length = text.Length;
            int i = index;

            while (i < length)
            {
                char c = text[i];

                if (!IsSeparator(c))
                {
                    break;
                }

                i++;
            }

            return i;
        }

        private void Colorize(string text, int startIndex, int endIndex)
        {
            MethodProfiler.BeginMethod();

            try
            {
                long maxTicks = Stopwatch.Frequency*5; // max. 5 seconds
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                int orgSelectionStart = Math.Max(richTextBox.SelectionStart, 0);
                int orgSelectionLength = Math.Max(richTextBox.SelectionLength, 0);

                IntPtr intPtr = richTextBox.Handle;
                int hWnd = intPtr.ToInt32();
                NativeMethods.SendMessage(hWnd, (int)NativeMethods.Message.Gdi.SetRedraw, 0, 0);

                MethodProfiler.BeginMethodFraction("ControlText");

                try
                {
                    richTextBox.SelectionStart = startIndex;
                    richTextBox.SelectionLength = endIndex - startIndex + 1;
                    richTextBox.SelectionColor = SystemColors.ControlText;
                }
                finally
                {
                    MethodProfiler.EndMethodFraction();
                }

                MethodProfiler.BeginMethodFraction("NextWordStart");

                try
                {
                    int startWord = NextWordStart(text, startIndex);

                    while (startWord <= endIndex)
                    {
                        if (stopwatch.ElapsedTicks > maxTicks)
                        {
                            break;
                        }

                        int endWord = WordEnd(text, startWord);
                        int length = endWord - startWord + 1;

                        if (length == 0)
                        {
                            break;
                        }

                        string word = text.Substring(startWord, length);
                        word = word.ToUpper();
                        Color color = Color.Black;

                        foreach (KeyWordList keyWordList in keyWordLists)
                        {
                            if (Array.BinarySearch(keyWordList.KeyWords, word) >= 0)
                            {
                                color = keyWordList.Color;
                                break;
                            }
                        }

                        SetColor(startWord, length, color);

                        startWord = NextWordStart(text, endWord + 1);
                    }
                }
                finally
                {
                    MethodProfiler.EndMethodFraction();
                }

                MethodProfiler.BeginMethodFraction("Selection");

                try
                {
                    richTextBox.SelectionStart = orgSelectionStart;
                    richTextBox.SelectionLength = orgSelectionLength;
                    NativeMethods.SendMessage(hWnd, (int)NativeMethods.Message.Gdi.SetRedraw, 1, 0);
                    richTextBox.Refresh();
                }
                finally
                {
                    MethodProfiler.EndMethodFraction();
                }
            }
            finally
            {
                MethodProfiler.EndMethod();
            }
        }

        private void richTextBox_TextChanged(object sender, System.EventArgs e)
        {
            MethodProfiler.BeginMethod();

            try
            {
                if (changeEventEnabled)
                {
                    richTextBox.SelectionChanged -= new EventHandler(richTextBox_SelectionChanged);

                    string text = richTextBox.Text;

                    if (text.Length > 0)
                    {
                        int startIndex;
                        int endIndex;

                        if (prevSelectionStart < selectionStart)
                        {
                            startIndex = prevSelectionStart;
                            endIndex = selectionStart;
                        }
                        else
                        {
                            startIndex = selectionStart;
                            endIndex = Math.Min(prevSelectionStart, text.Length);
                        }

                        if (startIndex > 0)
                        {
                            startIndex--;
                        }
                        else
                        {
                            startIndex = 0;
                        }

                        if (endIndex > 0)
                        {
                            endIndex--;
                        }
                        else
                        {
                            endIndex = 0;
                        }

                        if (startIndex >= 0)
                        {
                            startIndex = WordStart(text, startIndex);
                            endIndex = WordEnd(text, endIndex);

                            // colorizing next word if necessary
                            if (selectionStart > 0)
                            {
                                char c = text[selectionStart - 1];

                                if (IsSeparator(c))
                                {
                                    if (selectionStart < text.Length)
                                    {
                                        char c2 = text[selectionStart];

                                        if (!IsSeparator(c2))
                                        {
                                            endIndex = WordEnd(text, endIndex + 2);
                                        }
                                    }
                                }
                            }

                            int length = endIndex - startIndex;

                            if (endIndex < text.Length - 1)
                            {
                                length++;
                            }

                            string s = text.Substring(startIndex, length);
                            this.Colorize(text, startIndex, endIndex);
                        }
                    }

                    richTextBox.SelectionChanged += new EventHandler(richTextBox_SelectionChanged);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                MethodProfiler.EndMethod();
            }
        }

        public static bool IsSeparator(char c)
        {
            bool isSeparator =
                c == ' ' ||
                c == '.' || c == ',' || c == ';' ||
                c == '=' ||
                c == '(' || c == ')' ||
                c == '[' || c == ']' ||
                c == '\t' ||
                c == '\r' || c == '\n' ||
                c == '+' || c == '-' || c == '*' || c == '/' ||
                c == '<' || c == '>';

            return isSeparator;
        }

        private void richTextBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            MethodProfiler.BeginMethod();

            try
            {
                if (keyboardHandler != null)
                {
                    e.Handled = keyboardHandler.HandleKeyDown(e);
                }
                else
                {
                    if (e.KeyCode == Keys.Insert && e.Shift)
                    {
                        Paste();
                        e.Handled = true;
                    }
                    else if (e.KeyCode == Keys.Tab)
                    {
                        e.Handled = true;
                        string text;
                        int length;

                        if (insertSpaces)
                        {
                            //text = new string( ' ', tabSize );

                            //1 -> 4
                            //2 -> 3
                            //3 -> 2
                            //4 -> 1
                            //5 -> 4
                            //6 -> 3
                            //7 -> 2                            

                            length = -((this.columnIndex - 1)%this.tabSize) + this.tabSize;
                            text = new string(' ', length);
                        }
                        else
                        {
                            text = new string('\t', 1);
                        }

                        length = richTextBox.SelectionLength;

                        if (length == 0)
                        {
                            richTextBox.SelectedText = text;
                        }
                    }
                    else if (e.KeyCode == Keys.E && e.Control)
                    {
                        e.Handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                MethodProfiler.EndMethod();
            }
        }

        private void richTextBox_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (keyboardHandler != null)
            {
                e.Handled = keyboardHandler.HandleKeyPress(e);
            }
            else
            {
                if (e.KeyChar == '\t')
                {
                    e.Handled = true;
                }
            }
        }

        private static bool GetDataPresent(IDataObject dataObject, string format)
        {
            DataFormats.Format dataFormat = DataFormats.GetFormat(format);
            string name = dataFormat.Name;
            return dataObject.GetDataPresent(name);
        }

        private static object GetData(IDataObject dataObject, string format)
        {
            DataFormats.Format dataFormat = DataFormats.GetFormat(format);
            string name = dataFormat.Name;
            return dataObject.GetData(name);
        }

        private void richTextBox_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void richTextBox_DragDrop(object sender, DragEventArgs e)
        {
            IDataObject dataObject = e.Data;

            if (GetDataPresent(dataObject, DataFormats.UnicodeText))
            {
                string text = (string)GetData(dataObject, DataFormats.UnicodeText);
                int startIndex = richTextBox.SelectionStart;
                richTextBox.SelectionLength = 0;
                richTextBox.SelectedText = text;
                richTextBox.SelectionStart = startIndex + text.Length;
                richTextBox.Focus();
            }
            else if (GetDataPresent(dataObject, DataFormats.FileDrop))
            {
                string[] fileNames = (string[])dataObject.GetData(DataFormats.FileDrop);
                Application.Instance.MainForm.LoadFiles(fileNames);
            }
        }

        public IKeyboardHandler KeyboardHandler
        {
            get
            {
                return keyboardHandler;
            }

            set
            {
                keyboardHandler = value;
            }
        }

        public int TabSize
        {
            get
            {
                return tabSize;
            }

            set
            {
                tabSize = value;
            }
        }

        private void CreateTable_Click(object sender, EventArgs e)
        {
            var queryForm = this.Parent as QueryForm;
            if (queryForm != null)
            {
                queryForm.ScriptQueryAsCreateTable();
            }
        }

        private void CopyTable_Click(object sender, EventArgs e)
        {
            var queryForm = this.Parent as QueryForm;
            if (queryForm != null)
            {
                queryForm.CopyTable();
            }
        }

        private void CopyTableWithSqlBulkCopy_Click(object sender, EventArgs e)
        {
            var queryForm = (QueryForm)this.Parent;
            queryForm.CopyTableWithSqlBulkCopy();
        }

        private void richTextBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenuStrip contextMenu = new ContextMenuStrip(this.components);
                ToolStripItemCollection items = contextMenu.Items;
                ToolStripMenuItem menuItem = new ToolStripMenuItem("Create table", null, this.CreateTable_Click);
                items.Add(menuItem);
                menuItem = new ToolStripMenuItem("Copy table", null, this.CopyTable_Click);
                items.Add(menuItem);

                Form[] forms = DataCommander.Providers.Application.Instance.MainForm.MdiChildren;
                int index = Array.IndexOf<Form>(forms, (QueryForm)this.Parent);
                if (index < forms.Length - 1)
                {
                    var nextQueryForm = (QueryForm)forms[index + 1];
                    var destinationProvider = nextQueryForm.Provider;
                    if (destinationProvider.DbProviderFactory == SqlClientFactory.Instance)
                    {
                        menuItem = new ToolStripMenuItem("Copy table with SqlBulkCopy", null, this.CopyTableWithSqlBulkCopy_Click);
                        items.Add(menuItem);
                    }
                }

                contextMenu.Show(this, e.Location);
            }
        }

        private sealed class KeyWordList
        {
            public string[] KeyWords;
            public Color Color;
        }
    }
}