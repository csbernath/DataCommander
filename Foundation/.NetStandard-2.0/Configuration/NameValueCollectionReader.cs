using System;
using System.Globalization;
using Foundation.Assertions;

namespace Foundation.Configuration
{
    public class NameValueCollectionReader
    {
        private readonly TryGetValue<string, string> _tryGetValue;

        public delegate bool TryParse<T>(string s, out T value);

        public NameValueCollectionReader(TryGetValue<string, string> tryGetValue)
        {
            Assert.IsNotNull(tryGetValue);
            _tryGetValue = tryGetValue;
        }

        public T GetValue<T>(string name, TryParse<T> tryParse, T defaultValue)
        {
            T value;
            var contains = TryGetValue(name, tryParse, out value);
            if (!contains)
                value = defaultValue;

            return value;
        }

        public bool GetBoolean(string name, bool defaultValue) => GetValue(name, bool.TryParse, defaultValue);
        public double GetDouble(string name, double defaultValue) => GetValue(name, double.TryParse, defaultValue);
        public int GetInt32(string name, int defaultValue) => GetValue(name, int.TryParse, defaultValue);

        public string GetString(string name)
        {
            string value;
            _tryGetValue(name, out value);
            return value;
        }

        public bool TryGetBoolean(string name, out bool value)
        {
            var contains = TryGetValue(name, bool.TryParse, out value);
            return contains;
        }

        public bool TryGetDateTime(string name, IFormatProvider provider, DateTimeStyles styles, out DateTime value)
        {
            string s;
            var contains = _tryGetValue(name, out s);

            if (contains)
            {
                var succeeded = DateTime.TryParse(s, provider, styles, out value);
                Assert.IsTrue(succeeded);
            }
            else
                value = default(DateTime);

            return contains;
        }

        public bool TryGetDouble(string name, out double value)
        {
            string s;
            var contains = _tryGetValue(name, out s);

            if (contains)
            {
                var succeeded = double.TryParse(s, out value);
                Assert.IsTrue(succeeded);
            }
            else
                value = default(double);

            return contains;
        }

        public bool TryGetDouble(string name, NumberStyles style, IFormatProvider provider, out double value)
        {
            string s;
            var contains = _tryGetValue(name, out s);

            if (contains)
            {
                var succeeded = double.TryParse(s, style, provider, out value);
                Assert.IsTrue(succeeded);
            }
            else
                value = default(double);

            return contains;
        }

        public bool TryGetEnum<T>(string name, out T value)
        {
            string s;
            var contains = _tryGetValue(name, out s);

            if (contains)
            {
                var enumType = typeof(T);
                var valueObject = Enum.Parse(enumType, s);
                value = (T) valueObject;
            }
            else
                value = default(T);

            return contains;
        }

        public bool TryGetInt16(string name, out short value)
        {
            var contains = TryGetValue(name, short.TryParse, out value);
            return contains;
        }

        public bool TryGetInt32(string name, out int value)
        {
            var contains = TryGetValue(name, int.TryParse, out value);
            return contains;
        }

        public bool TryGetInt64(string name, out long value)
        {
            var contains = TryGetValue(name, long.TryParse, out value);
            return contains;
        }

        public bool TryGetSingle(string name, NumberStyles style, IFormatProvider provider, out float value)
        {
            string s;
            var contains = _tryGetValue(name, out s);

            if (contains)
            {
                var succeeded = float.TryParse(s, style, provider, out value);
                Assert.IsTrue(succeeded);
            }
            else
                value = default(float);

            return contains;
        }

        public bool TryGetString(string name, out string value)
        {
            string s;
            var contains = _tryGetValue(name, out s);

            value = contains
                ? s
                : null;

            return contains;
        }

        public bool TryGetTimeSpan(string name, out TimeSpan value)
        {
            string s;
            var contains = _tryGetValue(name, out s);

            value = contains
                ? TimeSpan.Parse(s)
                : default(TimeSpan);

            return contains;
        }

        public bool TryGetValue<T>(string name, TryParse<T> tryParse, out T value)
        {
            Assert.IsNotNull(tryParse);

            string s;
            var contains = _tryGetValue(name, out s);

            if (contains)
            {
                var succeeded = tryParse(s, out value);
                Assert.IsTrue(succeeded);
            }
            else
                value = default(T);

            return contains;
        }
    }
}