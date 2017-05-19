namespace DataCommander.Foundation.Data.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using DataCommander.Foundation.Configuration;
    using DataCommander.Foundation.Data.SqlClient.SqlLoggedSqlConnection;
    using DataCommander.Foundation.Diagnostics;
    using DataCommander.Foundation.Diagnostics.Log;

    internal sealed class SimpleLoggedSqlCommandFilter : ISqlLoggedSqlCommandFilter
    {
        private readonly ConfigurationSection section;
        private readonly string nodeName;
        private SimpleLoggedSqlCommandFilterRule[] rules;

        public SimpleLoggedSqlCommandFilter(
            ConfigurationSection section,
            string nodeName)
        {
            this.section = section;
            this.nodeName = nodeName;
            this.section.Changed += this.SettingsChanged;
            this.SettingsChanged(null, null);
        }

        private void SettingsChanged(object sender, EventArgs e)
        {
            using (var log = LogFactory.Instance.GetCurrentMethodLog())
            {
                var node = this.section.SelectNode(this.nodeName, false);
                if (node != null)
                {
                    var list = new List<SimpleLoggedSqlCommandFilterRule>();

                    foreach (var childNode in node.ChildNodes)
                    {
                        var attributes = childNode.Attributes;
                        var include = attributes["Include"].GetValue<bool>();
                        var userName = attributes["UserName"].GetValue<string>();

                        if (userName == "*")
                        {
                            userName = null;
                        }

                        var hostName = attributes["HostName"].GetValue<string>();

                        if (hostName == "*")
                        {
                            hostName = null;
                        }

                        var database = attributes["Database"].GetValue<string>();

                        if (database == "*")
                        {
                            database = null;
                        }

                        var commandText = attributes["CommandText"].GetValue<string>();

                        if (commandText == "*")
                        {
                            commandText = null;
                        }

                        var rule = new SimpleLoggedSqlCommandFilterRule(include, userName, hostName, database, commandText);
                        list.Add(rule);
                    }

                    var count = list.Count;
                    SimpleLoggedSqlCommandFilterRule[] rules;

                    if (count > 0)
                    {
                        rules = new SimpleLoggedSqlCommandFilterRule[count];
                        list.CopyTo(rules);
                    }
                    else
                    {
                        rules = null;
                    }

                    this.rules = rules;
                }
            }
        }

        bool ISqlLoggedSqlCommandFilter.Contains(
            string userName,
            string hostName,
            IDbCommand command)
        {
            bool contains;

            if (this.rules != null && this.rules.Length > 0)
            {
                contains = false;

                for (var i = 0; i < this.rules.Length; i++)
                {
                    var rule = this.rules[i];
                    var match = rule.Match(userName, hostName, command);

                    if (match)
                    {
                        var include = rule.Include;
                        contains = (include && match) || !match;
                        break;
                    }
                }
            }
            else
            {
                contains = true;
            }

            return contains;
        }
    }
}