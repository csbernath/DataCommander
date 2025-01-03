using System;
using System.Windows.Forms;

namespace Foundation.Windows.Forms;

public static class ControlExtensions
{
    public static void Invoke(this Control control, Action action) => control.Invoke(action);
}