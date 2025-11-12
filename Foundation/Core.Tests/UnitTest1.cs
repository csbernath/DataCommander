using Foundation.Core;

namespace Core.Tests;

public class MemoryExtensionsTests
{
    [Fact]
    public void Test1()
    {
        var commandText = "select 1\r\ngo\r\nselect 2\r\n";
        var value = '\n';
        //IndexOf(commandText, value, 0);
        //IndexOf(commandText, value, 10);
        //IndexOf(commandText, value, 20);
        //IndexOf(commandText, value, 30);

        AssertResultsEqual(
            () => commandText.LastIndexOf(value, 10),
            () => commandText.AsSpan().LastIndexOf(value, 10)
        );
    }

    private static void IndexOf(string s, char value, int startIndex)
    {
        int? indexOf = null;
        Exception? exception1 = null;
        try
        {
            indexOf = s.IndexOf(value, startIndex);
        }
        catch (Exception e)
        {
            exception1 = e;
        }

        int? indexOfAsSpan = null;
        Exception? exception2 = null;
        try
        {
            indexOfAsSpan = s.AsSpan().IndexOf(value, startIndex);
        }
        catch (Exception e)
        {
            exception2 = e;
        }

        if (exception1 == null)
            Assert.Equal(indexOf, indexOfAsSpan);
        else
            Assert.Equal(exception1.GetType(), exception2.GetType());
    }

    private static void AssertResultsEqual(Func<int> func1, Func<int> func2)
    {
        var (result1, exception1) = Execute(func1);
        var (result2, exception2) = Execute(func2);

        if (exception1 == null)
            Assert.Equal(result1, result2);
        else
            Assert.Equal(exception1.GetType(), exception2.GetType());
    }

    private static (int? result, Exception? exception) Execute(Func<int> function)
    {
        int? result = null;
        Exception? exception = null;
        try
        {
            result = function();
        }
        catch (Exception e)
        {
            exception = e;
        }
        return (result, exception);
    }
}
