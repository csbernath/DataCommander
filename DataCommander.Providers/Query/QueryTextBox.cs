namespace DataCommander.Providers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Forms;
    using DataCommander.Foundation.Diagnostics;
    using Foundation.Diagnostics.MethodProfiler;

    public sealed class QueryTextBox : UserControl
    {
        private readonly List<KeyWordList> keyWordLists = new List<KeyWordList>();
        private int selectionStart = 0;
        private int selectionLength = 0;
        private int prevSelectionStart;
        private int prevSelectionLength;
        private bool changeEventEnabled = true;
        private ToolStripStatusLabel sbPanel;
        private int columnIndex;

        public RichTextBox RichTextBox { get; private set; }

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private readonly Container components = new Container();

        public QueryTextBox()
        {
            // This call is required by the Windows.Forms Form Designer.
            this.InitializeComponent();

            // TODO: Add any initialization after the InitForm call
            this.RichTextBox.SelectionChanged += new EventHandler(this.richTextBox_SelectionChanged);
            this.RichTextBox.DragEnter += new DragEventHandler(this.richTextBox_DragEnter);
            this.RichTextBox.DragDrop += new DragEventHandler(this.richTextBox_DragDrop);
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

                this.keyWordLists.Add(keyWordList);
            }
        }

        public ToolStripStatusLabel CaretPositionPanel
        {
            set
            {
                this.sbPanel = value;
            }
        }

        public override string Text
        {
            get
            {
                return this.RichTextBox.Text;
            }

            set
            {
                this.changeEventEnabled = false;
                this.RichTextBox.Text = value;

                if (value != null)
                {
                    string text = this.RichTextBox.Text;
                    this.Colorize(text, 0, text.Length - 1);
                }

                this.changeEventEnabled = true;
            }
        }

        public string SelectedText => this.RichTextBox.SelectedText;

        public void Paste()
        {
            var format = DataFormats.GetFormat(DataFormats.UnicodeText);
            this.RichTextBox.Paste(format);
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.components != null)
                {
                    this.components.Dispose();
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
            this.RichTextBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // richTextBox
            // 
            this.RichTextBox.AcceptsTab = true;
            this.RichTextBox.AllowDrop = true;
            this.RichTextBox.AutoWordSelection = true;
            this.RichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RichTextBox.Location = new System.Drawing.Point(0, 0);
            this.RichTextBox.Name = "RichTextBox";
            this.RichTextBox.Size = new System.Drawing.Size(408, 150);
            this.RichTextBox.TabIndex = 0;
            this.RichTextBox.Text = "";
            this.RichTextBox.WordWrap = false;
            this.RichTextBox.TextChanged += new System.EventHandler(this.richTextBox_TextChanged);
            this.RichTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.richTextBox_KeyDown);
            this.RichTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.richTextBox_KeyPress);
            this.RichTextBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.richTextBox_MouseUp);
            // 
            // QueryTextBox
            // 
            this.Controls.Add(this.RichTextBox);
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
                this.RichTextBox.Select(startWord, length);
                this.RichTextBox.SelectionColor = color;
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
            return NativeMethods.SendMessage(richTextBox.Handle.ToInt32(), (int) NativeMethods.Message.EditBox.LineIndex,
                i, 0);
        }

        private int LineIndex(int i)
        {
            return GetLineIndex(this.RichTextBox, i);
        }

        private void richTextBox_SelectionChanged(object sender, EventArgs e)
        {
            MethodProfiler.BeginMethod();

            try
            {
                this.prevSelectionStart = this.selectionStart;
                this.selectionStart = this.RichTextBox.SelectionStart;
                this.prevSelectionLength = this.selectionLength;
                this.selectionLength = this.RichTextBox.SelectionLength;

                int charIndex = this.selectionStart;
                int line = this.RichTextBox.GetLineFromCharIndex(charIndex) + 1;
                int lineIndex = this.LineIndex(-1);
                int col = charIndex - lineIndex + 1;
                this.sbPanel.Text = "Ln " + line + " Col " + col;
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

                int orgSelectionStart = Math.Max(this.RichTextBox.SelectionStart, 0);
                int orgSelectionLength = Math.Max(this.RichTextBox.SelectionLength, 0);

                IntPtr intPtr = this.RichTextBox.Handle;
                int hWnd = intPtr.ToInt32();
                NativeMethods.SendMessage(hWnd, (int) NativeMethods.Message.Gdi.SetRedraw, 0, 0);

                MethodProfiler.BeginMethodFraction("ControlText");

                try
                {
                    this.RichTextBox.SelectionStart = startIndex;
                    this.RichTextBox.SelectionLength = endIndex - startIndex + 1;
                    this.RichTextBox.SelectionColor = SystemColors.ControlText;
                }
                finally
                {
                    MethodProfiler.EndMethodFraction();
                }

                var subString = text.Substring(startIndex, endIndex - startIndex + 1);
                var tokenIterator = new TokenIterator(subString);
                var tokens = new List<Token>();
                while (true)
                {
                    var token = tokenIterator.Next();
                    if (token == null)
                    {
                        break;
                    }
                    tokens.Add(token);
                }

                foreach (var token in tokens)
                {
                    Color? color = null;

                    switch (token.Type)
                    {
                        case TokenType.KeyWord:
                            color = Color.Black;
                            string keyWord = token.Value.ToUpper();
                            foreach (KeyWordList keyWordList in this.keyWordLists)
                            {
                                if (Array.BinarySearch(keyWordList.KeyWords, keyWord) >= 0)
                                {
                                    color = keyWordList.Color;
                                    break;
                                }
                            }
                            break;

                        case TokenType.String:
                            color = Color.Red;
                            break;
                    }

                    if (color != null)
                    {
                        this.SetColor(startIndex + token.StartPosition, token.EndPosition - token.StartPosition + 1,
                            color.Value);
                    }
                }

                //MethodProfiler.BeginMethodFraction("NextWordStart");

                //try
                //{
                //    int startWord = NextWordStart(text, startIndex);

                //    while (startWord <= endIndex)
                //    {
                //        if (stopwatch.ElapsedTicks > maxTicks)
                //        {
                //            break;
                //        }

                //        int endWord = WordEnd(text, startWord);
                //        int length = endWord - startWord + 1;

                //        if (length == 0)
                //        {
                //            break;
                //        }

                //        string word = text.Substring(startWord, length);
                //        word = word.ToUpper();
                //        Color color = Color.Black;

                //        foreach (KeyWordList keyWordList in this.keyWordLists)
                //        {
                //            if (Array.BinarySearch(keyWordList.KeyWords, word) >= 0)
                //            {
                //                color = keyWordList.Color;
                //                break;
                //            }
                //        }

                //        this.SetColor(startWord, length, color);

                //        startWord = NextWordStart(text, endWord + 1);
                //    }
                //}
                //finally
                //{
                //    MethodProfiler.EndMethodFraction();
                //}

                MethodProfiler.BeginMethodFraction("Selection");

                try
                {
                    this.RichTextBox.SelectionStart = orgSelectionStart;
                    this.RichTextBox.SelectionLength = orgSelectionLength;
                    NativeMethods.SendMessage(hWnd, (int) NativeMethods.Message.Gdi.SetRedraw, 1, 0);
                    this.RichTextBox.Refresh();
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

        private void richTextBox_TextChanged(object sender, EventArgs e)
        {
            MethodProfiler.BeginMethod();

            try
            {
                if (this.changeEventEnabled)
                {
                    this.RichTextBox.SelectionChanged -= new EventHandler(this.richTextBox_SelectionChanged);

                    string text = this.RichTextBox.Text;

                    if (text.Length > 0)
                    {
                        int startIndex;
                        int endIndex;

                        if (this.prevSelectionStart < this.selectionStart)
                        {
                            startIndex = this.prevSelectionStart;
                            endIndex = this.selectionStart;
                        }
                        else
                        {
                            startIndex = this.selectionStart;
                            endIndex = Math.Min(this.prevSelectionStart, text.Length);
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
                            startIndex = this.WordStart(text, startIndex);
                            endIndex = WordEnd(text, endIndex);

                            // colorizing next word if necessary
                            if (this.selectionStart > 0)
                            {
                                char c = text[this.selectionStart - 1];

                                if (IsSeparator(c))
                                {
                                    if (this.selectionStart < text.Length)
                                    {
                                        char c2 = text[this.selectionStart];

                                        if (!IsSeparator(c2))
                                        {
                                            endIndex = WordEnd(text, selectionStart);
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

                    this.RichTextBox.SelectionChanged += new EventHandler(this.richTextBox_SelectionChanged);
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

        private void richTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            MethodProfiler.BeginMethod();

            try
            {
                if (this.KeyboardHandler != null)
                {
                    e.Handled = this.KeyboardHandler.HandleKeyDown(e);
                }
                else
                {
                    if (e.KeyCode == Keys.Insert && e.Shift)
                    {
                        this.Paste();
                        e.Handled = true;
                    }
                    else if (e.KeyCode == Keys.Tab)
                    {
                        e.Handled = true;

                        int length = -((this.columnIndex - 1)%this.TabSize) + this.TabSize;
                        string text = new string(' ', length);
                        length = this.RichTextBox.SelectionLength;

                        if (length == 0)
                        {
                            this.RichTextBox.SelectedText = text;
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

        private void richTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (this.KeyboardHandler != null)
            {
                e.Handled = this.KeyboardHandler.HandleKeyPress(e);
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
                string text = (string) GetData(dataObject, DataFormats.UnicodeText);
                int startIndex = this.RichTextBox.SelectionStart;
                this.RichTextBox.SelectionLength = 0;
                this.RichTextBox.SelectedText = text;
                this.RichTextBox.SelectionStart = startIndex + text.Length;
                this.RichTextBox.Focus();
            }
            else if (GetDataPresent(dataObject, DataFormats.FileDrop))
            {
                string[] fileNames = (string[]) dataObject.GetData(DataFormats.FileDrop);
                DataCommanderApplication.Instance.MainForm.LoadFiles(fileNames);
            }
        }

        internal IKeyboardHandler KeyboardHandler { get; set; }

        public int TabSize { get; set; } = 4;

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
            var queryForm = (QueryForm) this.Parent;
            queryForm.CopyTableWithSqlBulkCopy();
        }

        private void richTextBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var contextMenu = new ContextMenuStrip(this.components);
                ToolStripItemCollection items = contextMenu.Items;
                var menuItem = new ToolStripMenuItem("Create table", null, this.CreateTable_Click);
                items.Add(menuItem);
                menuItem = new ToolStripMenuItem("Copy table", null, this.CopyTable_Click);
                items.Add(menuItem);

                Form[] forms = DataCommanderApplication.Instance.MainForm.MdiChildren;
                int index = Array.IndexOf(forms, (QueryForm) this.Parent);
                if (index < forms.Length - 1)
                {
                    var nextQueryForm = (QueryForm) forms[index + 1];
                    var destinationProvider = nextQueryForm.Provider;
                    if (destinationProvider.DbProviderFactory == SqlClientFactory.Instance)
                    {
                        menuItem = new ToolStripMenuItem("Copy table with SqlBulkCopy", null,
                            this.CopyTableWithSqlBulkCopy_Click);
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