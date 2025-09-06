using Foundation.Windows.Forms;

namespace DataCommander.Application;

public static class DataCommanderMessageBox
{
    public static IMessageBox MessageBox = null;

    public static void Set(IMessageBox messageBox) => MessageBox = messageBox;
}