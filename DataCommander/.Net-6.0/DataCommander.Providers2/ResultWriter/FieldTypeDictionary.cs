﻿using System;
using DataCommander.Providers2.FieldNamespace;
using Foundation.Core;

namespace DataCommander.Providers2.ResultWriter;

public static class FieldTypeDictionary
{
    static FieldTypeDictionary()
    {
        Instance.Add<Guid>(FieldType.Guid);
        Instance.Add<string>(FieldType.String);
        Instance.Add<string[]>(FieldType.StringArray);
        Instance.Add<BinaryField>(FieldType.BinaryField);
        Instance.Add<DateTimeField>(FieldType.DateTimeField);
        Instance.Add<StreamField>(FieldType.StreamField);
        Instance.Add<StringField>(FieldType.StringField);
    }

    public static TypeDictionary<FieldType> Instance { get; } = new();
}