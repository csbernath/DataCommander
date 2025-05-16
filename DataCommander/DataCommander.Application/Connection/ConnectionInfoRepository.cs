using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Foundation.Configuration;
using Foundation.Log;
using Newtonsoft.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace DataCommander.Application.Connection;

public static class ConnectionInfoRepository
{
    private static readonly ILog Log = LogFactory.Instance.GetCurrentTypeLog();

    public static IEnumerable<ConnectionInfo> Get()
    {
        var path = GetPath();
        IEnumerable<ConnectionInfo> connectionInfos;
        if (Path.Exists(path))
        {
            IEnumerable<ConnectionDto> connectionDtos;            
            using (var streamReader = File.OpenText(path))
            {
                var serializer = new JsonSerializer();
                var jsonReader = new JsonTextReader(streamReader);
                connectionDtos = serializer.Deserialize<ConnectionDto[]>(jsonReader)!;
            }

            connectionInfos = connectionDtos.Select(connectionDto => connectionDto.ToConnectionProperties());
        }
        else
            connectionInfos = [];

        return connectionInfos;
    }

    public static void Save(IEnumerable<ConnectionInfo> connectionInfos)
    {
        var connectionInfoArray = connectionInfos.ToArray();
        
        SaveWithNewtonsoft(connectionInfoArray);
        
        var connectionDtos = connectionInfoArray
            .Select(connectionProperties => connectionProperties.ToConnectionDto());
        var path = GetPath();
        using var fileStream = new FileStream(path + "New", FileMode.Create, FileAccess.Write, FileShare.None);
        fileStream.Write(Encoding.UTF8.Preamble);
        
        var utf8JsonWriter = new Utf8JsonWriter(fileStream, new JsonWriterOptions { Indented = true });
        System.Text.Json.JsonSerializer.Serialize(utf8JsonWriter, connectionDtos, typeof(IEnumerable<ConnectionDto>), new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            IncludeFields = true
        });
        utf8JsonWriter.Dispose();
    }

    private static void SaveWithNewtonsoft(IEnumerable<ConnectionInfo> connectionInfos)
    {
        var connectionDtos = connectionInfos
            .Select(connectionProperties => connectionProperties.ToConnectionDto());
        var path = GetPath();
        using var streamWriter = new StreamWriter(path, false, Encoding.UTF8);
        var serializer = new JsonSerializer
        {
            Formatting = Formatting.Indented
        };
        serializer.Serialize(streamWriter, connectionDtos);
    }

    private static string GetPath()
    {
        var applicationDataFolderPath = ApplicationData.GetApplicationDataFolderPath(false);
        var path = applicationDataFolderPath + Path.DirectorySeparatorChar + "ConnectionInfoRepository.json";
        return path;
    }
}