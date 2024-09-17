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
        ConfigurationNode node = new ConfigurationNode(null);
        ConfigurationNode currentNode = node;

        while (reader.Peek() != -1)
        {
            string line = reader.ReadLine();

            if (!string.IsNullOrEmpty(line))
            {
                if (line[0] == '[')
                {
                    int index = line.IndexOf(']');
                    string name = line[1..index];
                    ConfigurationNode childNode = new ConfigurationNode(name);
                    node.AddChildNode(childNode);
                    currentNode = childNode;
                }
                else
                {
                    int index = line.IndexOf('=');

                    if (index >= 0)
                    {
                        string name = line[..index];
                        int length = line.Length - index - 1;
                        string value = line.Substring(index + 1, length);
                        currentNode.Attributes.Add(new ConfigurationAttribute(name, value, null));
                    }
                }
            }
        }

        return node;
    }
}