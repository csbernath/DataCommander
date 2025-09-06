using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Windows.Forms;
using Foundation.Assertions;
using Foundation.Log;

namespace Foundation.Windows.Forms;

public class FoundationMessageBox : IMessageBox
{
    private static ILog Log = LogFactory.Instance.GetCurrentTypeLog();

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
        
        var form = new FoundationMessageBoxForm(messageBoxButtons)
        {
            FormBorderStyle = FormBorderStyle.FixedDialog,
#pragma warning disable WFO5001
            FormCornerPreference = FormCornerPreference.DoNotRound,
#pragma warning restore WFO5001
            KeyPreview = true,            
            MaximizeBox = false,
            MinimizeBox = false,
            Text = caption,
            ShowInTaskbar = false,
            StartPosition = FormStartPosition.CenterParent            
        };

        if (owner is Form ownerForm)
            form.Owner = ownerForm;

        const int borderY = 24;
        const int paddingY = 28;
        
        var height = 0;

        PictureBox? pictureBox = null;
        if (messageBoxIcon != MessageBoxIcon.None)
        {
            pictureBox = CreatePictureBox(messageBoxIcon);
            pictureBox.Location = new Point(25, borderY);
            form.Controls.Add(pictureBox);
        }

        var textLabel = CreateTextLabel(text);
        form.Controls.Add(textLabel);
 
        if (messageBoxIcon == MessageBoxIcon.None)
        {
            textLabel.Location = new Point(10, borderY);
            height += paddingY + textLabel.Height;
            height = Math.Max(height, 43);
        }
        else
        {
            var textLabelTop = textLabel.Height < pictureBox!.Height
                ? borderY + (pictureBox.Height - textLabel.Height) / 2
                : borderY;
            textLabel.Location = new Point(pictureBox.Right + 5, textLabelTop);
            height += paddingY + textLabel.Height;
            height = Math.Max(height, 64);
        }

        var iconAndTextWidth = textLabel.Right + 25;

        var bottomPanel = new Panel
        {
            BackColor = SystemColors.ControlLight,
            Dock = DockStyle.Bottom,
            Height = 49
        };
        form.Controls.Add(bottomPanel);
        
        var buttons = CreateButtons(messageBoxButtons);
        const int buttonBorderX = 19;
        const int buttonPaddingX = 9;
        var buttonsWidth = 2 * buttonBorderX + buttons.Sum(c => c.Width) + (buttons.Count - 1) * buttonPaddingX;
        var width = Math.Max(iconAndTextWidth, buttonsWidth);
        width = Math.Max(width, 333);
        
        SetButtonLocation(width, buttonsWidth, buttonBorderX, buttons, bottomPanel, buttonPaddingX);
        SetFormAcceptButton(defaultButton, form, buttons);
        SetFormCancelButton(messageBoxButtons, form, buttons);

        height += bottomPanel.Height + borderY;
        form.ClientSize = new Size(width, height);

        AddFormEventHandlers(form, text, caption, messageBoxButtons);

        SystemSounds.Beep.Play();
        return form.ShowDialog();
    }

    private static void SetButtonLocation(int width, int buttonsWidth, int buttonBorderX, List<Button> buttons, Panel bottomPanel, int buttonPaddingX)
    {
        var left = width - buttonsWidth + buttonBorderX;
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
        var clipboardButtonsText = messageBoxButtons switch
        {
            MessageBoxButtons.OK => "OK   ",
            MessageBoxButtons.OKCancel => "OK   Cancel   ",
            MessageBoxButtons.AbortRetryIgnore => "Abort   Retry   Ignore   ",
            MessageBoxButtons.YesNoCancel => "Yes   No   Cancel   ",
            MessageBoxButtons.YesNo => "Yes   No   ",
            MessageBoxButtons.RetryCancel => "Retry   Cancel   ",
            MessageBoxButtons.CancelTryContinue => "Cancel   TryContinue   ",
            _ => throw new ArgumentOutOfRangeException(nameof(messageBoxButtons), messageBoxButtons, null)
        };
        return clipboardButtonsText;
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
            Font = font,
            Text = text,
            AutoSize = true,
            MaximumSize = new Size(434, screenHeight - 300),
        };
        return textLabel;
    }

    private static List<Button> CreateButtons(MessageBoxButtons messageBoxButtons)
    {
        var buttons = new List<Button>();
        switch (messageBoxButtons)
        {
            case MessageBoxButtons.OK:
                buttons.Add(CreateOkButton());
                break;
            case MessageBoxButtons.OKCancel:
                buttons.AddRange([
                    CreateOkButton(),
                    CreateCancelButton()
                ]);
                break;
            case MessageBoxButtons.AbortRetryIgnore:
                buttons.AddRange([
                    CreateAbortButton(),
                    CreateRetryButton(),
                    CreateIgnoreButton()
                ]);
                break;
            case MessageBoxButtons.YesNoCancel:
                buttons.AddRange([
                    CreateYesButton(),
                    CreateNoButton(),
                    CreateCancelButton()
                ]);
                break;
            case MessageBoxButtons.YesNo:
                buttons.AddRange([
                    CreateYesButton(),
                    CreateNoButton()
                ]);
                break;
            case MessageBoxButtons.RetryCancel:
                buttons.AddRange([
                    CreateRetryButton(),
                    CreateCancelButton()
                ]);
                break;
            case MessageBoxButtons.CancelTryContinue:
                buttons.AddRange([
                    CreateCancelButton(),
                    CreateTryAgainButton(),
                    CreateContinueButton()
                ]);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(messageBoxButtons), messageBoxButtons, null);
        }

        for (var index = 0; index < buttons.Count; index++)
            buttons[index].TabIndex = index;

        return buttons;
    }

    private static Button CreateButton(string text, DialogResult dialogResult) =>
        new()
        {
            Size = new Size(88, 28),
            Text = text,
            DialogResult = dialogResult
        };

    private static Button CreateAbortButton()
    {
        var button = CreateButton("&Abort", DialogResult.Abort);
        button.UseMnemonic = true;
        return button;
    }

    private static Button CreateCancelButton() => CreateButton("Cancel", DialogResult.Cancel);

    private static Button CreateContinueButton() => CreateButton("&Continue", DialogResult.Continue);    

    private static Button CreateIgnoreButton() =>
        new()
        {
            Size = new Size(90, 28),
            UseMnemonic = true,
            Text = "&Ignore",
            DialogResult = DialogResult.Retry
        };

    private static Button CreateNoButton()
    {
        var button = CreateButton("&No", DialogResult.No);
        button.UseMnemonic = true;
        return button;
    }
 
    private static Button CreateOkButton() => CreateButton("OK", DialogResult.OK);

    private static Button CreateRetryButton()
    {
        var button = CreateButton("&Retry", DialogResult.Retry);
        button.UseMnemonic = true;
        return button;
    }

    private static Button CreateTryAgainButton() => CreateButton("&Try Again", DialogResult.TryAgain);    

    private static Button CreateYesButton()
    {
        var button = CreateButton("&Yes", DialogResult.Yes);
        button.UseMnemonic = true;
        return button;
    }

    private static void SetFormAcceptButton(MessageBoxDefaultButton messageBoxDefaultButton, Form form, List<Button> buttonControls)
    {
        var defaultButtonIndex = messageBoxDefaultButton switch
        {
            MessageBoxDefaultButton.Button1 => 0,
            MessageBoxDefaultButton.Button2 => 1,
            MessageBoxDefaultButton.Button3 => 2,
            MessageBoxDefaultButton.Button4 => 3,
            _ => throw new ArgumentOutOfRangeException(nameof(messageBoxDefaultButton), messageBoxDefaultButton, null)
        };
        form.AcceptButton = buttonControls[defaultButtonIndex];
        buttonControls[defaultButtonIndex].Select();
    }

    private static void SetFormCancelButton(MessageBoxButtons messageBoxButtons, Form form, List<Button> buttons)
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
}