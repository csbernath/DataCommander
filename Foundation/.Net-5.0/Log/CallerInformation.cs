﻿using System.Runtime.CompilerServices;

namespace Foundation.Log;

public sealed class CallerInformation
{
    public readonly string CallerMemberName;
    public readonly string CallerFilePath;
    public readonly int CallerLineNumber;

    private CallerInformation(string callerMemberName, string callerFilePath, int callerLineNumber)
    {
        CallerMemberName = callerMemberName;
        CallerFilePath = callerFilePath;
        CallerLineNumber = callerLineNumber;
    }

    public static CallerInformation Get(
        [CallerMemberName] string callerMemberName = null,
        [CallerFilePath] string callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0) => new(callerMemberName, callerFilePath, callerLineNumber);
}