using Foundation.Windows.Forms;

namespace DataCommander.Application;

public static class DataCommanderMessageBox
{
    private static IMessageBox _messageBox;

    public static IMessageBox MessageBox => _messageBox;

    public static void Set(IMessageBox messageBox) => _messageBox = messageBox;
}