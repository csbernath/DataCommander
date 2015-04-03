namespace DataCommander.Providers
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using DataCommander.Foundation.Linq;

    /// <summary>
    /// Summary description for MemberListBox.
    /// </summary>
    internal sealed class MemberListBox : UserControl, IKeyboardHandler
    {
        private readonly CompletionForm completionForm;
        private readonly QueryTextBox textBox;
        private GetCompletionResponse response;
        private string prefix = string.Empty;
        private ListBox listBox;

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

        public void Initialize(GetCompletionResponse response)
        {
            this.response = response;

            foreach (var item in response.Items)
            {
                var listBoxItem = new ListBoxItem<IObjectName>(item, ToString);
                this.ListBox.Items.Add(listBoxItem);
            }

            this.textBox.KeyboardHandler = this;

            if (response.Items.Count > 0)
            {
                this.prefix = this.textBox.Text.Substring(response.StartPosition, response.Length);
                if (this.prefix.Length > 0)
                {
                    string[] items = this.prefix.Split('.');

                    int count = response.Items[0].UnquotedName.Count(c => c == '.') + 1;
                    var sb = new StringBuilder();
                    for (int i = Math.Max(items.Length - count, 0); i < items.Length; i++)
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
            this.listBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // listBox
            // 
            this.listBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte) (238)));
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(180, 134);
            this.listBox.TabIndex = 0;
            this.listBox.DoubleClick += new System.EventHandler(this.listBox_DoubleClick);
            // 
            // MemberListBox
            // 
            this.Controls.AddRange(new System.Windows.Forms.Control[]
            {
                this.listBox
            });
            this.Name = "MemberListBox";
            this.Size = new System.Drawing.Size(180, 142);
            this.ResumeLayout(false);

        }

        #endregion

        private void Close()
        {
            this.textBox.KeyboardHandler = null;
            Form form = this.Parent as Form;
            form.Controls.Remove(this);
            form.Close();
        }

        private void SelectItem()
        {
            var listBoxItem = (ListBoxItem<IObjectName>) this.listBox.SelectedItem;

            if (listBoxItem != null)
            {
                var selectedItem = listBoxItem.Item.UnquotedName;

                int startIndex = this.response.StartPosition;
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

                string originalText = this.textBox.Text.Substring(startIndex, length);
                string[] originalItems = originalText.Split('.');
                string[] newItems = selectedItem.Split('.');
                var sb = new StringBuilder();
                for (int i = 0; i < originalItems.Length - newItems.Length; i++)
                {
                    if (sb.Length > 0)
                    {
                        sb.Append('.');
                    }
                    sb.Append(originalItems[i]);
                }
                for (int i = 0; i < newItems.Length; i++)
                {
                    if (sb.Length > 0)
                    {
                        sb.Append('.');
                    }
                    sb.Append(newItems[i]);
                }
                string newText = sb.ToString();

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
            get
            {
                return this.listBox;
            }
        }

        public bool HandleKeyDown(KeyEventArgs e)
        {
            bool handled;
            int hWnd = this.listBox.Handle.ToInt32();
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
                    int startIndex = this.listBox.SelectedIndex + 1;
                    if (startIndex < this.listBox.Items.Count - 1)
                    {
                        this.FindNext(startIndex);
                    }
                }
                else if (e.KeyCode == Keys.Up && e.Shift)
                {
                    int startIndex = this.listBox.SelectedIndex - 1;
                    if (startIndex > 0)
                    {
                        this.FindPrevious(startIndex);
                    }
                }
            }
            else if (e.KeyCode.In(Keys.Subtract, Keys.OemMinus) && e.Control)
            {
                handled = true;
                var filteredItems = this.listBox.Items.Cast<ListBoxItem<IObjectName>>().Where(item => item.Item.UnquotedName.IndexOf(this.prefix, StringComparison.InvariantCultureIgnoreCase) >= 0).ToArray();
                this.listBox.Items.Clear();
                this.listBox.Items.AddRange(filteredItems);
            }
            else if (e.KeyCode == Keys.Enter)
            {
                handled = true;
            }
            else
            {
                handled = false;

                //        if (e.KeyCode == Keys.Delete)
                //        {
                //          int i = 0; // TODO
                //        }
            }

            return handled;
        }

        private void Find(string prefix, int startIndex)
        {
            int index = -1;
            // bool found = false;
            var filteredItems =
                this.listBox.Items.Cast<ListBoxItem<IObjectName>>()
                    .Select((listBoxItem, i) => Tuple.Create(i, listBoxItem.Item.UnquotedName.IndexOf(prefix, StringComparison.InvariantCultureIgnoreCase)))
                    .Where(item => item.Item2 >= 0).ToArray();

            if (filteredItems.Length > 0)
            {
                var item = filteredItems.MinMax(null, i => i.Item2).Min;
                index = item.Value.Item1;
            }

            //for (int i = startIndex; i < listBox.Items.Count; i++)
            //{
            //    string item = listBox.Items[i].ToString();
            //    string item2 = item.ToLower();
            //    int j = item2.IndexOf(prefix);

            //    if (j == 0)
            //    {
            //        index = i;
            //        break;
            //    }
            //    else if (j > 0 && !found)
            //    {
            //        index = i;
            //        found = true;
            //    }
            //}

            if (index >= 0)
            {
                this.listBox.SelectedIndex = index;

                if (index >= 3)
                {
                    // scrolling 3 items up
                    int wParam = (int) NativeMethods.Message.ScrollBarParameter.ThumbPosition;
                    int pos = (index - 3) << 16;
                    wParam += pos;
                    int hWnd = this.listBox.Handle.ToInt32();
                    NativeMethods.SendMessage(hWnd, (int) NativeMethods.Message.ScrollBar.VScroll, wParam, 0);
                }
            }
        }

        private void FindNext(int startIndex)
        {
            var items = this.listBox.Items.Cast<string>();
            int index = items.IndexOf(startIndex, item => item.IndexOf(this.prefix, StringComparison.InvariantCultureIgnoreCase) >= 0);
            if (index >= 0)
            {
                this.listBox.SelectedIndex = index;
            }
        }

        private void FindPrevious(int startIndex)
        {
            var items = this.listBox.Items.Cast<string>();
            int index = items.LastIndexOf(startIndex, item => item.StartsWith(this.prefix, StringComparison.InvariantCultureIgnoreCase));
            if (index == -1)
            {
                index = items.LastIndexOf(startIndex, item => item.IndexOf(this.prefix, StringComparison.InvariantCultureIgnoreCase) >= 0);
            }
            if (index >= 0)
            {
                this.listBox.SelectedIndex = index;
            }
        }

        public bool HandleKeyPress(KeyPressEventArgs e)
        {
            bool handled = false;

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
                    int length = this.prefix.Length;

                    if (length > 0)
                    {
                        this.prefix = this.prefix.Substring(0, length - 1);
                    }
                }
                else
                {
                    this.prefix += char.ToLower(e.KeyChar);
                }

                this.Find(this.prefix, 0);
            }

            return handled;
        }
    }
}