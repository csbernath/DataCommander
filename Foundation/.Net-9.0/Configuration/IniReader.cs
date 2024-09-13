using System.IO;

namespace Foundation.Configuration;

/// <summary>
/// 
/// </summary>
public static class IniReader
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static ConfigurationNode Read(TextReader reader)
    {
        var node = new ConfigurationNode(null);
        var currentNode = node;

        while (reader.Peek() != -1)
        {
            var line = reader.ReadLine();

            if (!string.IsNullOrEmpty(line))
            {
                if (line[0] == '[')
                {
                    var index = line.IndexOf(']');
                    var name = line[1..index];
                    var childNode = new ConfigurationNode(name);
                    node.AddChildNode(childNode);
                    currentNode = childNode;
                }
                else
                {
                    var index = line.IndexOf('=');

                    if (index >= 0)
                    {
                        var name = line[..index];
                        var length = line.Length - index - 1;
                        var value = line.Substring(index + 1, length);
                        currentNode.Attributes.Add(new ConfigurationAttribute(name, value, null));
                    }
                }
            }
        }

        return node;
    }
}