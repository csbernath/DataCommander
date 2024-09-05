using System.Windows.Forms;

namespace DataCommander.Application;

internal interface IKeyboardHandler
{
    bool HandleKeyDown(KeyEventArgs e);
    bool HandleKeyPress(KeyPressEventArgs e);
}