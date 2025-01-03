using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Foundation.Configuration;

public static class AppSettings
{
    public static readonly Lazy<NameValueCollectionReader> Instance = new(CreateInstance);

    private static NameValueCollectionReader CreateInstance()
    {
        var reader = new Reader(ConfigurationManager.AppSettings);
        return new NameValueCollectionReader(reader.TryGetValue);
    }

    public static NameValueCollectionReader CurrentType
    {
        get
        {
            var nameValueCollection = ConfigurationManager.AppSettings;

            var stackTrace = new StackTrace(1);
            var stackFrame = stackTrace.GetFrame(0)!;
            var methodBase = stackFrame.GetMethod()!;
            var typeName = methodBase.DeclaringType!.FullName;
            var prefix = typeName + Type.Delimiter;

            var reader = new PrefixedReader(nameValueCollection, prefix);
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

        public bool TryGetValue(string name, [MaybeNullWhen(false)] out string value)
        {
            value = _nameValueCollection[name]!;
            var contains = value != null;
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

        public bool TryGetValue(string name, [MaybeNullWhen(false)] out string value)
        {
            var prefixedName = _prefix != null ? _prefix + name : name;
            value = _nameValueCollection[prefixedName];
            var contains = value != null;
            return contains;
        }
    }
}