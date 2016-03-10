namespace DataCommander.Providers.MySql
{
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Text;

    internal sealed class DatabaseObjectMultipartName
    {
        private string database;
        private readonly string name;

        public DatabaseObjectMultipartName(string currentDatabase, List<string> nameParts)
        {
            if (nameParts != null)
            {
                int i = nameParts.Count - 1;
                var commandBuilder = new SqlCommandBuilder();

                if (i >= 0)
                {
                    this.name = nameParts[i];
                    i--;


                    if (i >= 0)
                    {
                        this.database = nameParts[i];
                    }
                }
            }

            if (this.database == null)
            {
                this.database = currentDatabase;
            }

            if (this.name != null)
            {
                int length = this.name.Length;

                if (length > 0 && this.name[0] == '[')
                {
                    this.name = this.name.Substring(1);
                    length--;
                }

                if (length > 0 && this.name[length - 1] == ']')
                {
                    this.name = this.name.Substring(0, length - 1);
                }
            }
        }

        public string Database
        {
            get
            {
                return this.database;
            }

            set
            {
                this.database = value;
            }
        }

        public string Name => this.name;

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (this.database != null)
            {
                sb.Append(this.database);
            }

            if (sb.Length > 0)
            {
                sb.Append('.');
            }

            sb.Append(this.name);
            return sb.ToString();
        }
    }
}