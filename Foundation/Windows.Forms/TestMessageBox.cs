using System;
using System.Windows.Forms;
using Foundation.Assertions;
using Foundation.Windows.Forms;

public class TestMessageBox : IMessageBox
{
    private readonly FoundationMessageBox _foundationMessageBox = new  FoundationMessageBox();

    public DialogResult Show(string? text)
    {
        var dialogResult0 = MessageBox.Show(text);
        var dialogResult1 = _foundationMessageBox.Show(text);
        Assert.CompareToEquals(dialogResult0, dialogResult1);
        return dialogResult1;
    }

    public DialogResult Show(IWin32Window? owner, string? text, string? caption) => throw new NotImplementedException();

    public DialogResult Show(IWin32Window? owner, string? text, string? caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
    {
        var dialogResult0 = MessageBox.Show(owner, text, caption, buttons, icon, defaultButton);
        var dialogResult1 = _foundationMessageBox.Show(owner, text, caption, buttons, icon, defaultButton);
        Assert.CompareToEquals(dialogResult0, dialogResult1);
        return dialogResult1;
    }

    public DialogResult Show(string? text, string? caption, MessageBoxButtons buttons, MessageBoxIcon icon) => throw new NotImplementedException();

    public DialogResult Show(IWin32Window? owner, string? text, string? caption, MessageBoxButtons buttons, MessageBoxIcon icon) => throw new NotImplementedException();

    public DialogResult Show(IWin32Window? owner, string? text)
    {
        var dialogResult0 = MessageBox.Show(owner, text);
        var dialogResult1 = _foundationMessageBox.Show(owner, text);
        Assert.CompareToEquals(dialogResult0, dialogResult1);
        return dialogResult1;
    }
}