using Xunit;

namespace Windows.Forms.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
#pragma warning disable WFO5001
        Application.SetColorMode(SystemColorMode.Dark);
#pragma warning restore WFO5001
        
        const string? text = null;
        // var testMessageBox = new TestMessageBox();
        // testMessageBox.Show(text);
    }
}