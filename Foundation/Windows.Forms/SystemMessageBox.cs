using System.Windows.Forms;

namespace Foundation.Windows.Forms;

public class SystemMessageBox : IMessageBox
{
    public DialogResult Show(string? text) => MessageBox.Show(text);
    public DialogResult Show(IWin32Window? owner, string? text, string? caption) => MessageBox.Show(owner, text, caption);

    public DialogResult Show(IWin32Window? owner, string? text, string? caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton) => MessageBox.Show(owner, text, caption, buttons, icon, defaultButton);

    public DialogResult Show(string? text, string? caption, MessageBoxButtons buttons, MessageBoxIcon icon) => MessageBox.Show(text, caption, buttons, icon);

    public DialogResult Show(IWin32Window? owner, string? text, string? caption, MessageBoxButtons buttons, MessageBoxIcon icon) =>
        MessageBox.Show(owner, text, caption, buttons, icon);

    public DialogResult Show(IWin32Window? owner, string? text) => MessageBox.Show(owner, text);
}