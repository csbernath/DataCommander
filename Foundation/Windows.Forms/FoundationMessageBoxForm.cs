using System.Windows.Forms;

namespace Foundation.Windows.Forms;

internal class FoundationMessageBoxForm : Form
{
    private const int CP_NOCLOSE_BUTTON = 0x200;

    private readonly MessageBoxButtons _messageBoxButtons;

    public FoundationMessageBoxForm(MessageBoxButtons messageBoxButtons)
    {
        _messageBoxButtons = messageBoxButtons;
    }

    protected override CreateParams CreateParams
    {
        get
        {
            var myCp = base.CreateParams;
    
            if (_messageBoxButtons != MessageBoxButtons.OK &&
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
}