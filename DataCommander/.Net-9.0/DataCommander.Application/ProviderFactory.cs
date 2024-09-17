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
        ConfigurationNode folder = Settings.SelectNode("DataCommander/Providers", true);
        folder = folder.ChildNodes[providerIdentifier];
        ConfigurationAttributeCollection attributes = folder.Attributes;
        string typeName = attributes["TypeName"].GetValue<string>();
        Type? type = Type.GetType(typeName, true);
        object? instance = Activator.CreateInstance(type);
        ArgumentNullException.ThrowIfNull(instance);
        Assert.IsTrue(instance is IProvider);
        IProvider provider = (IProvider)instance;
        ArgumentNullException.ThrowIfNull(provider);
        return provider;
    }

    public static string[] GetKeyWords(string connectionString)
    {
        string[] keyWords = null;

        try
        {
            ConnectionClass connection = new ConnectionClass();

            connection.Open(connectionString, null, null, 0);
            Recordset schema = connection.OpenSchema(SchemaEnum.adSchemaDBInfoKeywords, Type.Missing, Type.Missing);
            System.Data.DataTable dataTable = OleDbHelper.Convert(schema);
            schema.Close();
            connection.Close();

            keyWords = new string[dataTable.Rows.Count];

            for (int i = 0; i < dataTable.Rows.Count; i++)
                keyWords[i] = (string)dataTable.Rows[i][0];
        }
        catch
        {
        }

        return keyWords;
    }
}