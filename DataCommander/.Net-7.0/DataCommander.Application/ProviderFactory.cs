using System;
using System.Collections.Generic;
using ADODB;
using DataCommander.Api;
using Foundation.Assertions;
using Foundation.Configuration;

namespace DataCommander.Application;

public static class ProviderFactory
{
    public static IEnumerable<Provider> GetProviders()
    {
        var node = Settings.SelectNode("DataCommander/Providers", true);

        foreach (var childNode in node.ChildNodes)
        {
            childNode.Attributes.TryGetAttributeValue("Enabled", out bool enabled);
            if (enabled)
            {
                var identifier = childNode.Name;

                if (!childNode.Attributes.TryGetAttributeValue("Name", out string name))
                    name = identifier;

                var provider = new Provider(identifier, name);
                yield return provider;
            }
        }
    }

    public static IProvider CreateProvider(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        var folder = Settings.SelectNode("DataCommander/Providers", true);
        folder = folder.ChildNodes[name];
        var attributes = folder.Attributes;
        var typeName = attributes["TypeName"].GetValue<string>();
        var type = Type.GetType(typeName, true);
        var instance = Activator.CreateInstance(type);
        ArgumentNullException.ThrowIfNull(instance);
        Assert.IsTrue(instance is IProvider);
        var provider = (IProvider)instance;
        ArgumentNullException.ThrowIfNull(provider);
        return provider;
    }

    public static string[] GetKeyWords(string connectionString)
    {
        string[] keyWords = null;

        try
        {
            var connection = new ConnectionClass();

            connection.Open(connectionString, null, null, 0);
            var schema = connection.OpenSchema(SchemaEnum.adSchemaDBInfoKeywords, Type.Missing, Type.Missing);
            var dataTable = OleDbHelper.Convert(schema);
            schema.Close();
            connection.Close();

            keyWords = new string[dataTable.Rows.Count];

            for (var i = 0; i < dataTable.Rows.Count; i++)
                keyWords[i] = (string)dataTable.Rows[i][0];
        }
        catch
        {
        }

        return keyWords;
    }
}