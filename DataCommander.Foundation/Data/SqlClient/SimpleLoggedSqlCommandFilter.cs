namespace DataCommander.Foundation.Data.SqlClient
{
    using System;
    using System.Collections.Generic;
    using DataCommander.Foundation.Configuration;
    using DataCommander.Foundation.Diagnostics;

    internal sealed class SimpleLoggedSqlCommandFilter : ISqlLoggedSqlCommandFilter
    {
        private ConfigurationSection section;
        private String nodeName;
        private SimpleLoggedSqlCommandFilterRule[] rules;

        public SimpleLoggedSqlCommandFilter(
            ConfigurationSection section,
            String nodeName)
        {
            this.section = section;
            this.nodeName = nodeName;
            this.section.Changed += this.SettingsChanged;
            this.SettingsChanged(null, null);
        }

        private void SettingsChanged(Object sender, EventArgs e)
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
                        Boolean include = attributes["Include"].GetValue<Boolean>();
                        String userName = attributes["UserName"].GetValue<String>();

                        if (userName == "*")
                        {
                            userName = null;
                        }

                        String hostName = attributes["HostName"].GetValue<String>();

                        if (hostName == "*")
                        {
                            hostName = null;
                        }

                        String database = attributes["Database"].GetValue<String>();

                        if (database == "*")
                        {
                            database = null;
                        }

                        String commandText = attributes["CommandText"].GetValue<String>();

                        if (commandText == "*")
                        {
                            commandText = null;
                        }

                        SimpleLoggedSqlCommandFilterRule rule = new SimpleLoggedSqlCommandFilterRule(include, userName, hostName, database, commandText);
                        list.Add(rule);
                    }

                    Int32 count = list.Count;
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

        Boolean ISqlLoggedSqlCommandFilter.Contains(
            String userName,
            String hostName,
            System.Data.IDbCommand command)
        {
            Boolean contains;

            if (this.rules != null && this.rules.Length > 0)
            {
                contains = false;

                for (Int32 i = 0; i < this.rules.Length; i++)
                {
                    SimpleLoggedSqlCommandFilterRule rule = this.rules[i];
                    Boolean match = rule.Match(userName, hostName, command);

                    if (match)
                    {
                        Boolean include = rule.Include;
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