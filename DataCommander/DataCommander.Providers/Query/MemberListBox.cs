using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Foundation.Collections;
using Foundation.Linq;

namespace DataCommander.Providers.Query
{
    internal sealed class MemberListBox : UserControl, IKeyboardHandler
    {
        private readonly CompletionForm _completionForm;
        private readonly QueryTextBox _textBox;
        private GetCompletionResponse _response;
        private string _prefix = string.Empty;
        private readonly Container _components = null;

        public MemberListBox(CompletionForm completionForm, QueryTextBox textBox, ColorTheme colorTheme)
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitForm call
            _completionForm = completionForm;
            _textBox = textBox;

            if (colorTheme != null)
            {
                ListBox.ForeColor = colorTheme.ForeColor;
                ListBox.BackColor = colorTheme.BackColor;
            }
        }

        private static string ToString(IObjectName objectName)
        {
            return objectName.UnquotedName;
        }

        private void LoadItems()
        {
            ListBox.Items.Clear();

            foreach (var item in _response.Items)
            {
                var listBoxItem = new ListBoxItem<IObjectName>(item, ToString);
                ListBox.Items.Add(listBoxItem);
            }
        }

        public void Initialize(GetCompletionResponse response)
        {
            _response = response;
            LoadItems();
            _textBox.KeyboardHandler = this;

            if (response.Items.Count > 0)
            {
                _prefix = _textBox.Text.Substring(response.StartPosition, response.Length);
                if (_prefix.Length > 0)
                {
                    var items = _prefix.Split('.');

                    var count = response.Items[0].UnquotedName.Count(c => c == '.') + 1;
                    var stringBuilder = new StringBuilder();
                    for (var i = Math.Max(items.Length - count, 0); i < items.Length; i++)
                    {
                        if (stringBuilder.Length > 0)
                            stringBuilder.Append('.');
                        stringBuilder.Append(items[i]);
                    }

                    _prefix = stringBuilder.ToString().ToLower();
                    Find(_prefix, 0);
                }
            }
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            ListBox = new System.Windows.Forms.ListBox();
            SuspendLayout();
            // 
            // listBox
            // 
            ListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            ListBox.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point,
                ((System.Byte) (238)));
            ListBox.Name = "ListBox";
            ListBox.Size = new System.Drawing.Size(180, 134);
            ListBox.TabIndex = 0;
            ListBox.DoubleClick += new System.EventHandler(listBox_DoubleClick);
            // 
            // MemberListBox
            // 
            Controls.AddRange(new System.Windows.Forms.Control[]
            {
                ListBox
            });
            Name = "MemberListBox";
            Size = new System.Drawing.Size(180, 142);
            ResumeLayout(false);

        }

        #endregion

        private void Close()
        {
            _textBox.KeyboardHandler = null;
            var form = (Form) Parent;
            form.Controls.Remove(this);
            form.Close();
        }

        private void SelectItem()
        {
            var listBoxItem = (ListBoxItem<IObjectName>) ListBox.SelectedItem;

            if (listBoxItem != null)
            {
                var selectedItem = listBoxItem.Item.UnquotedName;
                var startIndex = _response.StartPosition;
                var tokenIterator = new TokenIterator(_textBox.Text.Substring(startIndex));
                var token = tokenIterator.Next();
                int length;
                if (token != null && token.StartPosition == 0)
                    length = token.EndPosition - token.StartPosition + 1;
                else
                    length = 0;

                var originalText = _textBox.Text.Substring(startIndex, length);
                var originalItems = originalText.Split('.');
                var newItems = selectedItem.Split('.');
                var sb = new StringBuilder();
                for (var i = 0; i < originalItems.Length - newItems.Length; i++)
                {
                    if (sb.Length > 0)
                        sb.Append('.');
                    sb.Append(originalItems[i]);
                }

                for (var i = 0; i < newItems.Length; i++)
                {
                    if (sb.Length > 0)
                        sb.Append('.');
                    sb.Append(newItems[i]);
                }

                var newText = sb.ToString();

                // TODO
                _completionForm.SelectItem(startIndex, length, listBoxItem.Item);
            }
        }

        private void listBox_DoubleClick(object sender, EventArgs e)
        {
            SelectItem();
            Close();
        }

        public ListBox ListBox { get; private set; }

        public bool HandleKeyDown(KeyEventArgs e)
        {
            bool handled;
            var hWnd = ListBox.Handle.ToInt32();
            NativeMethods.SendMessage(hWnd, (int) NativeMethods.Message.Keyboard.KeyDown, (int) e.KeyCode, 0);

            if (e.KeyCode.In(Keys.Down, Keys.Up, Keys.PageDown, Keys.PageUp, Keys.Home, Keys.End))
            {
                handled = true;
                if (e.KeyCode == Keys.Down && e.Shift)
                {
                    var startIndex = ListBox.SelectedIndex + 1;
                    if (startIndex < ListBox.Items.Count - 1)
                        FindNext(startIndex);
                }
                else if (e.KeyCode == Keys.Up && e.Shift)
                {
                    var startIndex = ListBox.SelectedIndex - 1;
                    if (startIndex > 0)
                        FindPrevious(startIndex);
                }
            }
            else if (e.KeyCode.In(Keys.Subtract, Keys.OemMinus) && e.Control)
            {
                handled = true;
                var filteredItems = ListBox.Items.Cast<ListBoxItem<IObjectName>>()
                    .Where(item => IndexOf(item.Item.UnquotedName, _prefix) >= 0)
                    .ToArray();
                ListBox.Items.Clear();
                ListBox.Items.AddRange(filteredItems);
            }
            else if (e.KeyCode == Keys.Enter)
                handled = true;
            else
                handled = false;

            return handled;
        }

        private void Find(string prefix, int startIndex)
        {
            var filteredItems = ListBox.Items
                .Cast<ListBoxItem<IObjectName>>()
                .Select((listBoxItem, i) => new
                {
                    Index = i,
                    IndexOf = IndexOf(listBoxItem.Item.UnquotedName, prefix)
                })
                .Where(item => item.IndexOf >= 0)
                .ToList();

            var index = -1;

            if (filteredItems.Count > 0)
                index = filteredItems.MinIndexedItem(i => i.IndexOf).Value.Index;

            if (index >= 0)
            {
                ListBox.SelectedIndex = index;
                if (index >= 3)
                {
                    // scrolling 3 items up
                    var wParam = (int) NativeMethods.Message.ScrollBarParameter.ThumbPosition;
                    var pos = (index - 3) << 16;
                    wParam += pos;
                    var hWnd = ListBox.Handle.ToInt32();
                    NativeMethods.SendMessage(hWnd, (int) NativeMethods.Message.ScrollBar.VScroll, wParam, 0);
                }
            }
        }

        private static int IndexOf(string item, string searchPattern)
        {
            var index = item.IndexOf(searchPattern, StringComparison.InvariantCultureIgnoreCase);
            if (index == -1)
            {
                var camelHumps = GetCamelHumps(item);
                index = camelHumps.IndexOf(searchPattern, StringComparison.InvariantCultureIgnoreCase);
            }

            return index;
        }

        private static string GetCamelHumps(string source)
        {
            var stringBuilder = new StringBuilder();

            for (var i = 0; i < source.Length; ++i)
            {
                var c = source[i];

                if (i == 0)
                    stringBuilder.Append(c);
                else
                {
                    if (char.GetUnicodeCategory(c) == UnicodeCategory.UppercaseLetter)
                        stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString();
        }

        private void FindNext(int startIndex)
        {
            var items = ListBox.Items.Cast<ListBoxItem<IObjectName>>().ToList();
            var index = LinearSearch.IndexOf(startIndex, items.Count - 1, currentIndex =>
            {
                var item = items[currentIndex];
                var name = item.Item.UnquotedName;
                return name.IndexOf(_prefix) >= 0;
            });
            if (index >= 0)
                ListBox.SelectedIndex = index;
        }

        private void FindPrevious(int startIndex)
        {
            var items = ListBox.Items.Cast<ListBoxItem<IObjectName>>().ToList();
            var index = LinearSearch.LastIndexOf(startIndex, items.Count - 1, currentIndex =>
            {
                var item = items[currentIndex];
                var name = item.Item.UnquotedName;
                return name.IndexOf(_prefix) >= 0;
            });
            if (index >= 0)
                ListBox.SelectedIndex = index;
        }

        public bool HandleKeyPress(KeyPressEventArgs e)
        {
            var handled = false;

            if (e.KeyChar == '\r' || e.KeyChar == '\n')
            {
                // Enter
                handled = true;
                SelectItem();
                Close();
            }
            else if (e.KeyChar == '\x1B')
            {
                // Escape
                handled = true;
                Close();
            }
            else
            {
                if (e.KeyChar == '\x08')
                {
                    // Backspace
                    var length = _prefix.Length;
                    if (length > 0)
                        _prefix = _prefix.Substring(0, length - 1);
                }
                else
                    _prefix += char.ToLower(e.KeyChar);

                LoadItems();
                Find(_prefix, 0);
            }

            return handled;
        }
    }
}