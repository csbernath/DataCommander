using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Foundation.Configuration;
using Foundation.Log;
using Newtonsoft.Json;

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
                connectionDtos = serializer.Deserialize<ConnectionDto[]>(jsonReader);
            }

            connectionInfos = connectionDtos.Select(connectionDto => connectionDto.ToConnectionProperties());
        }
        else
            connectionInfos = [];

        return connectionInfos;
    }

    public static void Save(IEnumerable<ConnectionInfo> connectionInfos)
    {
        var connectionDtos = connectionInfos
            .Select(connectionProperties => connectionProperties.ToConnectionDto());
        var path = GetPath();
        using (var streamWriter = new StreamWriter(path, false, Encoding.UTF8))
        {
            var serializer = new JsonSerializer();
            serializer.Formatting = Formatting.Indented;
            serializer.Serialize(streamWriter, connectionDtos);
        }
    }

    private static string GetPath()
    {
        var applicationDataFolderPath = ApplicationData.GetApplicationDataFolderPath(false);
        var path = applicationDataFolderPath + Path.DirectorySeparatorChar + "ConnectionInfoRepository.json";
        return path;
    }
}