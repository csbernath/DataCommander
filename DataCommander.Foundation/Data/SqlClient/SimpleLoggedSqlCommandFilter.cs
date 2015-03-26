namespace DataCommander.Foundation.Data.SqlClient
{
    using System;
    using System.Collections.Generic;
    using DataCommander.Foundation.Configuration;
    using DataCommander.Foundation.Diagnostics;

    internal sealed class SimpleLoggedSqlCommandFilter : ISqlLoggedSqlCommandFilter
    {
        private ConfigurationSection section;
        private string nodeName;
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
                ConfigurationNode node = this.section.SelectNode(this.nodeName, false);
                if (node != null)
                {
                    var list = new List<SimpleLoggedSqlCommandFilterRule>();

                    foreach (ConfigurationNode childNode in node.ChildNodes)
                    {
                        ConfigurationAttributeCollection attributes = childNode.Attributes;
                        bool include = attributes["Include"].GetValue<bool>();
                        string userName = attributes["UserName"].GetValue<string>();

                        if (userName == "*")
                        {
                            userName = null;
                        }

                        string hostName = attributes["HostName"].GetValue<string>();

                        if (hostName == "*")
                        {
                            hostName = null;
                        }

                        string database = attributes["Database"].GetValue<string>();

                        if (database == "*")
                        {
                            database = null;
                        }

                        string commandText = attributes["CommandText"].GetValue<string>();

                        if (commandText == "*")
                        {
                            commandText = null;
                        }

                        SimpleLoggedSqlCommandFilterRule rule = new SimpleLoggedSqlCommandFilterRule(include, userName, hostName, database, commandText);
                        list.Add(rule);
                    }

                    int count = list.Count;
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
            System.Data.IDbCommand command)
        {
            bool contains;

            if (this.rules != null && this.rules.Length > 0)
            {
                contains = false;

                for (int i = 0; i < this.rules.Length; i++)
                {
                    SimpleLoggedSqlCommandFilterRule rule = this.rules[i];
                    bool match = rule.Match(userName, hostName, command);

                    if (match)
                    {
                        bool include = rule.Include;
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