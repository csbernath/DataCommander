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
        private IndexableCollection<CommandLineArgument> arguments;
        private ListIndex<CommandLineArgument> listIndex = new ListIndex<CommandLineArgument>("listIndex");
        private NonUniqueIndex<string, CommandLineArgument> nameIndex;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandLine"></param>
        public CommandLine(string commandLine)
        {
            Contract.Requires(commandLine != null);

            this.arguments = new IndexableCollection<CommandLineArgument>(this.listIndex);
            var dictionary = new Dictionary<string, ICollection<CommandLineArgument>>(StringComparer.InvariantCultureIgnoreCase);
            this.nameIndex = new NonUniqueIndex<string, CommandLineArgument>(
                "nameIndex",
                argument => GetKeyResponse.Create(argument.Name != null, argument.Name),
                dictionary,
                () => new List<CommandLineArgument>());
            this.arguments.Indexes.Add(this.nameIndex);
            var stringReader = new StringReader(commandLine);
            var arguments = Parse(stringReader);
            this.arguments.Add(arguments);
        }

        /// <summary>
        /// 
        /// </summary>
        public ListIndex<CommandLineArgument> ListIndex
        {
            get
            {
                return this.listIndex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public NonUniqueIndex<string, CommandLineArgument> NameIndex
        {
            get
            {
                return this.nameIndex;
            }
        }

        #region Private Methods

        private static string ReadString(TextReader textReader)
        {
            int read = textReader.Read();
            Char c = (Char) read;
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

            string value = sb.ToString();
            return value;
        }

        private static string ReadName(TextReader textReader)
        {
            int read = textReader.Read();
            Char c = (Char) read;
            Contract.Assert(c == '/' || c == '-');
            var sb = new StringBuilder();

            while (true)
            {
                int peek = textReader.Peek();

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
            StringBuilder sb = new StringBuilder();

            while (true)
            {
                int peek = textReader.Peek();

                if (peek == -1)
                {
                    break;
                }

                Char c = (Char) peek;

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
                Char c = (Char) peek;

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
            int index = 0;

            while (true)
            {
                int peek = textReader.Peek();

                if (peek == -1)
                {
                    break;
                }

                Char c = (Char) peek;

                if (c == '"')
                {
                    string value = ReadString(textReader);
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
                    string value = ReadValue(textReader);
                    var argument = new CommandLineArgument(index, null, value);
                    index++;
                    yield return argument;
                }
            }
        }

        #endregion
    }
}