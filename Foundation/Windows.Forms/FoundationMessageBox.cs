using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Windows.Forms;
using Foundation.Log;

namespace Foundation.Windows.Forms;

public class FoundationMessageBox : IMessageBox
{
    private static readonly ILog Log = LogFactory.Instance.GetCurrentTypeLog();

    public DialogResult Show(string? text) =>
        ShowCore(null, text, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0, false);

    public DialogResult Show(IWin32Window? owner, string? text, string? caption) => ShowCore(owner, text, caption, MessageBoxButtons.OK,
        MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0, false);

    public DialogResult Show(
        IWin32Window? owner,
        string? text,
        string? caption,
        MessageBoxButtons buttons,
        MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton) =>
        ShowCore(owner, text, caption, buttons, icon, defaultButton, 0, false);

    public DialogResult Show(
        string? text,
        string? caption,
        MessageBoxButtons buttons,
        MessageBoxIcon icon) =>
        ShowCore(null, text, caption, buttons, icon, MessageBoxDefaultButton.Button1, 0, false);

    public DialogResult Show(
        IWin32Window? owner,
        string? text,
        string? caption,
        MessageBoxButtons buttons,
        MessageBoxIcon icon) =>
        ShowCore(owner, text, caption, buttons, icon, MessageBoxDefaultButton.Button1, 0, false);

    public DialogResult Show(IWin32Window? owner, string? text) => ShowCore(owner, text, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.None,
        MessageBoxDefaultButton.Button1, 0, false);

    private static DialogResult ShowCore(
        IWin32Window? owner,
        string? text,
        string? caption,
        MessageBoxButtons messageBoxButtons,
        MessageBoxIcon messageBoxIcon,
        MessageBoxDefaultButton defaultButton,
        MessageBoxOptions options,
        bool showHelp)
    {
        Log.Trace(CallerInformation.Create(), $"Caption: {caption}, Text: {text}");

        Beep(messageBoxIcon);
        var foundationMessageBoxForm = new FoundationMessageBoxForm(owner, text, caption, messageBoxButtons, messageBoxIcon, defaultButton, options, showHelp);
        return foundationMessageBoxForm.ShowDialog();
    }

    private static void Beep(MessageBoxIcon messageBoxIcon)
    {
        switch (messageBoxIcon)
        {
            case MessageBoxIcon.Hand:
                SystemSounds.Hand.Play();
                break;
            case MessageBoxIcon.Asterisk:
                SystemSounds.Asterisk.Play();
                break;
            case MessageBoxIcon.Exclamation:
                SystemSounds.Exclamation.Play();
                break;
        }
    }
}