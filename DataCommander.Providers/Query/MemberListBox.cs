namespace DataCommander.Providers.Query
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using Foundation.Collections;
    using Foundation.Linq;

    /// <summary>
    /// Summary description for MemberListBox.
    /// </summary>
    internal sealed class MemberListBox : UserControl, IKeyboardHandler
    {
        private readonly CompletionForm completionForm;
        private readonly QueryTextBox textBox;
        private GetCompletionResponse response;
        private string prefix = string.Empty;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

        public MemberListBox(CompletionForm completionForm, QueryTextBox textBox)
        {
            // This call is required by the Windows.Forms Form Designer.
            this.InitializeComponent();

            // TODO: Add any initialization after the InitForm call
            this.completionForm = completionForm;
            this.textBox = textBox;
        }

        private static string ToString(IObjectName objectName)
        {
            return objectName.UnquotedName;
        }

        private void LoadItems()
        {
            this.ListBox.Items.Clear();

            foreach (var item in this.response.Items)
            {
                var listBoxItem = new ListBoxItem<IObjectName>(item, ToString);
                this.ListBox.Items.Add(listBoxItem);
            }
        }

        public void Initialize(GetCompletionResponse response)
        {
            this.response = response;

            this.LoadItems();

            this.textBox.KeyboardHandler = this;

            if (response.Items.Count > 0)
            {
                this.prefix = this.textBox.Text.Substring(response.StartPosition, response.Length);
                if (this.prefix.Length > 0)
                {
                    var items = this.prefix.Split('.');

                    var count = response.Items[0].UnquotedName.Count(c => c == '.') + 1;
                    var sb = new StringBuilder();
                    for (var i = Math.Max(items.Length - count, 0); i < items.Length; i++)
                    {
                        if (sb.Length > 0)
                        {
                            sb.Append('.');
                        }
                        sb.Append(items[i]);
                    }

                    this.prefix = sb.ToString().ToLower();
                    this.Find(this.prefix, 0);
                }
            }
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                if (this.components != null)
                    this.components.Dispose();

            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ListBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // listBox
            // 
            this.ListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ListBox.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point,
                ((System.Byte) (238)));
            this.ListBox.Name = "ListBox";
            this.ListBox.Size = new System.Drawing.Size(180, 134);
            this.ListBox.TabIndex = 0;
            this.ListBox.DoubleClick += new System.EventHandler(this.listBox_DoubleClick);
            // 
            // MemberListBox
            // 
            this.Controls.AddRange(new System.Windows.Forms.Control[]
            {
                this.ListBox
            });
            this.Name = "MemberListBox";
            this.Size = new System.Drawing.Size(180, 142);
            this.ResumeLayout(false);

        }

        #endregion

        private void Close()
        {
            this.textBox.KeyboardHandler = null;
            var form = (Form) this.Parent;
            form.Controls.Remove(this);
            form.Close();
        }

        private void SelectItem()
        {
            var listBoxItem = (ListBoxItem<IObjectName>) this.ListBox.SelectedItem;

            if (listBoxItem != null)
            {
                var selectedItem = listBoxItem.Item.UnquotedName;

                var startIndex = this.response.StartPosition;
                var tokenIterator = new TokenIterator(this.textBox.Text.Substring(startIndex));
                var token = tokenIterator.Next();
                int length;
                if (token != null && token.StartPosition == 0)
                {
                    length = token.EndPosition - token.StartPosition + 1;
                }
                else
                {
                    length = 0;
                }

                var originalText = this.textBox.Text.Substring(startIndex, length);
                var originalItems = originalText.Split('.');
                var newItems = selectedItem.Split('.');
                var sb = new StringBuilder();
                for (var i = 0; i < originalItems.Length - newItems.Length; i++)
                {
                    if (sb.Length > 0)
                    {
                        sb.Append('.');
                    }
                    sb.Append(originalItems[i]);
                }
                for (var i = 0; i < newItems.Length; i++)
                {
                    if (sb.Length > 0)
                    {
                        sb.Append('.');
                    }
                    sb.Append(newItems[i]);
                }
                var newText = sb.ToString();

                // TODO
                this.completionForm.SelectItem(startIndex, length, listBoxItem.Item);

                //IntPtr intPtr = textBox.RichTextBox.Handle;
                //int hWnd = intPtr.ToInt32();
                //NativeMethods.SendMessage(hWnd, (int)NativeMethods.Message.Gdi.SetRedraw, 0, 0);

                //textBox.RichTextBox.SelectionStart = startIndex;
                //textBox.RichTextBox.SelectionLength = length;
                //textBox.RichTextBox.SelectedText = newText;
                //textBox.RichTextBox.SelectionStart = startIndex + newText.Length;

                //NativeMethods.SendMessage(hWnd, (int)NativeMethods.Message.Gdi.SetRedraw, 1, 0);
            }
        }

        private void listBox_DoubleClick(object sender, EventArgs e)
        {
            this.SelectItem();
            this.Close();
        }

        public ListBox ListBox
        {
            get;
            private set;
        }

        public bool HandleKeyDown(KeyEventArgs e)
        {
            bool handled;
            var hWnd = this.ListBox.Handle.ToInt32();
            NativeMethods.SendMessage(hWnd, (int) NativeMethods.Message.Keyboard.KeyDown, (int) e.KeyCode, 0);

            if (
                e.KeyCode == Keys.Down ||
                e.KeyCode == Keys.Up ||
                e.KeyCode == Keys.PageDown ||
                e.KeyCode == Keys.PageUp ||
                e.KeyCode == Keys.Home ||
                e.KeyCode == Keys.End)
            {
                handled = true;

                if (e.KeyCode == Keys.Down && e.Shift)
                {
                    var startIndex = this.ListBox.SelectedIndex + 1;
                    if (startIndex < this.ListBox.Items.Count - 1)
                        this.FindNext(startIndex);
                }
                else if (e.KeyCode == Keys.Up && e.Shift)
                {
                    var startIndex = this.ListBox.SelectedIndex - 1;
                    if (startIndex > 0)
                        this.FindPrevious(startIndex);
                }
            }
            else if (e.KeyCode.In(Keys.Subtract, Keys.OemMinus) && e.Control)
            {
                handled = true;
                var filteredItems = this.ListBox.Items.Cast<ListBoxItem<IObjectName>>()
                    .Where(item => IndexOf(item.Item.UnquotedName, this.prefix) >= 0)
                    .ToArray();
                this.ListBox.Items.Clear();
                this.ListBox.Items.AddRange(filteredItems);
            }
            else if (e.KeyCode == Keys.Enter)
            {
                handled = true;
            }
            else
            {
                handled = false;
            }

            return handled;
        }

        private void Find(string prefix, int startIndex)
        {
            var filteredItems = this.ListBox.Items.Cast<ListBoxItem<IObjectName>>()
                .Select((listBoxItem, i) => new
                {
                    Index = i,
                    IndexOf = IndexOf(listBoxItem.Item.UnquotedName, prefix)
                })
                .Where(item => item.IndexOf >= 0)
                .ToList();

            var index = -1;

            if (filteredItems.Count > 0)
            {
                index = filteredItems.MinIndexedItem(i => i.IndexOf).Value.Index;
            }

            if (index >= 0)
            {
                this.ListBox.SelectedIndex = index;

                if (index >= 3)
                {
                    // scrolling 3 items up
                    var wParam = (int) NativeMethods.Message.ScrollBarParameter.ThumbPosition;
                    var pos = (index - 3) << 16;
                    wParam += pos;
                    var hWnd = this.ListBox.Handle.ToInt32();
                    NativeMethods.SendMessage(hWnd, (int) NativeMethods.Message.ScrollBar.VScroll, wParam, 0);
                }
            }
        }

        private static int IndexOf(string item, string searchPattern)
        {
            var index = item.IndexOf(searchPattern, StringComparison.InvariantCultureIgnoreCase);
            if (index < 0)
            {
                var camelCase = GetCamelCase(item);
                index = camelCase.IndexOf(searchPattern, StringComparison.InvariantCultureIgnoreCase);
            }

            return index;
        }

        private static string GetCamelCase(string source)
        {
            var sb = new StringBuilder();

            for (var i = 0; i < source.Length; i++)
            {
                var c = source[i];

                if (i == 0)
                    sb.Append(c);
                else
                {
                    if (char.GetUnicodeCategory(c) == UnicodeCategory.UppercaseLetter)
                        sb.Append(c);
                }
            }

            return sb.ToString();
        }

        private void FindNext(int startIndex)
        {
            var items = this.ListBox.Items.Cast<ListBoxItem<IObjectName>>().ToList();

            var index = LinearSearch.IndexOf(startIndex, items.Count - 1, currentIndex =>
            {
                var item = items[currentIndex];
                var name = item.Item.UnquotedName;
                return name.IndexOf(this.prefix) >= 0;
            });

            if (index >= 0)
                this.ListBox.SelectedIndex = index;
        }

        private void FindPrevious(int startIndex)
        {
            var items = this.ListBox.Items.Cast<ListBoxItem<IObjectName>>().ToList();
            var index = LinearSearch.LastIndexOf(startIndex, items.Count - 1, currentIndex =>
            {
                var item = items[currentIndex];
                var name = item.Item.UnquotedName;
                return name.IndexOf(this.prefix) >= 0;
            });

            if (index >= 0)
                this.ListBox.SelectedIndex = index;
        }

        public bool HandleKeyPress(KeyPressEventArgs e)
        {
            var handled = false;

            if (e.KeyChar == '\r' || e.KeyChar == '\n')
            {
                // Enter
                handled = true;
                this.SelectItem();
                this.Close();
            }
            else if (e.KeyChar == '\x1B')
            {
                // Escape
                handled = true;
                this.Close();
            }
            else
            {
                if (e.KeyChar == '\x08')
                {
                    // Backspace
                    var length = this.prefix.Length;

                    if (length > 0)
                    {
                        this.prefix = this.prefix.Substring(0, length - 1);
                    }
                }
                else
                    this.prefix += char.ToLower(e.KeyChar);

                this.LoadItems();

                this.Find(this.prefix, 0);
            }

            return handled;
        }
    }
}