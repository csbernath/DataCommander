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
        string path = GetPath();
        IEnumerable<ConnectionInfo> connectionInfos;
        if (Path.Exists(path))
        {
            IEnumerable<ConnectionDto> connectionDtos;            
            using (StreamReader streamReader = File.OpenText(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                JsonTextReader jsonReader = new JsonTextReader(streamReader);
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
        IEnumerable<ConnectionDto> connectionDtos = connectionInfos
            .Select(connectionProperties => connectionProperties.ToConnectionDto());
        string path = GetPath();
        using (StreamWriter streamWriter = new StreamWriter(path, false, Encoding.UTF8))
        {
            JsonSerializer serializer = new JsonSerializer
            {
                Formatting = Formatting.Indented
            };
            serializer.Serialize(streamWriter, connectionDtos);
        }
    }

    private static string GetPath()
    {
        string applicationDataFolderPath = ApplicationData.GetApplicationDataFolderPath(false);
        string path = applicationDataFolderPath + Path.DirectorySeparatorChar + "ConnectionInfoRepository.json";
        return path;
    }
}