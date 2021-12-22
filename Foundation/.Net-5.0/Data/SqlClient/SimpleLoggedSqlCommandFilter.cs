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
        using (var log = LogFactory.Instance.GetCurrentMethodLog())
        {
            var node = _section.SelectNode(_nodeName, false);
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

            for (var i = 0; i < _rules.Length; i++)
            {
                var rule = _rules[i];
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