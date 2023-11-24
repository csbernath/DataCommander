using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Foundation.Assertions;
using Foundation.Collections.IndexableCollection;
using Foundation.Linq;

namespace Foundation.Collections;

public sealed class CommandLine
{
    private readonly IndexableCollection<CommandLineArgument> _arguments;

    public CommandLine(string commandLine)
    {
        ArgumentNullException.ThrowIfNull(commandLine);

        _arguments = new IndexableCollection<CommandLineArgument>(ListIndex);
        var dictionary = new Dictionary<string, ICollection<CommandLineArgument>>(StringComparer.InvariantCultureIgnoreCase);
        NameIndex = new NonUniqueIndex<string, CommandLineArgument>(
            "nameIndex",
            argument => GetKeyResponse.Create(argument.Name != null, argument.Name),
            dictionary,
            () => new List<CommandLineArgument>());
        _arguments.Indexes.Add(NameIndex);
        var stringReader = new StringReader(commandLine);
        var arguments = Parse(stringReader);
        _arguments.Add(arguments);
    }

    public ListIndex<CommandLineArgument> ListIndex { get; } = new("listIndex");
    public NonUniqueIndex<string, CommandLineArgument> NameIndex { get; }

    #region Private Methods

    private static string ReadString(TextReader textReader)
    {
        var read = textReader.Read();
        var c = (char)read;

        Assert.IsTrue(c == '"');

        var sb = new StringBuilder();

        while (true)
        {
            read = textReader.Read();
            Assert.IsTrue(read >= 0);

            c = (char)read;

            if (c == '"')
                break;
            sb.Append(c);
        }

        var value = sb.ToString();
        return value;
    }

    private static string ReadName(TextReader textReader)
    {
        var read = textReader.Read();
        var c = (char)read;
        Assert.IsTrue(c == '/' || c == '-');

        var sb = new StringBuilder();

        while (true)
        {
            var peek = textReader.Peek();

            if (peek == -1) break;

            c = (char)peek;

            if (c == ':' || c == '=' || char.IsWhiteSpace(c)) break;

            sb.Append(c);
            textReader.Read();
        }

        var name = sb.Length > 0 ? sb.ToString() : null;

        return name;
    }

    private static string ReadValue(TextReader textReader)
    {
        var sb = new StringBuilder();

        while (true)
        {
            var peek = textReader.Peek();

            if (peek == -1)
                break;

            var c = (char)peek;

            if (char.IsWhiteSpace(c))
                break;

            sb.Append(c);

            textReader.Read();
        }

        var value = sb.ToString();
        return value;
    }

    private static Tuple<string, string> ReadNameValue(TextReader textReader)
    {
        var name = ReadName(textReader);
        string value;
        var peek = textReader.Peek();

        if (peek >= 0)
        {
            var c = (char)peek;

            if (c == ':' || c == '=')
            {
                textReader.Read();
                peek = textReader.Peek();
                c = (char)peek;

                value = peek == '"'
                    ? ReadString(textReader)
                    : ReadValue(textReader);
            }
            else
                value = null;
        }
        else
            value = null;

        return Tuple.Create(name, value);
    }

    private static IEnumerable<CommandLineArgument> Parse(TextReader textReader)
    {
        var index = 0;

        while (true)
        {
            var peek = textReader.Peek();

            if (peek == -1) break;

            var c = (char)peek;

            if (c == '"')
            {
                var value = ReadString(textReader);
                var argument = new CommandLineArgument(index, null, value);
                index++;
                yield return argument;
            }
            else if (c == '/' || c == '-')
            {
                var nameValue = ReadNameValue(textReader);
                var argument = new CommandLineArgument(index, nameValue.Item1, nameValue.Item2);
                index++;
                yield return argument;
            }
            else if (char.IsWhiteSpace(c))
            {
                textReader.Read();
            }
            else
            {
                var value = ReadValue(textReader);
                var argument = new CommandLineArgument(index, null, value);
                index++;
                yield return argument;
            }
        }
    }

    #endregion
}