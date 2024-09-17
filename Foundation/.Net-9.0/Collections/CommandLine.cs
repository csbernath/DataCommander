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
        Dictionary<string, ICollection<CommandLineArgument>> dictionary = new Dictionary<string, ICollection<CommandLineArgument>>(StringComparer.InvariantCultureIgnoreCase);
        NameIndex = new NonUniqueIndex<string, CommandLineArgument>(
            "nameIndex",
            argument => GetKeyResponse.Create(argument.Name != null, argument.Name),
            dictionary,
            () => []);
        _arguments.Indexes.Add(NameIndex);
        StringReader stringReader = new StringReader(commandLine);
        IEnumerable<CommandLineArgument> arguments = Parse(stringReader);
        _arguments.Add(arguments);
    }

    public ListIndex<CommandLineArgument> ListIndex { get; } = new("listIndex");
    public NonUniqueIndex<string, CommandLineArgument> NameIndex { get; }

    private static string ReadString(TextReader textReader)
    {
        int read = textReader.Read();
        char c = (char)read;

        Assert.IsTrue(c == '"');

        StringBuilder sb = new StringBuilder();

        while (true)
        {
            read = textReader.Read();
            Assert.IsTrue(read >= 0);

            c = (char)read;

            if (c == '"')
                break;
            sb.Append(c);
        }

        string value = sb.ToString();
        return value;
    }

    private static string ReadName(TextReader textReader)
    {
        int read = textReader.Read();
        char c = (char)read;
        Assert.IsTrue(c == '/' || c == '-');

        StringBuilder sb = new StringBuilder();

        while (true)
        {
            int peek = textReader.Peek();

            if (peek == -1) break;

            c = (char)peek;

            if (c == ':' || c == '=' || char.IsWhiteSpace(c)) break;

            sb.Append(c);
            textReader.Read();
        }

        string name = sb.Length > 0 ? sb.ToString() : null;

        return name;
    }

    private static string ReadValue(TextReader textReader)
    {
        StringBuilder sb = new StringBuilder();

        while (true)
        {
            int peek = textReader.Peek();

            if (peek == -1)
                break;

            char c = (char)peek;

            if (char.IsWhiteSpace(c))
                break;

            sb.Append(c);

            textReader.Read();
        }

        string value = sb.ToString();
        return value;
    }

    private static Tuple<string, string> ReadNameValue(TextReader textReader)
    {
        string name = ReadName(textReader);
        string value;
        int peek = textReader.Peek();

        if (peek >= 0)
        {
            char c = (char)peek;

            if (c == ':' || c == '=')
            {
                textReader.Read();
                peek = textReader.Peek();
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
        int index = 0;

        while (true)
        {
            int peek = textReader.Peek();

            if (peek == -1) break;

            char c = (char)peek;

            if (c == '"')
            {
                string value = ReadString(textReader);
                CommandLineArgument argument = new CommandLineArgument(index, null, value);
                index++;
                yield return argument;
            }
            else if (c == '/' || c == '-')
            {
                Tuple<string, string> nameValue = ReadNameValue(textReader);
                CommandLineArgument argument = new CommandLineArgument(index, nameValue.Item1, nameValue.Item2);
                index++;
                yield return argument;
            }
            else if (char.IsWhiteSpace(c))
            {
                textReader.Read();
            }
            else
            {
                string value = ReadValue(textReader);
                CommandLineArgument argument = new CommandLineArgument(index, null, value);
                index++;
                yield return argument;
            }
        }
    }
}