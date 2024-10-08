﻿namespace DataCommander.Application.ResultWriter;

public static class StringExtensions
{
    public static string SingularOrPlural(int count, string singular, string plural) => count == 1
            ? $"{count} {singular}"
            : $"{count} {plural}";
}