namespace DataCommander.Foundation.Xml
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Xml;

    /// <summary>
    /// 
    /// </summary>
    public sealed class SimpleXmlTextWriter : XmlWriter
    {
        private IndentedTextWriter textWriter;
        private readonly Stack<StackItem> stack = new Stack<StackItem>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleXmlTextWriter"/> class.
        /// </summary>
        /// <param name="textWriter">The text writer.</param>
        public SimpleXmlTextWriter(TextWriter textWriter)
        {
            this.textWriter = new IndentedTextWriter(textWriter, "    ");
        }

        /// <summary>
        /// When overridden in a derived class, closes this stream and the underlying stream.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">A call is made to write more output after <see langword="Close"/> has been called or the result of this call is an invalid XML document.</exception>
        public override void Close()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When overridden in a derived class, flushes whatever is in the buffer to the underlying streams and also flushes the
        /// underlying stream.
        /// </summary>
        public override void Flush()
        {
            this.textWriter.Flush();
        }

        /// <summary>
        /// When overridden in a derived class, returns the closest prefix defined in the
        /// current namespace scope for the namespace URI.
        /// </summary>
        /// <param name="ns">The namespace URI whose prefix you want to find.</param>
        /// <returns>
        /// The matching prefix or <see langword="null"/> if no matching namespace URI is found in the current scope.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="ns"/> is either <see langword="null"/> or String.Empty.</exception>
        public override String LookupPrefix(String ns)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When overridden in a derived class, writes out all the attributes found at the
        /// current position in the <see cref="T:System.Xml.XmlReader"/>
        /// .
        /// </summary>
        /// <param name="reader">The <see langword="XmlReader"/> from which to copy the attributes.</param>
        /// <param name="defattr"><see langword="true"/> to copy the default attributes from the <see langword="XmlReader"/> ;otherwise, <see langword="false"/>.</param>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="reader"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.Xml.XmlException"> The reader is not positioned on an element, attribute or XmlDeclaration node.</exception>
        public override void WriteAttributes(XmlReader reader, Boolean defattr)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When overridden in a derived class, encodes the specified binary bytes as base64 and writes out
        /// the resulting text.
        /// </summary>
        /// <param name="buffer">Byte array to encode.</param>
        /// <param name="index">The position in the buffer indicating the start of the bytes to write.</param>
        /// <param name="count">The number of bytes to write.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="buffer"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentException">The buffer length minus <paramref name="index"/> is less than <paramref name="count"/>.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> or <paramref name="count"/> is less than zero.</exception>
        public override void WriteBase64(Byte[] buffer, Int32 index, Int32 count)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When overridden in a derived class, encodes the specified binary bytes as binhex and writes out
        /// the resulting text.
        /// </summary>
        /// <param name="buffer">Byte array to encode.</param>
        /// <param name="index">The position in the buffer indicating the start of the bytes to write.</param>
        /// <param name="count">The number of bytes to write.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="buffer"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentException">The buffer length minus <paramref name="index"/> is less than <paramref name="count"/>.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> or <paramref name="count"/> is less than zero.</exception>
        public override void WriteBinHex(Byte[] buffer, Int32 index, Int32 count)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When overridden in a derived class, writes out a &lt;![CDATA[...]]&gt;block containing
        /// the specified text.
        /// </summary>
        /// <param name="text">The text to place inside the CDATA block.</param>
        /// <exception cref="T:System.ArgumentException">The text would result in a non-well formed XML document.</exception>
        public override void WriteCData(String text)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When overridden in a derived class, forces the
        /// generation of a character entity for the specified Unicode character value.
        /// </summary>
        /// <param name="ch">The Unicode character for which to generate a character entity.</param>
        /// <exception cref="T:System.ArgumentException">The character is in the surrogate pair character range, <see langword="0xd800"/> - <see langword="0xdfff"/>. </exception>
        public override void WriteCharEntity(Char ch)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When overridden in a derived class, writes text one buffer at a time.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index">The position in the buffer indicating the start of the text to write.</param>
        /// <param name="count">The number of characters to write.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="buffer"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentException">The buffer length minus <paramref name="index"/> is less than <paramref name="count"/>;the call results in surrogate pair characters being split or an invalid surrogate pair being written.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> or <paramref name="count"/> is less than zero.</exception>
        public override void WriteChars(Char[] buffer, Int32 index, Int32 count)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When overridden in a derived class, writes out a comment &lt;!--...--&gt;containing
        /// the specified text.
        /// </summary>
        /// <param name="text">Text to place inside the comment.</param>
        /// <exception cref="T:System.ArgumentException">The text would result in a non-well formed XML document.</exception>
        public override void WriteComment(String text)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When overridden in a derived class, writes the DOCTYPE declaration with the specified name
        /// and optional attributes.
        /// </summary>
        /// <param name="name">The name of the DOCTYPE. This must be non-empty.</param>
        /// <param name="pubid">If non-null it also writes PUBLIC "pubid" "sysid" where pubid and sysid are replaced with the value of the given arguments.</param>
        /// <param name="sysid">If pubid is <see langword="null"/> and sysid is non-null it writes SYSTEM "sysid" where sysid is replaced with the value of this argument.</param>
        /// <param name="subset">If non-null it writes [subset] where subset is replaced with the value of this argument.</param>
        /// <exception cref="T:System.InvalidOperationException">This method was called outside the prolog (after the root element).</exception>
        /// <exception cref="T:System.ArgumentException">The value for <paramref name="name "/> would result in invalid XML.</exception>
        public override void WriteDocType(String name, String pubid, String sysid, String subset)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When overridden in a derived class, closes the previous <see cref="M:System.Xml.XmlWriter.WriteStartAttribute(System.String,System.String)"/>
        /// call.
        /// </summary>
        public override void WriteEndAttribute()
        {
            this.textWriter.Indent--;
        }

        /// <summary>
        /// When overridden in a derived class, closes any open elements or attributes and
        /// puts the writer back in the Start state.
        /// </summary>
        /// <exception cref="T:System.ArgumentException">The XML document is invalid.</exception>
        public override void WriteEndDocument()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When overridden in a derived class, closes one element and pops the corresponding namespace scope.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">This results in an invalid XML document.</exception>
        public override void WriteEndElement()
        {
            StackItem stackItem = this.stack.Pop();

            if (stackItem.HasChildNodes)
            {
                this.textWriter.Write("</");
                this.textWriter.Write(stackItem.LocalName);
                this.textWriter.WriteLine('>');
            }
            else
            {
                if (stackItem.HasAttributes)
                {
                    this.textWriter.WriteLine();
                    this.textWriter.WriteLine("/>");
                }
            }

            this.textWriter.Indent--;
        }

        /// <summary>
        /// When overridden in a derived class, writes out an entity
        /// reference as <see langword="&amp;name;"/> .
        /// </summary>
        /// <param name="name">The name of the entity reference.</param>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="name"/> is either <see langword="null"/> or String.Empty.</exception>
        public override void WriteEntityRef(String name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When overridden in a derived class, closes one element and pops the
        /// corresponding namespace scope.
        /// </summary>
        public override void WriteFullEndElement()
        {
            this.WriteEndElement();
        }

        /// <summary>
        /// When overridden in a derived class, writes out the specified name, ensuring it is a valid name according to
        /// the W3C XML 1.0 recommendation
        /// (http://www.w3.org/TR/1998/REC-xml-19980210#NT-Name).
        /// </summary>
        /// <param name="name">The name to write.</param>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="name "/>is not a valid XML name;or <paramref name="name"/> is either <see langword="null"/> or String.Empty.
        /// </exception>
        public override void WriteName(String name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When overridden in a derived class, writes out the specified name, ensuring it is a valid NmToken according to
        /// the W3C XML 1.0 recommendation (http://www.w3.org/TR/1998/REC-xml-19980210#NT-Name).
        /// </summary>
        /// <param name="name">The name to write.</param>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="name "/>is not a valid NmToken;or <paramref name="name"/> is either <see langword="null"/> or String.Empty.</exception>
        public override void WriteNmToken(String name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When overridden in a derived class, copies everything from the reader to the writer and
        /// moves the reader to the start of the next
        /// sibling.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> to read from.</param>
        /// <param name="defattr"><see langword="true"/> to copy the default attributes from the <see langword="XmlReader"/> ;otherwise, <see langword="false"/>.</param>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="reader"/> is <see langword="null"/>.</exception>
        public override void WriteNode(XmlReader reader, Boolean defattr)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When overridden in a derived class, writes out a processing instruction with a space between
        /// the name and text as follows: &lt;?name text?&gt;.
        /// </summary>
        /// <param name="name">The name of the processing instruction.</param>
        /// <param name="text"></param>
        /// <exception cref="T:System.ArgumentException">
        ///     <para> The text would result in a non-well formed XML document.</para>
        ///     <para>
        ///         <paramref name="name"/> is either <see langword="null"/> or String.Empty.</para>
        ///     <para>This method is being used to create an XML declaration after <see cref="M:System.Xml.XmlWriter.WriteStartDocument"/> has already been called. </para>
        /// </exception>
        public override void WriteProcessingInstruction(String name, String text)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When overridden in a derived class, writes out the namespace-qualified name. This method looks up the prefix
        /// that is in scope for the given namespace.
        /// </summary>
        /// <param name="localName">The local name to write.</param>
        /// <param name="ns"></param>
        /// <exception cref="T:System.ArgumentException">
        ///     <para>
        ///         <paramref name="localName"/> is either <see langword="null"/> or String.Empty.</para>
        ///     <para>
        ///         <paramref name="localName"/> is not a valid name.</para>
        /// </exception>
        public override void WriteQualifiedName(String localName, String ns)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When overridden in a derived class, writes raw markup manually from a character buffer.
        /// </summary>
        /// <param name="buffer">Character array containing the text to write.</param>
        /// <param name="index">The position within the buffer indicating the start of the text to write.</param>
        /// <param name="count">The number of characters to write.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="buffer"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentException">The buffer length minus <paramref name="index"/> is less than <paramref name="count"/>.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> or <paramref name="count"/> is less than zero.</exception>
        public override void WriteRaw(Char[] buffer, Int32 index, Int32 count)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When overridden in a derived class, writes raw markup manually from a String.
        /// </summary>
        /// <param name="data">String containing the text to write.</param>
        public override void WriteRaw(String data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When overridden in a derived class, writes the start of an attribute.
        /// </summary>
        /// <param name="prefix">The namespace prefix of the attribute.</param>
        /// <param name="localName"></param>
        /// <param name="ns"></param>
        public override void WriteStartAttribute(String prefix, String localName, String ns)
        {
            StackItem stackItem = this.stack.Peek();
            stackItem.HasAttributes = true;

            this.textWriter.WriteLine();
            this.textWriter.Indent++;
            this.textWriter.Write(localName);
            this.textWriter.Write(" = ");
        }

        /// <summary>
        /// When overridden in a derived class, writes the XML declaration with the version "1.0".
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">This is not the first write method called after the constructor.</exception>
        public override void WriteStartDocument()
        {
        }

        /// <summary>
        /// When overridden in a derived class, writes the XML declaration with the version "1.0" and the
        /// standalone attribute.
        /// </summary>
        /// <param name="standalone">If <see langword="true"/>, it writes "standalone=yes";if <see langword="false"/>, it writes "standalone=no".</param>
        /// <exception cref="T:System.InvalidOperationException">This is not the first write method called after the constructor.</exception>
        public override void WriteStartDocument(Boolean standalone)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When overridden in a derived class, writes the specified start tag and
        /// associates it with the given namespace and prefix.
        /// </summary>
        /// <param name="prefix">The namespace prefix of the element.</param>
        /// <param name="localName">The local name of the element.</param>
        /// <param name="ns">The namespace URI to associate with the element.</param>
        /// <exception cref="T:System.InvalidOperationException">The writer is closed.</exception>
        public override void WriteStartElement(String prefix, String localName, String ns)
        {
            StackItem parent = this.stack.Count > 0 ? this.stack.Peek() : null;

            if (parent != null)
            {
                if (parent.HasAttributes && !parent.HasChildNodes)
                {
                    this.textWriter.WriteLine();
                    this.textWriter.WriteLine('>');
                }
                else if (!parent.HasAttributes && !parent.HasChildNodes)
                {
                    this.textWriter.WriteLine('>');
                }

                parent.HasChildNodes = true;
                this.textWriter.Indent++;
            }

            this.textWriter.Write('<');
            this.textWriter.Write(localName);

            StackItem stackItem = new StackItem(localName);
            this.stack.Push(stackItem);
        }

        /// <summary>
        /// When overridden in a derived class, gets the state of the writer.
        /// </summary>
        /// <value></value>
        public override WriteState WriteState
        {
            get
            {
                return new WriteState();
            }
        }

        private static String Encode(Char c)
        {
            UInt16 charCode = (UInt16)c;
            String encoded = "&#x" + charCode.ToString("x", CultureInfo.InvariantCulture) + ';';
            return encoded;
        }

        /// <summary>
        /// When overridden in a derived class, writes the given text content.
        /// </summary>
        /// <param name="text">The text to write.</param>
        /// <exception cref="T:System.ArgumentException">The text String contains an invalid surrogate pair.</exception>
        public override void WriteString(String text)
        {
            this.textWriter.Write('"');

            for (Int32 i = 0; i < text.Length; i++)
            {
                Char c = text[i];

                switch (c)
                {
                    case '"':
                    case '\r':
                    case '\n':
                        String encoded = Encode(c);
                        this.textWriter.Write(encoded);
                        break;

                    default:
                        this.textWriter.Write(c);
                        break;
                }
            }

            this.textWriter.Write('"');
        }

        /// <summary>
        /// When overridden in a derived class, generates and writes the surrogate character entity
        /// for the surrogate character pair.
        /// </summary>
        /// <param name="lowChar">The low surrogate. This must be a value between 0xDC00 and 0xDFFF.</param>
        /// <param name="highChar"></param>
        /// <exception cref="T:System.Exception">An invalid surrogate character pair was passed.</exception>
        public override void WriteSurrogateCharEntity(Char lowChar, Char highChar)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When overridden in a derived class, writes out the given white space.
        /// </summary>
        /// <param name="ws">The String of white space characters.</param>
        /// <exception cref="T:System.ArgumentException">The String contains non-white space characters.</exception>
        public override void WriteWhitespace(String ws)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When overridden in a derived class, gets the current xml:lang scope.
        /// </summary>
        /// <value></value>
        public override String XmlLang
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets an <see cref="T:System.Xml.XmlSpace"/> representing the current xml:space scope.
        /// </summary>
        /// <value></value>
        public override XmlSpace XmlSpace
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        private sealed class StackItem
        {
            private String localName;
            private Boolean hasAttributes;
            private Boolean hasChildNodes;

            public StackItem(String localName)
            {
                this.localName = localName;
            }

            public String LocalName
            {
                get
                {
                    return this.localName;
                }
            }

            public Boolean HasAttributes
            {
                get
                {
                    return this.hasAttributes;
                }

                set
                {
                    this.hasAttributes = value;
                }
            }

            public Boolean HasChildNodes
            {
                get
                {
                    return this.hasChildNodes;
                }

                set
                {
                    this.hasChildNodes = value;
                }
            }
        }
    }
}