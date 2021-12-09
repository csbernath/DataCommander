using System.Windows.Forms;

namespace DataCommander.Providers;

internal interface IKeyboardHandler
{
    bool HandleKeyDown(KeyEventArgs e);
    bool HandleKeyPress(KeyPressEventArgs e);
}