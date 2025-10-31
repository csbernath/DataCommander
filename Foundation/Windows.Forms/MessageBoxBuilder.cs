using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Windows.Forms;

namespace Foundation.Windows.Forms
{
    internal static class MessageBoxBuilder
    {        public static void Beep(MessageBoxIcon messageBoxIcon)
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

        public static readonly Dictionary<MessageBoxButtonId, ButtonInfo> ButtonInfosById = new ButtonInfo[]
        {
            new(MessageBoxButtonId.Abort, "Abort", true, "&Abort", DialogResult.Abort),
            new(MessageBoxButtonId.Cancel, "Cancel", false, "Cancel", DialogResult.Cancel),
            new(MessageBoxButtonId.Continue, "Continue", true, "&Continue", DialogResult.Continue),
            new(MessageBoxButtonId.Ignore, "Ignore", true, "&Ignore", DialogResult.Ignore),
            new(MessageBoxButtonId.No, "No", true, "&No", DialogResult.No),
            new(MessageBoxButtonId.Ok, "OK", false, "OK", DialogResult.OK),
            new(MessageBoxButtonId.Retry, "Retry", true, "&Retry", DialogResult.Retry),
            new(MessageBoxButtonId.TryAgain, "Try Again", true, "&Try Again", DialogResult.TryAgain),
            new(MessageBoxButtonId.Yes, "Yes", true, "&Yes", DialogResult.Yes)
        }.ToDictionary(i => i.ButtonId);

        public static Icon GetIcon(MessageBoxIcon messageBoxIcon)
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
            return icon;
        }

        public static MessageBoxButtonId[] GetButtonIds(MessageBoxButtons messageBoxButtons)
        {
            var buttonIds = messageBoxButtons switch
            {
                MessageBoxButtons.OK => new[] { MessageBoxButtonId.Ok },
                MessageBoxButtons.OKCancel => [MessageBoxButtonId.Ok, MessageBoxButtonId.Cancel],
                MessageBoxButtons.AbortRetryIgnore => [MessageBoxButtonId.Abort, MessageBoxButtonId.Retry, MessageBoxButtonId.Ignore],
                MessageBoxButtons.YesNoCancel => [MessageBoxButtonId.Yes, MessageBoxButtonId.No, MessageBoxButtonId.Cancel],
                MessageBoxButtons.YesNo => [MessageBoxButtonId.Yes, MessageBoxButtonId.No],
                MessageBoxButtons.RetryCancel => [MessageBoxButtonId.Retry, MessageBoxButtonId.Cancel],
                MessageBoxButtons.CancelTryContinue => [MessageBoxButtonId.Cancel, MessageBoxButtonId.TryAgain, MessageBoxButtonId.Continue],
                _ => throw new ArgumentOutOfRangeException(nameof(messageBoxButtons), messageBoxButtons, null)
            };
            return buttonIds;
        }

        public static string GetClipboardText(string? text, string? caption, MessageBoxButtons messageBoxButtons)
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

        public class ButtonInfo(MessageBoxButtonId buttonId, string text, bool useMnemonic, string textWithMnemonic, DialogResult dialogResult)
        {
            public readonly MessageBoxButtonId ButtonId = buttonId;
            public readonly string Text = text;
            public readonly bool UseMnemonic = useMnemonic;
            public readonly string TextWithMnemonic = textWithMnemonic;
            public readonly DialogResult DialogResult = dialogResult;
        }
    }
}