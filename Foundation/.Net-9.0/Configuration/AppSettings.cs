using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;

namespace Foundation.Configuration;

public static class AppSettings
{
    public static readonly Lazy<NameValueCollectionReader> Instance = new(CreateInstance);

    private static NameValueCollectionReader CreateInstance()
    {
        Reader reader = new Reader(ConfigurationManager.AppSettings);
        return new NameValueCollectionReader(reader.TryGetValue);
    }

    public static NameValueCollectionReader CurrentType
    {
        get
        {
            NameValueCollection nameValueCollection = ConfigurationManager.AppSettings;

            StackTrace stackTrace = new StackTrace(1);
            StackFrame stackFrame = stackTrace.GetFrame(0);
            System.Reflection.MethodBase methodBase = stackFrame.GetMethod();
            string typeName = methodBase.DeclaringType.FullName;
            string prefix = typeName + Type.Delimiter;

            PrefixedReader reader = new PrefixedReader(nameValueCollection, prefix);
            return new NameValueCollectionReader(reader.TryGetValue);
        }
    }

    private sealed class Reader
    {
        private readonly NameValueCollection _nameValueCollection;

        public Reader(NameValueCollection nameValueCollection)
        {
            ArgumentNullException.ThrowIfNull(nameValueCollection);

            _nameValueCollection = nameValueCollection;
        }

        public bool TryGetValue(string name, out string value)
        {
            value = _nameValueCollection[name];
            bool contains = value != null;
            return contains;
        }
    }

    private sealed class PrefixedReader
    {
        private readonly NameValueCollection _nameValueCollection;
        private readonly string _prefix;

        public PrefixedReader(NameValueCollection nameValueCollection, string prefix)
        {
            ArgumentNullException.ThrowIfNull(nameValueCollection);
            ArgumentNullException.ThrowIfNull(prefix);

            _nameValueCollection = nameValueCollection;
            _prefix = prefix;
        }

        public bool TryGetValue(string name, out string value)
        {
            string prefixedName = _prefix != null ? _prefix + name : name;
            value = _nameValueCollection[prefixedName];
            bool contains = value != null;
            return contains;
        }
    }
}