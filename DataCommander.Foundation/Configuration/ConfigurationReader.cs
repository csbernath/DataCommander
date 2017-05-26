using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using Foundation.Diagnostics;
using Foundation.Diagnostics.Log;
using Foundation.Linq;
using Foundation.Text;
using Foundation.Xml;

namespace Foundation.Configuration
{
    /// <summary>
    /// config file section reader.
    /// Reads nodes and attributes in the DataCommander.Foundation.Configuration section.
    /// </summary>
    public sealed class ConfigurationReader
    {
        private static readonly ILog Log = InternalLogFactory.Instance.GetTypeLog(typeof(ConfigurationReader));
        private string _fileName;
        private string _sectionName;
        private XmlReader _xmlReader;
        private IFormatProvider _formatProvider;
        private readonly ErrorCollection _errors = new ErrorCollection();
        private bool _enableFileSystemWatcher;

        private static Stream OpenStream(string configFileName)
        {
            Log.Trace("ConfigurationReader.OpenStream({0})...", configFileName);
            Stream stream = null;

            if (true)
            {
                var count = 0;

                while (true)
                {
                    try
                    {
                        if (File.Exists(configFileName))
                            stream = File.OpenRead(configFileName);
                        else
                            Log.Trace("{0} not found.", configFileName);

                        break;
                    }
                    catch (FileNotFoundException e)
                    {
                        Log.Trace(e.ToLogString());
                        break;
                    }
                    catch (Exception e)
                    {
                        if (count == 3)
                            throw;

                        Log.Write(LogLevel.Warning, e.ToLogString());
                        Thread.Sleep(200);
                        count++;
                    }
                }
            }

            return stream;
        }

        private static Type GetType(string typeName)
        {
            Type type;

            if (typeName == null)
                type = typeof(string);
            else
            {
                type = Type.GetType(typeName);

                if (type == null)
                    type = TypeNameCollection.GetType(typeName);
            }

            return type;
        }

        /// <summary>
        /// Finds the section DataCommander.Foundation.Configuration in the config file.
        /// </summary>
        /// <returns></returns>
        private bool FindSection(string sectionName)
        {
            var found = false;

            while (this._xmlReader.Read())
            {
                if (this._xmlReader.NodeType == XmlNodeType.Element)
                {
                    if (this._xmlReader.Name == sectionName)
                    {
                        found = true;
                        break;
                    }
                }
            }

            return found;
        }

        private object ReadAttributeValueArray(Type elementType)
        {
            var list = new List<object>();
            var go = !this._xmlReader.IsEmptyElement;

            while (go && this._xmlReader.Read())
            {
                switch (this._xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                    {
                        var valueStr = this._xmlReader["value"];
                        var value = Convert.ChangeType(valueStr, elementType, this._formatProvider);
                        list.Add(value);
                    }

                        break;

                    case XmlNodeType.EndElement:
                        go = false;
                        break;

                    default:
                        break;
                }
            }

            var array = Array.CreateInstance(elementType, list.Count);
            var values = (object[]) array;
            list.CopyTo(values);

            return array;
        }

        private object ReadAttributeValue(Type type)
        {
            var typeCode = Type.GetTypeCode(type);
            object value = null;
            this._xmlReader.MoveToContent();

            if (type.IsArray)
            {
                var elementType = type.GetElementType();
                typeCode = Type.GetTypeCode(elementType);

                if (typeCode == TypeCode.Byte)
                {
                    var innerXml = this._xmlReader.ReadInnerXml();
                    value = System.Convert.FromBase64String(innerXml);
                }
                else
                    value = this.ReadAttributeValueArray(elementType);
            }
            else if (typeCode == TypeCode.String)
            {
                if (this._xmlReader.IsEmptyElement)
                    this.AddError(ErrorType.Warning, "value attribute not found", null);
                else
                {
                    while (this._xmlReader.Read())
                    {
                        var isBreakable = false;
                        Trace.WriteLine(this._xmlReader.NodeType);

                        switch (this._xmlReader.NodeType)
                        {
                            case XmlNodeType.CDATA:
                                value = this._xmlReader.Value;
                                break;

                            case XmlNodeType.EndElement:
                                isBreakable = true;
                                break;
                        }

                        if (isBreakable)
                        {
                            break;
                        }
                    }

                    // TODO ha nem CDATA, akkor:  value = xmlReader.ReadInnerXml();
                }
            }
            else if (type == typeof(XmlNode))
            {
                var innerXml = this._xmlReader.ReadInnerXml();
                var document = new XmlDocument();
                document.LoadXml(innerXml);
                value = document.DocumentElement;
            }
            else
            {
                var innerXml = this._xmlReader.ReadInnerXml();
                value = XmlSerializerHelper.Deserialize(innerXml, type);
            }

            return value;
        }

        private void AddError(ErrorType errorType, string message, Exception e)
        {
            string message2 = null;
            var lineInfo = this._xmlReader as IXmlLineInfo;

            if (lineInfo != null && lineInfo.HasLineInfo())
            {
                message2 +=
                    "LineNumber: " + lineInfo.LineNumber + Environment.NewLine +
                    "LinePosition: " + lineInfo.LinePosition + Environment.NewLine;
            }

            message2 += "Error: " + message;

            this._errors.Add(new Error(errorType, message2, e));
        }

        private void ReadAttribute(ConfigurationNode node)
        {
            ConfigurationAttribute attribute = null;
            var name = this._xmlReader["name"];
            object value = null;

            try
            {
                if (name == null)
                    this.AddError(ErrorType.Warning, "name attribute not found", null);

                var typeName = this._xmlReader["type"];
                var type = GetType(typeName);

                if (type != null)
                {
                    var isNullStr = this._xmlReader["isNull"];
                    var isNull = false;
                    var description = this._xmlReader["description"];

                    if (isNullStr != null)
                    {
                        try
                        {
                            isNull = bool.Parse(isNullStr);
                        }
                        catch (Exception e)
                        {
                            this.AddError(ErrorType.Error, "Error parsing isNull attribute.", e);
                        }
                    }

                    if (!isNull)
                    {
                        var valueStr = this._xmlReader["value"];

                        try
                        {
                            if (valueStr != null)
                                value = Convert.ChangeType(valueStr, type, this._formatProvider);
                            else
                                value = this.ReadAttributeValue(type);
                        }
                        catch (Exception e)
                        {
                            this.AddError(ErrorType.Error, "Reading attribute value failed.", e);
                        }
                    }

                    attribute = new ConfigurationAttribute(name, value, description);
                }
                else
                    this.AddError(ErrorType.Warning, "Parsing attribute type failed.", null);
            }
            catch (Exception e)
            {
                this.AddError(ErrorType.Error, "Reading attribute failed.", e);
            }

            if (attribute != null)
            {
                try
                {
                    node.Attributes.Add(attribute);
                }
                catch (Exception e)
                {
                    this.AddError(ErrorType.Error, "Adding attribute to node failed.", e);
                }
            }
        }

        private void Read(ConfigurationNode node, StringCollection fileNames)
        {
            var name = node.Name;
            var endOfNode = this._xmlReader.IsEmptyElement;

            if (name != null)
            {
                var hasNext = this._xmlReader.MoveToFirstAttribute();

                while (hasNext)
                {
                    var attributeName = this._xmlReader.Name;
                    var attributeValue = this._xmlReader.Value;

                    if (attributeName == "name")
                    {
                    }
                    else if (attributeName == "description")
                    {
                        node.Description = attributeValue;
                    }
                    else
                    {
                        var attribute = new ConfigurationAttribute(attributeName, attributeValue, null);
                        node.Attributes.Add(attribute);
                    }

                    hasNext = this._xmlReader.MoveToNextAttribute();
                }
            }

            while (!endOfNode && this._xmlReader.Read())
            {
                switch (this._xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                    {
                        var elementName = this._xmlReader.Name;

                        switch (elementName)
                        {
                            case ConfigurationElementName.Attribute:
                                this.ReadAttribute(node);
                                break;

                            case ConfigurationElementName.Node:
                            {
                                var nodeName = this._xmlReader.GetAttribute("name");
                                var childNode = new ConfigurationNode(nodeName);
                                node.AddChildNode(childNode);
                                this.Read(childNode, fileNames);
                            }

                                break;

                            case "include":
                            {
                                var fileName = this._xmlReader.GetAttribute("fileName");
                                fileName = Environment.ExpandEnvironmentVariables(fileName);

                                var reader2 = new ConfigurationReader();
                                var includeNode = reader2.Read(fileName, this._sectionName, fileNames);
                                node.Attributes.Add(includeNode.Attributes);

                                foreach (var childNode in includeNode.ChildNodes)
                                {
                                    node.AddChildNode(childNode);
                                }

                                if (this._enableFileSystemWatcher && !fileNames.Contains(fileName))
                                {
                                    fileNames.Add(fileName);
                                }
                            }

                                break;

                            default:
                            {
                                var nodeName = XmlConvert.DecodeName(elementName);
                                var childNode = new ConfigurationNode(nodeName);
                                node.AddChildNode(childNode);
                                this.Read(childNode, fileNames);
                            }

                                break;
                        }
                    }

                        break;

                    case XmlNodeType.EndElement:
                        endOfNode = true;
                        break;

                    default:
                        break;
                }
            }
        }

        private void InitCultureInfo()
        {
            var cultureInfo = this._xmlReader["cultureInfo"];

            if (cultureInfo != null)
            {
                try
                {
                    try
                    {
                        var culture = int.Parse(cultureInfo, CultureInfo.InvariantCulture);
                        this._formatProvider = new CultureInfo(culture);
                    }
                    catch
                    {
                        this._formatProvider = new CultureInfo(cultureInfo);
                    }
                }
                catch
                {
                    this.AddError(ErrorType.Error, "Invalid cultureInfo attribute.", null);
                    this._formatProvider = CultureInfo.InvariantCulture;
                }
            }
            else
            {
                this._formatProvider = CultureInfo.InvariantCulture;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlReader"></param>
        /// <param name="configFilename"></param>
        /// <param name="sectionName"></param>
        /// <param name="fileNames"></param>
        /// <returns></returns>
        public ConfigurationNode Read(
            XmlReader xmlReader,
            string configFilename,
            string sectionName,
            StringCollection fileNames)
        {
            Log.Trace("ConfigurationReader.Read({0},{1})...", configFilename, sectionName);
            var startTick = Stopwatch.GetTimestamp();
            this._xmlReader = xmlReader;
            this._fileName = configFilename;
            this._sectionName = sectionName;
            ConfigurationNode node = null;
            string message = null;

            try
            {
                var found = this.FindSection(sectionName);

                if (found)
                {
                    var nodeType = xmlReader.MoveToContent();

                    if (nodeType == XmlNodeType.Element)
                    {
                        this.InitCultureInfo();
                        this._enableFileSystemWatcher = StringHelper.ParseBoolean(xmlReader["enableFileSystemWatcher"], false);
                        node = new ConfigurationNode(null);
                        this.Read(node, fileNames);

                        if (this._enableFileSystemWatcher && fileNames != null && !fileNames.Contains(configFilename))
                            fileNames.Add(configFilename);
                    }
                    else
                    {
                        message = $"RootNode not found. fileName: {this._fileName}, sectionName: {sectionName}";
                        this.AddError(ErrorType.Error, message, null);
                    }
                }
                else
                {
                    message = $"RootNode not found. fileName: {this._fileName}, sectionName: {sectionName}";
                    this.AddError(ErrorType.Information, message, null);
                }
            }
            catch (Exception e)
            {
                this.AddError(ErrorType.Error, null, e);
            }

            var ticks = Stopwatch.GetTimestamp() - startTick;
            message = $"{configFilename} loaded successfully in {StopwatchTimeSpan.ToString(ticks, 3)}.";
            LogLevel logLevel;
            var source = this._errors.Where(e => e.Type == ErrorType.Error);

            if (source.Any())
                logLevel = LogLevel.Error;
            else
            {
                var enumerable = this._errors.Where(e => e.Type == ErrorType.Warning);
                logLevel = enumerable.Any() ? LogLevel.Warning : LogLevel.Trace;
            }

            Log.Write(logLevel, "ConfigurationReader.Read finished.\r\nthis.errors.Count: {0}\r\n{1}", this._errors.Count, this._errors.ToString());
            return node;
        }

        /// <summary>
        /// Reads a config file into memory.
        /// </summary>
        /// <param name="configFileName">The name of the file to read</param>
        /// <param name="sectionName"></param>
        /// <param name="fileNames"></param>
        /// <returns></returns>
        public ConfigurationNode Read(string configFileName, string sectionName, StringCollection fileNames)
        {
            ConfigurationNode node = null;

            using (var stream = OpenStream(configFileName))
            {
                if (stream != null)
                {
                    var xmlTextReader = new XmlTextReader(stream);
                    node = this.Read(xmlTextReader, configFileName, sectionName, fileNames);
                }
            }

            return node;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlReader"></param>
        /// <returns></returns>
        public ConfigurationNode Read(XmlReader xmlReader)
        {
            this._xmlReader = xmlReader;
            var node = new ConfigurationNode(null);
            var fileNames = new StringCollection();
            this.Read(node, fileNames);

            if (node.ChildNodes.Count == 1)
            {
                var childNode = node.ChildNodes[0];
                node.RemoveChildNode(childNode);
                node = childNode;
            }

            return node;
        }
    }
}