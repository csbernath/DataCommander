using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DataCommander.Api;
using DataCommander.Api.Query;
using Foundation.Core;
using Foundation.Data.MethodProfiler;
using Foundation.Linq;
using Foundation.Text;

namespace DataCommander.Application.Query;

public sealed class QueryTextBox : UserControl
{
    private readonly List<KeyWordList> _keyWordLists = [];
    private int _selectionStart = 0;
    private int _selectionLength = 0;
    private int _prevSelectionStart;
    private int _prevSelectionLength;
    private bool _changeEventEnabled = true;
    private ToolStripStatusLabel _sbPanel;
    private int _columnIndex;
    private ColorTheme _colorTheme;
    private readonly UndoRedoState<string> _undoRedoState = new([], 0);

    public RichTextBox RichTextBox { get; private set; }

    public bool EnableChangeEvent(bool enabled) => _changeEventEnabled = enabled;

    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private readonly Container _components = new();

    public QueryTextBox()
    {
        // This call is required by the Windows.Forms Form Designer.
        InitializeComponent();

        // TODO: Add any initialization after the InitForm call
        RichTextBox.SelectionChanged += RichTextBox_SelectionChanged;
        RichTextBox.DragEnter += richTextBox_DragEnter;
        RichTextBox.DragDrop += RichTextBox_DragDrop;
    }

    public void SetColorTheme(ColorTheme colorTheme)
    {
        _colorTheme = colorTheme;

        if (colorTheme != null)
        {
            BackColor = colorTheme.BackColor;
            ForeColor = colorTheme.ForeColor;

            //EnableChangeEvent(false);
            RichTextBox.BackColor = colorTheme.BackColor;
            RichTextBox.ForeColor = colorTheme.ForeColor;
            //EnableChangeEvent(true);
        }
    }

    public void AddKeyWords(string[] keyWords, Color color)
    {
        if (keyWords != null)
        {
            KeyWordList keyWordList = new KeyWordList
            {
                KeyWords = new string[keyWords.Length]
            };

            for (int i = 0; i < keyWords.Length; ++i)
                keyWordList.KeyWords[i] = keyWords[i].ToUpper();

            keyWordList.Color = color;

            _keyWordLists.Add(keyWordList);
        }
    }

    public ToolStripStatusLabel CaretPositionPanel
    {
        set => _sbPanel = value;
    }

    public override string Text
    {
        get => RichTextBox.Text;

        set
        {
            _changeEventEnabled = false;
            RichTextBox.Text = value;

            if (value != null)
            {
                string text = RichTextBox.Text;
                Colorize(text, 0, text.Length - 1);
            }

            _changeEventEnabled = true;
        }
    }

    public string SelectedText => RichTextBox.SelectedText;

    public void Paste()
    {
        DataFormats.Format format = DataFormats.GetFormat(DataFormats.UnicodeText);
        RichTextBox.Paste(format);
    }

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
            if (_components != null)
                _components.Dispose();

        base.Dispose(disposing);
    }

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
        this.RichTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RichTextBox_KeyPress);
        this.RichTextBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.richTextBox_MouseUp);
        // 
        // QueryTextBox
        // 
        this.Controls.Add(this.RichTextBox);
        this.Name = "QueryTextBox";
        this.Size = new System.Drawing.Size(408, 150);
        this.ResumeLayout(false);
    }

    private void SetColor(int startWord, int length, Color color)
    {
        MethodProfiler.BeginMethod();

        try
        {
            RichTextBox.Select(startWord, length);
            RichTextBox.SelectionColor = color;
        }
        finally
        {
            MethodProfiler.EndMethod();
        }
    }

    public static int GetLineIndex(RichTextBox richTextBox, int i)
    {
        return NativeMethods.SendMessage(richTextBox.Handle.ToInt32(), (int)NativeMethods.Message.EditBox.LineIndex, i, 0);
    }

    private int LineIndex(int i)
    {
        return GetLineIndex(RichTextBox, i);
    }

    private void RichTextBox_SelectionChanged(object sender, EventArgs e)
    {
        MethodProfiler.BeginMethod();

        if (_sbPanel != null)
        {

            try
            {
                _prevSelectionStart = _selectionStart;
                _selectionStart = RichTextBox.SelectionStart;
                _prevSelectionLength = _selectionLength;
                _selectionLength = RichTextBox.SelectionLength;

                int charIndex = _selectionStart;
                int line = RichTextBox.GetLineFromCharIndex(charIndex) + 1;
                int lineIndex = LineIndex(-1);
                int col = charIndex - lineIndex + 1;
                _sbPanel.Text = "Ln " + line + " Col " + col;
                _columnIndex = col;
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
    }

    public static int WordStart(string text, int index)
    {
        int i = index;
        bool wordFound = false;

        while (i >= 0)
        {
            char c = text[i];

            if (wordFound && IsSeparator(c))
                break;
            else
                wordFound = true;

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
                    index++;

                word = text.Substring(index, wordEnd - index + 1);
                break;
            }
            else if (wordEnd == -1 && !isSeparator)
                wordEnd = index;

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
                break;

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
                break;

            i++;
        }

        return i;
    }

    private void Colorize(string text, int startIndex, int endIndex)
    {
        MethodProfiler.BeginMethod();

        try
        {
            long maxTicks = Stopwatch.Frequency * 5; // max. 5 seconds
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            int orgSelectionStart = Math.Max(RichTextBox.SelectionStart, 0);
            int orgSelectionLength = Math.Max(RichTextBox.SelectionLength, 0);

            nint intPtr = RichTextBox.Handle;
            int hWnd = intPtr.ToInt32();
            NativeMethods.SendMessage(hWnd, (int)NativeMethods.Message.Gdi.SetRedraw, 0, 0);

            MethodProfiler.BeginMethodFraction("ControlText");

            try
            {
                RichTextBox.SelectionStart = startIndex;
                RichTextBox.SelectionLength = endIndex - startIndex + 1;
                RichTextBox.SelectionColor = _colorTheme != null
                    ? _colorTheme.ForeColor
                    : SystemColors.ControlText;
            }
            finally
            {
                MethodProfiler.EndMethodFraction();
            }

            string subString = text.Substring(startIndex, endIndex - startIndex + 1);
            TokenIterator tokenIterator = new TokenIterator(subString);
            List<Token> tokens = [];
            while (true)
            {
                Token token = tokenIterator.Next();
                if (token == null)
                    break;
                tokens.Add(token);
            }

            foreach (Token token in tokens)
            {
                if (maxTicks < stopwatch.ElapsedTicks)
                    break;
                
                Color? color = null;

                switch (token.Type)
                {
                    case TokenType.KeyWord:
                        color = _colorTheme != null
                            ? _colorTheme.ForeColor
                            : Color.Black;
                        string keyWord = token.Value.ToUpper();
                        foreach (KeyWordList keyWordList in _keyWordLists)
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
                    SetColor(startIndex + token.StartPosition, token.EndPosition - token.StartPosition + 1,
                        color.Value);
                }
            }

            MethodProfiler.BeginMethodFraction("Selection");

            try
            {
                RichTextBox.SelectionStart = orgSelectionStart;
                RichTextBox.SelectionLength = orgSelectionLength;
                NativeMethods.SendMessage(hWnd, (int)NativeMethods.Message.Gdi.SetRedraw, 1, 0);
                RichTextBox.Refresh();
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
            if (_changeEventEnabled)
            {
                RichTextBox.SelectionChanged -= RichTextBox_SelectionChanged;

                string text = RichTextBox.Text;

                if (text.Length > 0)
                {
                    int startIndex;
                    int endIndex;

                    if (_prevSelectionStart < _selectionStart)
                    {
                        startIndex = _prevSelectionStart;
                        endIndex = _selectionStart;
                    }
                    else
                    {
                        startIndex = _selectionStart;
                        endIndex = Math.Min(_prevSelectionStart, text.Length);
                    }

                    if (startIndex > 0)
                        startIndex--;
                    else
                        startIndex = 0;

                    if (endIndex > 0)
                        endIndex--;
                    else
                        endIndex = 0;

                    if (startIndex >= 0)
                    {
                        startIndex = WordStart(text, startIndex);
                        endIndex = WordEnd(text, endIndex);

                        // colorizing next word if necessary
                        if (_selectionStart > 0)
                        {
                            char c = text[_selectionStart - 1];
                            if (IsSeparator(c))
                                if (_selectionStart < text.Length)
                                {
                                    char c2 = text[_selectionStart];
                                    if (!IsSeparator(c2))
                                        endIndex = WordEnd(text, _selectionStart);
                                }
                        }

                        int length = endIndex - startIndex;
                        if (endIndex < text.Length - 1)
                            length++;

                        string s = text.Substring(startIndex, length);
                        Colorize(text, startIndex, endIndex);
                    }
                }

                //_undoRedoState.Do(new[] {text});

                RichTextBox.SelectionChanged += RichTextBox_SelectionChanged;
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
            if (KeyboardHandler != null)
                e.Handled = KeyboardHandler.HandleKeyDown(e);
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

                    if (e.Modifiers == Keys.None)
                    {
                        int selectionLength = RichTextBox.SelectionLength;
                        if (selectionLength == 0)
                        {
                            selectionLength = -((_columnIndex - 1) % TabSize) + TabSize;
                            string text = new string(' ', selectionLength);
                            RichTextBox.SelectedText = text;
                        }
                        else
                        {
                            int selectionStart = RichTextBox.SelectionStart;
                            int startLine = RichTextBox.GetLineFromCharIndex(selectionStart);
                            int endLine = RichTextBox.GetLineFromCharIndex(selectionStart + selectionLength - 1);

                            int startCharIndex = RichTextBox.GetFirstCharIndexFromLine(startLine);
                            int firstCharIndex = RichTextBox.GetFirstCharIndexFromLine(endLine + 1);
                            int endCharIndex = firstCharIndex >= 0 ? firstCharIndex - 1 : RichTextBox.TextLength;

                            RichTextBox.SelectionStart = startCharIndex;
                            RichTextBox.SelectionLength = endCharIndex - startCharIndex + 1;

                            string selectedText = RichTextBox.SelectedText;
                            selectedText = selectedText.GetLines().Select(i => i.IncreaseLineIndent(TabSize)).Join("\n");
                            selectedText += "\n";
                            RichTextBox.SelectedText = selectedText;

                            RichTextBox.SelectionStart = startCharIndex;
                            RichTextBox.SelectionLength = selectedText.Length;
                        }
                    }
                    else if (e.Modifiers == Keys.Shift)
                    {
                        int selectionStart = RichTextBox.SelectionStart;
                        int selectionLength = RichTextBox.SelectionLength;
                        int startLine = RichTextBox.GetLineFromCharIndex(selectionStart);
                        int endLine = RichTextBox.GetLineFromCharIndex(selectionStart + selectionLength - 1);

                        int startCharIndex = RichTextBox.GetFirstCharIndexFromLine(startLine);
                        int firstCharIndex = RichTextBox.GetFirstCharIndexFromLine(endLine + 1);
                        int endCharIndex = firstCharIndex >= 0 ? firstCharIndex - 1 : RichTextBox.TextLength;

                        RichTextBox.SelectionStart = startCharIndex;
                        RichTextBox.SelectionLength = endCharIndex - startCharIndex + 1;

                        string tabString = new string(' ', TabSize);
                        string selectedText = RichTextBox.SelectedText;
                        selectedText = selectedText.GetLines()
                            .Select(i => i.Replace("\t", tabString))
                            .Select(i => i.DecreaseLineIndent(TabSize)).Join("\n");
                        selectedText += '\n';
                        RichTextBox.SelectedText = selectedText;

                        RichTextBox.SelectionStart = startCharIndex;
                        RichTextBox.SelectionLength = selectedText.Length;
                    }
                }
                else if (e.KeyCode == Keys.E && e.Control)
                    e.Handled = true;
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

    private void RichTextBox_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (KeyboardHandler != null)
            e.Handled = KeyboardHandler.HandleKeyPress(e);
        else if (e.KeyChar == '\t')
            e.Handled = true;
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

    private void richTextBox_DragEnter(object sender, DragEventArgs e) => e.Effect = DragDropEffects.All;

    private void RichTextBox_DragDrop(object sender, DragEventArgs e)
    {
        IDataObject? dataObject = e.Data;

        if (GetDataPresent(dataObject, DataFormats.UnicodeText))
        {
            string text = (string)GetData(dataObject, DataFormats.UnicodeText);
            string path = text;

            if (File.Exists(path))
                DataCommanderApplication.Instance.MainForm.LoadFiles(path.ItemToArray());
            else if (Uri.TryCreate(path, UriKind.Absolute, out Uri? uri))
            {
                if (uri.Scheme == "file")
                {
                    path = uri.LocalPath;
                    DataCommanderApplication.Instance.MainForm.LoadFiles(path.ItemToArray());
                }
            }
            else
            {
                int startIndex = RichTextBox.SelectionStart;
                RichTextBox.SelectionLength = 0;
                RichTextBox.SelectedText = text;
                RichTextBox.SelectionStart = startIndex + text.Length;
                RichTextBox.Focus();
            }
        }
        else if (GetDataPresent(dataObject, DataFormats.FileDrop))
        {
            string[]? fileNames = (string[])dataObject.GetData(DataFormats.FileDrop);
            string fileName = fileNames[0];
            string extension = Path.GetExtension(fileName);
            if (extension.In(".sql", ".txt"))
                DataCommanderApplication.Instance.MainForm.LoadFiles(fileNames);
            else
            {
                byte[] bytes = File.ReadAllBytes(fileNames[0]);
                char[] chars = Hex.Encode(bytes, true);
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("0x");
                stringBuilder.Append(chars);
                string text = stringBuilder.ToString();
                int startIndex = RichTextBox.SelectionStart;
                RichTextBox.SelectionLength = 0;
                RichTextBox.SelectedText = text;
                RichTextBox.SelectionStart = startIndex + text.Length;
                RichTextBox.Focus();
            }
        }
    }

    internal IKeyboardHandler KeyboardHandler { get; set; }

    public int TabSize { get; set; } = 4;

    private void CreateTable_Click(object sender, EventArgs e)
    {
        if (Parent is QueryForm queryForm)
            queryForm.ScriptQueryAsCreateTable();
    }

    private void CopyTable_Click(object sender, EventArgs e)
    {
        if (Parent is QueryForm queryForm)
            queryForm.CopyTable();
    }

    private void CopyTableWithSqlBulkCopy_Click(object sender, EventArgs e)
    {
        QueryForm? queryForm = (QueryForm)Parent;
        QueryForm.CopyTableWithSqlBulkCopy();
    }

    private void richTextBox_MouseUp(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            ContextMenuStrip contextMenu = new ContextMenuStrip(_components);
            ToolStripItemCollection items = contextMenu.Items;
            ToolStripMenuItem menuItem = new ToolStripMenuItem("Create table", null, CreateTable_Click);
            items.Add(menuItem);
            menuItem = new ToolStripMenuItem("Copy table", null, CopyTable_Click);
            items.Add(menuItem);

            Form[] forms = DataCommanderApplication.Instance.MainForm.MdiChildren;
            int index = Array.IndexOf(forms, (QueryForm)Parent);
            if (index < forms.Length - 1)
            {
                QueryForm nextQueryForm = (QueryForm)forms[index + 1];
                IProvider destinationProvider = nextQueryForm.Provider;
                if (destinationProvider.DbProviderFactory == SqlClientFactory.Instance)
                {
                    menuItem = new ToolStripMenuItem("Copy table with SqlBulkCopy", null,
                        CopyTableWithSqlBulkCopy_Click);
                    items.Add(menuItem);
                }
            }

            contextMenu.Show(this, e.Location);
        }
    }

    public void Undo()
    {
        //_undoRedoState.Undo(1, items =>
        //{
        //    var item = items.Last();

        //    EnableChangeEvent(false);
        //    RichTextBox.Text = item;
        //    EnableChangeEvent(true);
        //});
    }

    public void Redo()
    {
        //_undoRedoState.Redo(1, items =>
        //{
        //    var item = items.First();

        //    EnableChangeEvent(false);
        //    RichTextBox.Text = item;
        //    EnableChangeEvent(true);
        //});
    }

    private sealed class KeyWordList
    {
        public string[] KeyWords;
        public Color Color;
    }
}