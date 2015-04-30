namespace DataCommander.Foundation.Configuration
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Globalization;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public delegate bool TryGetValue<TKey, TValue>(TKey key, out TValue value);

    /// <summary>
    /// 
    /// </summary>
    public class NameValueCollectionReader
    {
        private readonly TryGetValue<string, string> tryGetValue;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public delegate bool TryParse<T>(string s, out T value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tryGetValue"></param>
        public NameValueCollectionReader(TryGetValue<string, string> tryGetValue)
        {
            Contract.Requires(tryGetValue != null);
            this.tryGetValue = tryGetValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="tryParse"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetValue<T>(string name, TryParse<T> tryParse, T defaultValue)
        {
            T value;
            bool contains = this.TryGetValue(name, tryParse, out value);

            if (!contains)
            {
                value = defaultValue;
            }

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public bool GetBoolean(string name, bool defaultValue)
        {
            return this.GetValue(name, bool.TryParse, defaultValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public double GetDouble(string name, double defaultValue)
        {
            return this.GetValue(name, double.TryParse, defaultValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public int GetInt32(string name, int defaultValue)
        {
            return this.GetValue(name, int.TryParse, defaultValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetString(string name)
        {
            string value;
            this.tryGetValue(name, out value);
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetBoolean(string name, out bool value)
        {
            bool contains = this.TryGetValue(name, bool.TryParse, out value);
            return contains;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="provider"></param>
        /// <param name="styles"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetDateTime(string name, IFormatProvider provider, DateTimeStyles styles, out DateTime value)
        {
            string s;
            bool contains = this.tryGetValue(name, out s);

            if (contains)
            {
                bool succeeded = DateTime.TryParse(s, provider, styles, out value);
                Contract.Assert(succeeded);
            }
            else
            {
                value = default(DateTime);
            }

            return contains;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetDouble(string name, out double value)
        {
            string s;
            bool contains = this.tryGetValue(name, out s);

            if (contains)
            {
                bool succeeded = double.TryParse(s, out value);
                Contract.Assert(succeeded);
            }
            else
            {
                value = default(double);
            }

            return contains;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="style"></param>
        /// <param name="provider"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetDouble(string name, NumberStyles style, IFormatProvider provider, out double value)
        {
            string s;
            bool contains = this.tryGetValue(name, out s);

            if (contains)
            {
                bool succeeded = double.TryParse(s, style, provider, out value);
                Contract.Assert(succeeded);
            }
            else
            {
                value = default(double);
            }

            return contains;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetEnum<T>(string name, out T value)
        {
            string s;
            bool contains = this.tryGetValue(name, out s);

            if (contains)
            {
                Type enumType = typeof(T);
                object valueObject = Enum.Parse(enumType, s);
                value = (T)valueObject;
            }
            else
            {
                value = default(T);
            }

            return contains;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetInt16(string name, out Int16 value)
        {
            bool contains = this.TryGetValue(name, Int16.TryParse, out value);
            return contains;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetInt32(string name, out int value)
        {
            bool contains = this.TryGetValue(name, int.TryParse, out value);
            return contains;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetInt64(string name, out long value)
        {
            bool contains = this.TryGetValue(name, long.TryParse, out value);
            return contains;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="style"></param>
        /// <param name="provider"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetSingle(string name, NumberStyles style, IFormatProvider provider, out Single value)
        {
            string s;
            bool contains = this.tryGetValue(name, out s);

            if (contains)
            {
                bool succeeded = Single.TryParse(s, style, provider, out value);
                Contract.Assert(succeeded);
            }
            else
            {
                value = default(Single);
            }

            return contains;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetString(string name, out string value)
        {
            string s;
            bool contains = this.tryGetValue(name, out s);

            value = contains
                ? s
                : null;

            return contains;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetTimeSpan(string name, out TimeSpan value)
        {
            string s;
            bool contains = this.tryGetValue(name, out s);

            value = contains
                ? TimeSpan.Parse(s)
                : default(TimeSpan);

            return contains;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="tryParse"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue<T>(string name, TryParse<T> tryParse, out T value)
        {
            Contract.Requires(tryParse != null);
            string s;
            bool contains = this.tryGetValue(name, out s);

            if (contains)
            {
                bool succeeded = tryParse(s, out value);
                Contract.Assert(succeeded);
            }
            else
            {
                value = default(T);
            }

            return contains;
        }
    }
}