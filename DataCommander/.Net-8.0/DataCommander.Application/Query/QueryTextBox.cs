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
    private UndoRedoState<string> _undoRedoState = new([], 0);

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
        RichTextBox.SelectionChanged += richTextBox_SelectionChanged;
        RichTextBox.DragEnter += richTextBox_DragEnter;
        RichTextBox.DragDrop += richTextBox_DragDrop;
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
            var keyWordList = new KeyWordList();
            keyWordList.KeyWords = new string[keyWords.Length];

            for (var i = 0; i < keyWords.Length; ++i)
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
                var text = RichTextBox.Text;
                Colorize(text, 0, text.Length - 1);
            }

            _changeEventEnabled = true;
        }
    }

    public string SelectedText => RichTextBox.SelectedText;

    public void Paste()
    {
        var format = DataFormats.GetFormat(DataFormats.UnicodeText);
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

    private void richTextBox_SelectionChanged(object sender, EventArgs e)
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

                var charIndex = _selectionStart;
                var line = RichTextBox.GetLineFromCharIndex(charIndex) + 1;
                var lineIndex = LineIndex(-1);
                var col = charIndex - lineIndex + 1;
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

    public int WordStart(string text, int index)
    {
        var i = index;
        var wordFound = false;

        while (i >= 0)
        {
            var c = text[i];

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
        var wordEnd = -1;
        string word = null;

        while (index >= 0)
        {
            var c = text[index];
            var isSeparator = IsSeparator(c) || index == 0;

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
        var length = text.Length;
        var i = index;

        while (i < length)
        {
            var c = text[i];

            if (IsSeparator(c))
                break;

            i++;
        }

        i--;

        return i;
    }

    public static int NextWordStart(string text, int index)
    {
        var length = text.Length;
        var i = index;

        while (i < length)
        {
            var c = text[i];

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
            var maxTicks = Stopwatch.Frequency * 5; // max. 5 seconds
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var orgSelectionStart = Math.Max(RichTextBox.SelectionStart, 0);
            var orgSelectionLength = Math.Max(RichTextBox.SelectionLength, 0);

            var intPtr = RichTextBox.Handle;
            var hWnd = intPtr.ToInt32();
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

            var subString = text.Substring(startIndex, endIndex - startIndex + 1);
            var tokenIterator = new TokenIterator(subString);
            var tokens = new List<Token>();
            while (true)
            {
                var token = tokenIterator.Next();
                if (token == null)
                    break;
                tokens.Add(token);
            }

            foreach (var token in tokens)
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
                        var keyWord = token.Value.ToUpper();
                        foreach (var keyWordList in _keyWordLists)
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
                RichTextBox.SelectionChanged -= richTextBox_SelectionChanged;

                var text = RichTextBox.Text;

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
                            var c = text[_selectionStart - 1];
                            if (IsSeparator(c))
                                if (_selectionStart < text.Length)
                                {
                                    var c2 = text[_selectionStart];
                                    if (!IsSeparator(c2))
                                        endIndex = WordEnd(text, _selectionStart);
                                }
                        }

                        var length = endIndex - startIndex;
                        if (endIndex < text.Length - 1)
                            length++;

                        var s = text.Substring(startIndex, length);
                        Colorize(text, startIndex, endIndex);
                    }
                }

                //_undoRedoState.Do(new[] {text});

                RichTextBox.SelectionChanged += richTextBox_SelectionChanged;
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
        var isSeparator =
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
                        var selectionLength = RichTextBox.SelectionLength;
                        if (selectionLength == 0)
                        {
                            selectionLength = -((_columnIndex - 1) % TabSize) + TabSize;
                            var text = new string(' ', selectionLength);
                            RichTextBox.SelectedText = text;
                        }
                        else
                        {
                            var selectionStart = RichTextBox.SelectionStart;
                            var startLine = RichTextBox.GetLineFromCharIndex(selectionStart);
                            var endLine = RichTextBox.GetLineFromCharIndex(selectionStart + selectionLength - 1);

                            var startCharIndex = RichTextBox.GetFirstCharIndexFromLine(startLine);
                            var firstCharIndex = RichTextBox.GetFirstCharIndexFromLine(endLine + 1);
                            var endCharIndex = firstCharIndex >= 0 ? firstCharIndex - 1 : RichTextBox.TextLength;

                            RichTextBox.SelectionStart = startCharIndex;
                            RichTextBox.SelectionLength = endCharIndex - startCharIndex + 1;

                            var selectedText = RichTextBox.SelectedText;
                            selectedText = selectedText.GetLines().Select(i => i.IncreaseLineIndent(TabSize)).Join("\n");
                            selectedText += "\n";
                            RichTextBox.SelectedText = selectedText;

                            RichTextBox.SelectionStart = startCharIndex;
                            RichTextBox.SelectionLength = selectedText.Length;
                        }
                    }
                    else if (e.Modifiers == Keys.Shift)
                    {
                        var selectionStart = RichTextBox.SelectionStart;
                        var selectionLength = RichTextBox.SelectionLength;
                        var startLine = RichTextBox.GetLineFromCharIndex(selectionStart);
                        var endLine = RichTextBox.GetLineFromCharIndex(selectionStart + selectionLength - 1);

                        var startCharIndex = RichTextBox.GetFirstCharIndexFromLine(startLine);
                        var firstCharIndex = RichTextBox.GetFirstCharIndexFromLine(endLine + 1);
                        var endCharIndex = firstCharIndex >= 0 ? firstCharIndex - 1 : RichTextBox.TextLength;

                        RichTextBox.SelectionStart = startCharIndex;
                        RichTextBox.SelectionLength = endCharIndex - startCharIndex + 1;

                        var tabString = new string(' ', TabSize);
                        var selectedText = RichTextBox.SelectedText;
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

    private void richTextBox_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (KeyboardHandler != null)
            e.Handled = KeyboardHandler.HandleKeyPress(e);
        else if (e.KeyChar == '\t')
            e.Handled = true;
    }

    private static bool GetDataPresent(IDataObject dataObject, string format)
    {
        var dataFormat = DataFormats.GetFormat(format);
        var name = dataFormat.Name;
        return dataObject.GetDataPresent(name);
    }

    private static object GetData(IDataObject dataObject, string format)
    {
        var dataFormat = DataFormats.GetFormat(format);
        var name = dataFormat.Name;
        return dataObject.GetData(name);
    }

    private void richTextBox_DragEnter(object sender, DragEventArgs e) => e.Effect = DragDropEffects.All;

    private void richTextBox_DragDrop(object sender, DragEventArgs e)
    {
        var dataObject = e.Data;

        if (GetDataPresent(dataObject, DataFormats.UnicodeText))
        {
            var text = (string)GetData(dataObject, DataFormats.UnicodeText);
            var path = text;

            if (File.Exists(path))
                DataCommanderApplication.Instance.MainForm.LoadFiles(path.ItemToArray());
            else if (Uri.TryCreate(path, UriKind.Absolute, out var uri))
            {
                if (uri.Scheme == "file")
                {
                    path = uri.LocalPath;
                    DataCommanderApplication.Instance.MainForm.LoadFiles(path.ItemToArray());
                }
            }
            else
            {
                var startIndex = RichTextBox.SelectionStart;
                RichTextBox.SelectionLength = 0;
                RichTextBox.SelectedText = text;
                RichTextBox.SelectionStart = startIndex + text.Length;
                RichTextBox.Focus();
            }
        }
        else if (GetDataPresent(dataObject, DataFormats.FileDrop))
        {
            var fileNames = (string[])dataObject.GetData(DataFormats.FileDrop);
            var fileName = fileNames[0];
            var extension = Path.GetExtension(fileName);
            if (extension.In(".sql", ".txt"))
                DataCommanderApplication.Instance.MainForm.LoadFiles(fileNames);
            else
            {
                var bytes = File.ReadAllBytes(fileNames[0]);
                var chars = Hex.Encode(bytes, true);
                var stringBuilder = new StringBuilder();
                stringBuilder.Append("0x");
                stringBuilder.Append(chars);
                var text = stringBuilder.ToString();
                var startIndex = RichTextBox.SelectionStart;
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
        var queryForm = Parent as QueryForm;
        if (queryForm != null)
            queryForm.ScriptQueryAsCreateTable();
    }

    private void CopyTable_Click(object sender, EventArgs e)
    {
        var queryForm = Parent as QueryForm;
        if (queryForm != null)
            queryForm.CopyTable();
    }

    private void CopyTableWithSqlBulkCopy_Click(object sender, EventArgs e)
    {
        var queryForm = (QueryForm)Parent;
        queryForm.CopyTableWithSqlBulkCopy();
    }

    private void richTextBox_MouseUp(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            var contextMenu = new ContextMenuStrip(_components);
            var items = contextMenu.Items;
            var menuItem = new ToolStripMenuItem("Create table", null, CreateTable_Click);
            items.Add(menuItem);
            menuItem = new ToolStripMenuItem("Copy table", null, CopyTable_Click);
            items.Add(menuItem);

            var forms = DataCommanderApplication.Instance.MainForm.MdiChildren;
            var index = Array.IndexOf(forms, (QueryForm)Parent);
            if (index < forms.Length - 1)
            {
                var nextQueryForm = (QueryForm)forms[index + 1];
                var destinationProvider = nextQueryForm.Provider;
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