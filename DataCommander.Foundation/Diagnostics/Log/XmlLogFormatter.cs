namespace DataCommander.Foundation.Diagnostics
{
    using System;
    using System.IO;
    using System.Xml;

    internal sealed class XmlLogFormatter : ILogFormatter
    {
        private static void WriteElement(
            XmlWriter xmlWriter,
            String name,
            String value)
        {
            if (value != null)
            {
                xmlWriter.WriteElementString(name, value);
            }
        }

        private static void WriteTo(
            LogEntry entry,
            XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("logEntry");

            xmlWriter.WriteElementString("creationTime", XmlConvert.ToString(entry.CreationTime, XmlDateTimeSerializationMode.Local));
            xmlWriter.WriteElementString("logLevel", entry.LogLevel.ToString());
            WriteElement(xmlWriter, "hostName", entry.HostName);
            WriteElement(xmlWriter, "userName", entry.UserName);
            //TODO
            //MethodBase method = entry.Method;
            //Type type = method.DeclaringType;
            //String typeName = type.FullName;
            //xmlWriter.WriteElementString("type", typeName);
            //xmlWriter.WriteElementString("method", entry.Method.ToString());
            //xmlWriter.WriteElementString("message", entry.Message);

            //xmlWriter.WriteEndElement();
            //xmlWriter.WriteRaw(Environment.NewLine);
        }

        String ILogFormatter.Begin()
        {
            return "<logEntries>\r\n";
        }

        String ILogFormatter.Format(LogEntry entry)
        {
            StringWriter textWriter = new StringWriter();
            var xmlTextWriter = new XmlTextWriter(textWriter) { Formatting = Formatting.Indented, Indentation = 2, IndentChar = ' ' };
            WriteTo(entry, xmlTextWriter);
            return textWriter.ToString();
        }

        String ILogFormatter.End()
        {
            return "</logEntries>\r\n";
        }
    }
}