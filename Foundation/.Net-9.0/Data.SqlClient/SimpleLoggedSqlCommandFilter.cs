using System;
using System.Collections.Generic;
using System.Data;
using Foundation.Configuration;
using Foundation.Data.SqlClient.SqlLoggedSqlConnection;
using Foundation.Log;

namespace Foundation.Data.SqlClient;

internal sealed class SimpleLoggedSqlCommandFilter : ISqlLoggedSqlCommandFilter
{
    private readonly ConfigurationSection _section;
    private readonly string _nodeName;
    private SimpleLoggedSqlCommandFilterRule[] _rules;

    public SimpleLoggedSqlCommandFilter(
        ConfigurationSection section,
        string nodeName)
    {
        _section = section;
        _nodeName = nodeName;
        _section.Changed += SettingsChanged;
        SettingsChanged(null, null);
    }

    private void SettingsChanged(object sender, EventArgs e)
    {
        using (ILog log = LogFactory.Instance.GetCurrentMethodLog())
        {
            ConfigurationNode node = _section.SelectNode(_nodeName, false);
            if (node != null)
            {
                List<SimpleLoggedSqlCommandFilterRule> list = [];

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

                _rules = rules;
            }
        }
    }

    bool ISqlLoggedSqlCommandFilter.Contains(
        string userName,
        string hostName,
        IDbCommand command)
    {
        bool contains;

        if (_rules != null && _rules.Length > 0)
        {
            contains = false;

            for (int i = 0; i < _rules.Length; i++)
            {
                SimpleLoggedSqlCommandFilterRule rule = _rules[i];
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