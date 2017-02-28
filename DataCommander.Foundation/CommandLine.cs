namespace DataCommander.Foundation
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Text;
    using DataCommander.Foundation.Collections;
    using DataCommander.Foundation.Linq;

    /// <summary>
    /// 
    /// </summary>
    public sealed class CommandLine
    {
        private readonly IndexableCollection<CommandLineArgument> arguments;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandLine"></param>
        public CommandLine(string commandLine)
        {
            Contract.Requires<ArgumentNullException>(commandLine != null);

            this.arguments = new IndexableCollection<CommandLineArgument>(this.ListIndex);
            var dictionary = new Dictionary<string, ICollection<CommandLineArgument>>(StringComparer.InvariantCultureIgnoreCase);
            this.NameIndex = new NonUniqueIndex<string, CommandLineArgument>(
                "nameIndex",
                argument => GetKeyResponse.Create(argument.Name != null, argument.Name),
                dictionary,
                () => new List<CommandLineArgument>());
            this.arguments.Indexes.Add(this.NameIndex);
            var stringReader = new StringReader(commandLine);
            var arguments = Parse(stringReader);
            this.arguments.Add(arguments);
        }

        /// <summary>
        /// 
        /// </summary>
        public ListIndex<CommandLineArgument> ListIndex { get; } = new ListIndex<CommandLineArgument>("listIndex");

        /// <summary>
        /// 
        /// </summary>
        public NonUniqueIndex<string, CommandLineArgument> NameIndex { get; }

        #region Private Methods

        private static string ReadString(TextReader textReader)
        {
            var read = textReader.Read();
            var c = (Char) read;
            Contract.Assert(c == '"');
            var sb = new StringBuilder();

            while (true)
            {
                read = textReader.Read();
                Contract.Assert(read >= 0);
                c = (Char) read;

                if (c == '"')
                {
                    break;
                }
                else
                {
                    sb.Append(c);
                }
            }

            var value = sb.ToString();
            return value;
        }

        private static string ReadName(TextReader textReader)
        {
            var read = textReader.Read();
            var c = (Char) read;
            Contract.Assert(c == '/' || c == '-');
            var sb = new StringBuilder();

            while (true)
            {
                var peek = textReader.Peek();

                if (peek == -1)
                {
                    break;
                }

                c = (Char) peek;

                if (c == ':' || c == '=' || Char.IsWhiteSpace(c))
                {
                    break;
                }
                else
                {
                    sb.Append(c);
                    textReader.Read();
                }
            }

            string name;

            if (sb.Length > 0)
            {
                name = sb.ToString();
            }
            else
            {
                name = null;
            }

            return name;
        }

        private static string ReadValue(TextReader textReader)
        {
            var sb = new StringBuilder();

            while (true)
            {
                var peek = textReader.Peek();

                if (peek == -1)
                {
                    break;
                }

                var c = (Char) peek;

                if (Char.IsWhiteSpace(c))
                {
                    break;
                }
                else
                {
                    sb.Append(c);
                }

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
                var c = (Char) peek;

                if (c == ':' || c == '=')
                {
                    textReader.Read();
                    peek = textReader.Peek();
                    c = (Char) peek;

                    if (peek == '"')
                    {
                        value = ReadString(textReader);
                    }
                    else
                    {
                        value = ReadValue(textReader);
                    }
                }
                else
                {
                    value = null;
                }
            }
            else
            {
                value = null;
            }

            return Tuple.Create(name, value);
        }

        private static IEnumerable<CommandLineArgument> Parse(TextReader textReader)
        {
            var index = 0;

            while (true)
            {
                var peek = textReader.Peek();

                if (peek == -1)
                {
                    break;
                }

                var c = (Char) peek;

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
                else if (Char.IsWhiteSpace(c))
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
}