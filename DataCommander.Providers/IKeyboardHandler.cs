namespace DataCommander
{
    using System.Windows.Forms;

    internal interface IKeyboardHandler
    {
        bool HandleKeyDown(KeyEventArgs e);

        bool HandleKeyPress(KeyPressEventArgs e);
    }
}