namespace DataCommander.Foundation.Configuration
{
    using System;
    using System.IO;

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
        public static ConfigurationNode Read( TextReader reader )
        {
            var node = new ConfigurationNode( null );
            ConfigurationNode currentNode = node;

            while (reader.Peek() != -1)
            {
                String line = reader.ReadLine();

                if (!String.IsNullOrEmpty( line ))
                {
                    if (line[ 0 ] == '[')
                    {
                        Int32 index = line.IndexOf( ']' );
                        String name = line.Substring( 1, index - 1 );
                        ConfigurationNode childNode = new ConfigurationNode( name );
                        node.AddChildNode( childNode );
                        currentNode = childNode;
                    }
                    else
                    {
                        Int32 index = line.IndexOf( '=' );

                        if (index >= 0)
                        {
                            String name = line.Substring( 0, index );
                            Int32 length = line.Length - index - 1;
                            String value = line.Substring( index + 1, length );
                            currentNode.Attributes.Add( new ConfigurationAttribute( name, value, null ) );
                        }
                    }
                }
            }

            return node;
        }
    }
}