using System;
using ADODB;
using DataCommander.Api;
using Foundation.Assertions;
using Foundation.Configuration;

namespace DataCommander.Application;

public static class ProviderFactory
{
    public static IProvider CreateProvider(string providerIdentifier)
    {
        ArgumentNullException.ThrowIfNull(providerIdentifier);
        var folder = Settings.SelectNode("DataCommander/Providers", true);
        folder = folder.ChildNodes[providerIdentifier];
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