using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Windows.Forms;

namespace Foundation.Windows.Forms;

internal class FoundationMessageBoxForm : Form
{    
    private const int CP_NOCLOSE_BUTTON = 0x200;

    private readonly MessageBoxButtons _messageBoxButtons;

    public FoundationMessageBoxForm(
        IWin32Window? owner,
        string? text,
        string? caption,
        MessageBoxButtons messageBoxButtons,
        MessageBoxIcon messageBoxIcon,
        MessageBoxDefaultButton defaultButton,
        MessageBoxOptions options,
        bool showHelp)
    {
        _messageBoxButtons = messageBoxButtons;
        
        caption = caption ?? "Error";
        var startPosition = owner != null
            ? FormStartPosition.CenterParent
            : FormStartPosition.CenterScreen;
        
        SuspendLayout();
        AutoScaleDimensions = new SizeF(6F, 13F);        
        //AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        
        Font = new Font("Segoe UI", 8);

        FormBorderStyle = FormBorderStyle.FixedDialog;
// #pragma warning disable WFO5001
//         FormCornerPreference = FormCornerPreference.DoNotRound;
// #pragma warning restore WFO5001
        KeyPreview = true;
        MaximizeBox = false;
        MinimizeBox = false;
        Text = caption;
        ShowInTaskbar = false;
        StartPosition = startPosition;

        if (owner is Form ownerForm)
            Owner = ownerForm;  
        
        var stringSize = CreateGraphics().MeasureString(caption, Font);
        var captionWidth = (int)stringSize.Width + 74; 
        captionWidth = Math.Min(captionWidth, 411);

        var borderX = 21;
        var borderY = 23;
        PictureBox? pictureBox = null;
        if (messageBoxIcon != MessageBoxIcon.None)
        {
            pictureBox = CreatePictureBox(messageBoxIcon);
            pictureBox.SuspendLayout();
            pictureBox.Location = new Point(borderX, borderY);
            Controls.Add(pictureBox);
        }

        Label? textLabel = null;
        if (text != null)
        {
            textLabel = CreateTextLabel(text);
            Controls.Add(textLabel);
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
                textLabel.Location = new Point(pictureBox.Right + 6, textLabelTop);
            }
        }

        var iconAndTextHeight = 2 * borderY + Math.Max(pictureBox?.Height ?? 0, textLabel?.Height ?? 0);

        var iconAndTextWidth = (textLabel != null ? textLabel.Right : 0) + 26;
        iconAndTextWidth = Math.Max(iconAndTextWidth, 138);

        var bottomPanel = new Panel
        {
            BackColor = SystemColors.ControlLight,
            Dock = DockStyle.Bottom,
            Height = 42
        };
        Controls.Add(bottomPanel);

        var buttonIds = MessageBoxBuilder.GetButtonIds(messageBoxButtons);
        var buttons = CreateButtons(buttonIds);
        foreach (var button in buttons)
            bottomPanel.Controls.Add(button);
        
        const int buttonLeftBorderX = 32;        
        const int buttonRightBorderX = 19;
        const int buttonPaddingX = 10;
        var buttonsWidth = buttonLeftBorderX + buttons.Sum(c => c.Width) + (buttons.Length - 1) * buttonPaddingX + buttonRightBorderX;
        var width = Math.Max(iconAndTextWidth, buttonsWidth);
        width = Math.Max(width, captionWidth);

        AddButtonsToBottomPanel(buttons, bottomPanel, width, buttonsWidth, buttonLeftBorderX, buttonPaddingX);
        SetFormAcceptButton(defaultButton, buttons);
        SetFormCancelButton(messageBoxButtons, buttons);

        var height = iconAndTextHeight + bottomPanel.Height;
        ClientSize = new Size(width, height);

        if (pictureBox != null)
            pictureBox.ResumeLayout(false);

        ResumeLayout(false);
        PerformLayout();
        
        AddFormEventHandlers(text, caption, messageBoxButtons);        
    }

    protected override CreateParams CreateParams
    {
        get
        {
            var myCp = base.CreateParams;
    
            if (_messageBoxButtons != MessageBoxButtons.CancelTryContinue &&
                _messageBoxButtons != MessageBoxButtons.OK &&
                _messageBoxButtons != MessageBoxButtons.OKCancel &&
                _messageBoxButtons != MessageBoxButtons.RetryCancel &&
                _messageBoxButtons != MessageBoxButtons.YesNoCancel)
                myCp.ClassStyle |= CP_NOCLOSE_BUTTON;
    
            return myCp;
        }
    }
    
    private const int WM_UPDATEUISTATE = 0x0128;
    private const int UISF_HIDEACCEL = 0x2;
    private const int UIS_CLEAR = 0x2;    

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == WM_UPDATEUISTATE)
            m.WParam = (UISF_HIDEACCEL & 0x0000FFFF) | (UIS_CLEAR << 16);        
        base.WndProc(ref m);
    }

    private static void AddButtonsToBottomPanel(Button[] buttons, Panel bottomPanel, int width, int buttonsWidth, int buttonLeftBorderX, int buttonPaddingX)
    {
        var left = width - buttonsWidth + buttonLeftBorderX;
        foreach (var button in buttons)
        {
            button.Location = new Point(left, 10);
            left += button.Width + buttonPaddingX;
        }
    }

    private void AddFormEventHandlers(string? text, string? caption, MessageBoxButtons messageBoxButtons)
    {
        KeyDown += (_, e) =>
        {
            if (e is { Control: true, KeyCode: Keys.C })
            {
                SystemSounds.Beep.Play();
                var clipboardText = MessageBoxBuilder.GetClipboardText(text, caption, messageBoxButtons);
                Clipboard.SetText(clipboardText);
                e.Handled = true;
            }
            else if (messageBoxButtons == MessageBoxButtons.OK && e.KeyCode == Keys.Escape)
            {
                DialogResult = DialogResult.OK;
                e.Handled = true;
            }
        };

        FormClosed += (_, _) =>
        {
            if (messageBoxButtons == MessageBoxButtons.OK)
                DialogResult = DialogResult.OK;
        };
    }

    private static PictureBox CreatePictureBox(MessageBoxIcon messageBoxIcon)
    {
        var icon = MessageBoxBuilder.GetIcon(messageBoxIcon);
        var pictureBox = new PictureBox();
        pictureBox.Image = icon.ToBitmap();
        pictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
        return pictureBox;
    }

    private static Label CreateTextLabel(string? text)
    {
        var screen = Screen.PrimaryScreen;
        var screenHeight = screen != null ? screen.Bounds.Height : 768;
        var textLabel = new Label
        {
            AutoSize = true,           

            MaximumSize = new Size(326, screenHeight - 300),            
            Text = text,
        };
        return textLabel;
    }

    private static Button[] CreateButtons(MessageBoxButtonId[] buttonIds)
    {
        var buttons = buttonIds.Select(CreateButton).ToArray();
        for (var index = 0; index < buttons.Length; ++index)
            buttons[index].TabIndex = index;

        return buttons;
    }

    private static Button CreateButton(MessageBoxButtonId buttonId)
    {
        var buttonInfo = MessageBoxBuilder.ButtonInfosById[buttonId];
        return new Button
        {
            DialogResult = buttonInfo.DialogResult,
            Size = new Size(75, 24),            
            Text = buttonInfo.TextWithMnemonic,
            UseMnemonic = buttonInfo.UseMnemonic,
            UseVisualStyleBackColor = true
        };
    }

    private void SetFormAcceptButton(MessageBoxDefaultButton messageBoxDefaultButton, Button[] buttonControls)
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
        
        AcceptButton = buttonControls[defaultButtonIndex];
        buttonControls[defaultButtonIndex].Select();
    }

    private void SetFormCancelButton(MessageBoxButtons messageBoxButtons, Button[] buttons)
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
            CancelButton = buttons[cancelButtonIndex.Value];
    }
}
