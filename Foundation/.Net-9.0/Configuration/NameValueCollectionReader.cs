using System;
using System.Globalization;
using Foundation.Assertions;

namespace Foundation.Configuration;

public class NameValueCollectionReader
{
    private readonly TryGetValue<string, string> _tryGetValue;

    public delegate bool TryParse<T>(string s, out T value);

    public NameValueCollectionReader(TryGetValue<string, string> tryGetValue)
    {
        ArgumentNullException.ThrowIfNull(tryGetValue);
        _tryGetValue = tryGetValue;
    }

    public T GetValue<T>(string name, TryParse<T> tryParse, T defaultValue)
    {
        bool contains = TryGetValue(name, tryParse, out T value);
        if (!contains)
            value = defaultValue;

        return value;
    }

    public bool GetBoolean(string name, bool defaultValue) => GetValue(name, bool.TryParse, defaultValue);
    public double GetDouble(string name, double defaultValue) => GetValue(name, double.TryParse, defaultValue);
    public int GetInt32(string name, int defaultValue) => GetValue(name, int.TryParse, defaultValue);

    public string GetString(string name)
    {
        _tryGetValue(name, out string value);
        return value;
    }

    public bool TryGetBoolean(string name, out bool value)
    {
        bool contains = TryGetValue(name, bool.TryParse, out value);
        return contains;
    }

    public bool TryGetDateTime(string name, IFormatProvider provider, DateTimeStyles styles, out DateTime value)
    {
        bool contains = _tryGetValue(name, out string s);

        if (contains)
        {
            bool succeeded = DateTime.TryParse(s, provider, styles, out value);
            Assert.IsTrue(succeeded);
        }
        else
            value = default;

        return contains;
    }

    public bool TryGetDouble(string name, out double value)
    {
        bool contains = _tryGetValue(name, out string s);

        if (contains)
        {
            bool succeeded = double.TryParse(s, out value);
            Assert.IsTrue(succeeded);
        }
        else
            value = default;

        return contains;
    }

    public bool TryGetDouble(string name, NumberStyles style, IFormatProvider provider, out double value)
    {
        bool contains = _tryGetValue(name, out string s);

        if (contains)
        {
            bool succeeded = double.TryParse(s, style, provider, out value);
            Assert.IsTrue(succeeded);
        }
        else
            value = default;

        return contains;
    }

    public bool TryGetEnum<T>(string name, out T value)
    {
        bool contains = _tryGetValue(name, out string s);

        if (contains)
        {
            Type enumType = typeof(T);
            object valueObject = Enum.Parse(enumType, s);
            value = (T) valueObject;
        }
        else
            value = default;

        return contains;
    }

    public bool TryGetInt16(string name, out short value)
    {
        bool contains = TryGetValue(name, short.TryParse, out value);
        return contains;
    }

    public bool TryGetInt32(string name, out int value)
    {
        bool contains = TryGetValue(name, int.TryParse, out value);
        return contains;
    }

    public bool TryGetInt64(string name, out long value)
    {
        bool contains = TryGetValue(name, long.TryParse, out value);
        return contains;
    }

    public bool TryGetSingle(string name, NumberStyles style, IFormatProvider provider, out float value)
    {
        bool contains = _tryGetValue(name, out string s);

        if (contains)
        {
            bool succeeded = float.TryParse(s, style, provider, out value);
            Assert.IsTrue(succeeded);
        }
        else
            value = default;

        return contains;
    }

    public bool TryGetString(string name, out string value)
    {
        bool contains = _tryGetValue(name, out string s);

        value = contains
            ? s
            : null;

        return contains;
    }

    public bool TryGetTimeSpan(string name, out TimeSpan value)
    {
        bool contains = _tryGetValue(name, out string s);

        value = contains
            ? TimeSpan.Parse(s)
            : default;

        return contains;
    }

    public bool TryGetValue<T>(string name, TryParse<T> tryParse, out T value)
    {
        ArgumentNullException.ThrowIfNull(tryParse);

        bool contains = _tryGetValue(name, out string s);

        if (contains)
        {
            bool succeeded = tryParse(s, out value);
            Assert.IsTrue(succeeded);
        }
        else
            value = default;

        return contains;
    }
}