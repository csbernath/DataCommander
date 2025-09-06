using System.Windows.Forms;

namespace Foundation.Windows.Forms;

public interface IMessageBox
{
    public DialogResult Show(string? text);
    public DialogResult Show(IWin32Window? owner, string? text, string? caption);

    public DialogResult Show(IWin32Window? owner, string? text, string? caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton);

    public DialogResult Show(string? text, string? caption, MessageBoxButtons buttons, MessageBoxIcon icon);
    public DialogResult Show(IWin32Window? owner, string? text, string? caption, MessageBoxButtons buttons, MessageBoxIcon icon);
    public DialogResult Show(IWin32Window? owner, string? text);
}