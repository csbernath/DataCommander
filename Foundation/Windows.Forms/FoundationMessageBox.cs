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
    private static readonly Dictionary<ButtonId, ButtonInfo> ButtonInfosById = new ButtonInfo[]
    {
        new(ButtonId.Abort, "Abort", true, "&Abort", DialogResult.Abort),
        new(ButtonId.Cancel, "Cancel", false, "Cancel", DialogResult.Cancel),
        new(ButtonId.Continue, "Continue", true, "&Continue", DialogResult.Continue),
        new(ButtonId.Ignore, "Ignore", true, "&Ignore", DialogResult.Ignore),
        new(ButtonId.No, "No", true, "&No", DialogResult.No),
        new(ButtonId.Ok, "OK", false, "OK", DialogResult.OK),
        new(ButtonId.Retry, "Retry", true, "&Retry", DialogResult.Retry),
        new(ButtonId.TryAgain, "Try Again", true, "&Try Again", DialogResult.TryAgain),
        new(ButtonId.Yes, "Yes", true, "&Yes", DialogResult.Yes)
    }.ToDictionary(i => i.ButtonId);
    
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

        var form = CreateForm(owner, caption, messageBoxButtons);
        var captionWidth = form.CreateGraphics().MeasureString(caption, form.Font).Width;
        
        const int borderY = 26;
        PictureBox? pictureBox = null;
        if (messageBoxIcon != MessageBoxIcon.None)
        {
            pictureBox = CreatePictureBox(messageBoxIcon);
            pictureBox.Location = new Point(25, borderY);
            form.Controls.Add(pictureBox);
        }

        Label? textLabel = null;
        if (text != null)
        {
            textLabel = CreateTextLabel(text);
            form.Controls.Add(textLabel);
        }

        if (messageBoxIcon == MessageBoxIcon.None)
        {
            if (textLabel != null)
                textLabel.Location = new Point(9, borderY);
        }
        else
        {
            if (textLabel != null)
            {
                var textLabelTop = textLabel.Height < pictureBox!.Height
                    ? borderY + (pictureBox.Height - textLabel.Height) / 2
                    : borderY;
                textLabel.Location = new Point(pictureBox.Right + 5, textLabelTop);
            }
        }

        var iconAndTextHeight = 2 * borderY + Math.Max(pictureBox?.Height ?? 0, textLabel?.Height ?? 0);

        var iconAndTextWidth = (textLabel != null ? textLabel.Right : 0) + 34;
        iconAndTextWidth = Math.Max(iconAndTextWidth, 138);

        var bottomPanel = new Panel
        {
            BackColor = SystemColors.ControlLight,
            Dock = DockStyle.Bottom,
            Height = 50
        };
        form.Controls.Add(bottomPanel);

        var buttonIds = GetButtonIds(messageBoxButtons);
        var buttons = CreateButtons(buttonIds);
        const int buttonLeftBorderX = 32;        
        const int buttonRightBorderX = 19;
        const int buttonPaddingX = 10;
        var buttonsWidth = buttonLeftBorderX + buttons.Sum(c => c.Width) + (buttons.Length - 1) * buttonPaddingX + buttonRightBorderX;
        var width = Math.Max(iconAndTextWidth, buttonsWidth);
        width = Math.Max(width, captionWidth);

        AddButtonsToBottomPanel(buttons, bottomPanel, width, buttonsWidth, buttonLeftBorderX, buttonPaddingX);
        SetFormAcceptButton(defaultButton, form, buttons);
        SetFormCancelButton(messageBoxButtons, form, buttons);

        var height = iconAndTextHeight + bottomPanel.Height;
        form.ClientSize = new Size(width, height);

        AddFormEventHandlers(form, text, caption, messageBoxButtons);
        Beep(messageBoxIcon);
        
        return form.ShowDialog();
    }

    private static FoundationMessageBoxForm CreateForm(IWin32Window? owner, string? caption, MessageBoxButtons messageBoxButtons)
    {
        var text = caption ?? "Error";
        var startPosition = owner != null
            ? FormStartPosition.CenterParent
            : FormStartPosition.CenterScreen;
        var form = new FoundationMessageBoxForm(messageBoxButtons)
        {
            FormBorderStyle = FormBorderStyle.FixedDialog,
#pragma warning disable WFO5001
            FormCornerPreference = FormCornerPreference.DoNotRound,
#pragma warning restore WFO5001
            KeyPreview = true,
            MaximizeBox = false,
            MinimizeBox = false,
            Text = text,
            ShowInTaskbar = false,
            StartPosition = startPosition
        };
        
        

        if (owner is Form ownerForm)
            form.Owner = ownerForm;
        return form;
    }

    private static void AddButtonsToBottomPanel(Button[] buttons, Panel bottomPanel, int width, int buttonsWidth, int buttonLeftBorderX, int buttonPaddingX)
    {
        var left = width - buttonsWidth + buttonLeftBorderX;
        foreach (var button in buttons)
        {
            button.Location = new Point(left, 10);
            bottomPanel.Controls.Add(button);
            left += button.Width + buttonPaddingX;
        }
    }

    private static void AddFormEventHandlers(FoundationMessageBoxForm form, string? text, string? caption, MessageBoxButtons messageBoxButtons)
    {
        form.KeyDown += (_, e) =>
        {
            if (e is { Control: true, KeyCode: Keys.C })
            {
                SystemSounds.Beep.Play();
                var clipboardText = GetClipboardText(text, caption, messageBoxButtons);
                Clipboard.SetText(clipboardText);
                e.Handled = true;
            }
            else if (messageBoxButtons == MessageBoxButtons.OK && e.KeyCode == Keys.Escape)
            {
                form.DialogResult = DialogResult.OK;
                e.Handled = true;
            }
        };

        form.FormClosed += (_, _) =>
        {
            if (messageBoxButtons == MessageBoxButtons.OK)
                form.DialogResult = DialogResult.OK;
        };
    }

    private static string GetClipboardText(string? text, string? caption, MessageBoxButtons messageBoxButtons)
    {
        var separator = new string('-', 27);
        var buttonsText = GetClipboardButtonsText(messageBoxButtons);
        var clipboardText = $@"{separator}
{caption}
{separator}
{text}
{separator}
{buttonsText}
{separator}";
        return clipboardText;
    }

    private static string GetClipboardButtonsText(MessageBoxButtons messageBoxButtons)
    {
        var buttonIds = GetButtonIds(messageBoxButtons);
        var buttonInfos = buttonIds.Select(buttonId => ButtonInfosById[buttonId]);
        var buttonTexts = buttonInfos.Select(buttonInfo => buttonInfo.Text + "   ");
        var clipboardButtonTexts = string.Concat(buttonTexts);
        return clipboardButtonTexts;
    }

    private static PictureBox CreatePictureBox(MessageBoxIcon messageBoxIcon)
    {
        var stockIconId = messageBoxIcon switch
        {
            MessageBoxIcon.Asterisk => StockIconId.Info,
            MessageBoxIcon.Error => StockIconId.Error,
            MessageBoxIcon.Warning => StockIconId.Warning,
            MessageBoxIcon.Question => StockIconId.Help,
            _ => throw new ArgumentOutOfRangeException(nameof(messageBoxIcon), messageBoxIcon, null)
        };
        var icon = SystemIcons.GetStockIcon(stockIconId);
        var pictureBox = new PictureBox();
        pictureBox.Image = icon.ToBitmap();
        pictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
        return pictureBox;
    }

    private static Label CreateTextLabel(string? text)
    {
        var messageBoxFont = SystemFonts.MessageBoxFont!;
        var font = new Font(messageBoxFont.Name, messageBoxFont.Size - 1, FontStyle.Regular);
        var screen = Screen.PrimaryScreen;
        var screenHeight = screen != null ? screen.Bounds.Height : 768;
        var textLabel = new Label
        {
            AutoSize = true,            
            Font = font,
            MaximumSize = new Size(382, screenHeight - 300),            
            Text = text,
        };
        return textLabel;
    }

    private static Button[] CreateButtons(ButtonId[] buttonIds)
    {
        var buttons = buttonIds.Select(CreateButton).ToArray();
        for (var index = 0; index < buttons.Length; ++index)
            buttons[index].TabIndex = index;

        return buttons;
    }

    private static ButtonId[] GetButtonIds(MessageBoxButtons messageBoxButtons)
    {
        var buttonIds = messageBoxButtons switch
        {
            MessageBoxButtons.OK => new[] { ButtonId.Ok },
            MessageBoxButtons.OKCancel => [ButtonId.Ok, ButtonId.Cancel],
            MessageBoxButtons.AbortRetryIgnore => [ButtonId.Abort, ButtonId.Retry, ButtonId.Ignore],
            MessageBoxButtons.YesNoCancel => [ButtonId.Yes, ButtonId.No, ButtonId.Cancel],
            MessageBoxButtons.YesNo => [ButtonId.Yes, ButtonId.No],
            MessageBoxButtons.RetryCancel => [ButtonId.Retry, ButtonId.Cancel],
            MessageBoxButtons.CancelTryContinue => [ButtonId.Cancel, ButtonId.TryAgain, ButtonId.Continue],
            _ => throw new ArgumentOutOfRangeException(nameof(messageBoxButtons), messageBoxButtons, null)
        };
        return buttonIds;
    }

    private static Button CreateButton(ButtonId buttonId)
    {
        var buttonInfo = ButtonInfosById[buttonId];
        return new Button
        {
            DialogResult = buttonInfo.DialogResult,
            Size = new Size(88, 28),            
            Text = buttonInfo.TextWithMnemonic,
            UseMnemonic = buttonInfo.UseMnemonic
        };
    }

    private static void SetFormAcceptButton(MessageBoxDefaultButton messageBoxDefaultButton, Form form, Button[] buttonControls)
    {
        var defaultButtonIndex = messageBoxDefaultButton switch
        {
            MessageBoxDefaultButton.Button1 => 0,
            MessageBoxDefaultButton.Button2 => 1,
            MessageBoxDefaultButton.Button3 => 2,
            MessageBoxDefaultButton.Button4 => 3,
            _ => throw new ArgumentOutOfRangeException(nameof(messageBoxDefaultButton), messageBoxDefaultButton, null)
        };

        if (defaultButtonIndex > buttonControls.Length - 1)
            defaultButtonIndex = 0;
        
        form.AcceptButton = buttonControls[defaultButtonIndex];
        buttonControls[defaultButtonIndex].Select();
    }

    private static void SetFormCancelButton(MessageBoxButtons messageBoxButtons, Form form, Button[] buttons)
    {
        int? cancelButtonIndex = messageBoxButtons switch
        {
            MessageBoxButtons.CancelTryContinue => 0,
            MessageBoxButtons.OKCancel => 1,
            MessageBoxButtons.RetryCancel => 1,
            MessageBoxButtons.YesNoCancel => 2,
            _ => null
        };
        if (cancelButtonIndex != null)
            form.CancelButton = buttons[cancelButtonIndex.Value];
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

    private enum ButtonId
    {
        Abort,
        Cancel,
        Continue,
        Ignore,
        No,
        Ok,
        Retry,
        TryAgain,
        Yes
    }

    private class ButtonInfo(ButtonId buttonId, string text, bool useMnemonic, string textWithMnemonic, DialogResult dialogResult)
    {
        public readonly ButtonId ButtonId = buttonId;
        public readonly string Text = text;
        public readonly bool UseMnemonic = useMnemonic;
        public readonly string TextWithMnemonic = textWithMnemonic;
        public readonly DialogResult DialogResult = dialogResult;
    }
}