using Foundation.Windows.Forms;
using Xunit;

namespace Windows.Forms.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        Application.SetColorMode(SystemColorMode.Dark);
        const string? text = null;
        var testMessageBox = new TestMessageBox();
        testMessageBox.Show(text);
    }
}