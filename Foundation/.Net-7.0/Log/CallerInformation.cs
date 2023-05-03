using System.Runtime.CompilerServices;

namespace Foundation.Log;

public sealed class CallerInformation
{
    public readonly string CallerMemberName;
    public readonly string CallerFilePath;
    public readonly int CallerLineNumber;

    public static CallerInformation Create(
        [CallerMemberName] string callerMemberName = null,
        [CallerFilePath] string callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0) => new(callerMemberName, callerFilePath, callerLineNumber);

    private CallerInformation(string callerMemberName, string callerFilePath, int callerLineNumber)
    {
        CallerMemberName = callerMemberName;
        CallerFilePath = callerFilePath;
        CallerLineNumber = callerLineNumber;
    }
}