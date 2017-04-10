namespace DataCommander.Foundation.DocumentProperties
{
    using System;
    using System.IO;
    using System.IO.Packaging;
    using System.Runtime.InteropServices;
    using System.Xml;
    using System.Xml.Linq;

    /// <summary>
    /// 
    /// </summary>
    public sealed class Document
    {
        internal Document( DocumentPropertyCollection properties )
        {
#if CONTRACTS_FULL
            Contract.Requires(properties != null);
#endif

            this.Properties = properties;
        }

        /// <summary>
        /// 
        /// </summary>
        public DocumentPropertyCollection Properties { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="documentType"></param>
        /// <returns></returns>
        public static Document Open( string path, DocumentType documentType )
        {
            DocumentPropertyCollection properties;

            switch (documentType)
            {
                case DocumentType.OpenXml:
                    using (var package = Package.Open( path, FileMode.Open, FileAccess.Read ))
                    {
                        properties = GetOpenXmlPackageProperties( package );
                    }

                    break;

                case DocumentType.StucturedStorage:
                    properties = ReadFromStructuredStorage( path );
                    break;

                default:
                    throw new ArgumentException();
            }

            var document = new Document( properties );
            return document;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public static DocumentPropertyCollection GetOpenXmlPackageProperties( Package package )
        {
            var properties = new DocumentPropertyCollection();
            var packageProperties = package.PackageProperties;

            properties.Add( DocumentPropertyId.Category, packageProperties.Category );
            properties.Add( DocumentPropertyId.ContentStatus, packageProperties.ContentStatus );
            properties.Add( DocumentPropertyId.ContentType, packageProperties.ContentType );
            properties.Add( DocumentPropertyId.Created, packageProperties.Created );
            properties.Add( DocumentPropertyId.Creator, packageProperties.Creator );
            properties.Add( DocumentPropertyId.Description, packageProperties.Description );
            properties.Add( DocumentPropertyId.Identifier, packageProperties.Identifier );
            properties.Add( DocumentPropertyId.Keywords, packageProperties.Keywords );
            properties.Add( DocumentPropertyId.Language, packageProperties.Language );
            properties.Add( DocumentPropertyId.LastModifiedBy, packageProperties.LastModifiedBy );
            properties.Add( DocumentPropertyId.LastPrinted, packageProperties.LastPrinted );
            properties.Add( DocumentPropertyId.Modified, packageProperties.Modified );
            properties.Add( DocumentPropertyId.Revision, packageProperties.Revision );
            properties.Add( DocumentPropertyId.Subject, packageProperties.Subject );
            properties.Add( DocumentPropertyId.Title, packageProperties.Title );
            properties.Add( DocumentPropertyId.Version, packageProperties.Version );

            var extendedFilePropertiesPart = package.GetPart( new Uri( "/docProps/app.xml", UriKind.Relative ) );

            using (var xmlReader = XmlReader.Create( extendedFilePropertiesPart.GetStream() ))
            {
                var xDocument = XDocument.Load( xmlReader );
                XNamespace x = "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties";
                var xProperties = xDocument.Element( x + "Properties" );

                foreach (var xProperty in xProperties.Elements())
                {
                    DocumentPropertyId? id;
                    object value = null;

                    switch (xProperty.Name.LocalName)
                    {
                        case "Application":
                            id = DocumentPropertyId.Application;
                            value = xProperty.Value;
                            break;

                        case "AppVersion":
                            id = DocumentPropertyId.AppVersion;
                            value = xProperty.Value;
                            break;

                        case "Characters":
                            id = DocumentPropertyId.Characters;
                            value = XmlConvert.ToInt32( xProperty.Value );
                            break;

                        case "CharactersWithSpaces":
                            id = DocumentPropertyId.CharactersWithSpaces;
                            value = XmlConvert.ToInt32( xProperty.Value );
                            break;

                        case "Company":
                            id = DocumentPropertyId.Company;
                            value = xProperty.Value;
                            break;

                        case "DocSecurity":
                            id = DocumentPropertyId.DocSecurity;
                            value = XmlConvert.ToInt32( xProperty.Value );
                            break;

                        case "HeadingPairs":
                            id = DocumentPropertyId.HeadingPairs;
                            break;

                        case "HLinks":
                            id = null;
                            break;

                        case "HyperlinksChanged":
                            id = DocumentPropertyId.HyperLinkChanged;
                            value = XmlConvert.ToBoolean( xProperty.Value );
                            break;

                        case "Lines":
                            id = DocumentPropertyId.Lines;
                            value = XmlConvert.ToInt32( xProperty.Value );
                            break;

                        case "LinksUpToDate":
                            id = DocumentPropertyId.LinksUpToDate;
                            value = XmlConvert.ToBoolean( xProperty.Value );
                            break;

                        case "Manager":
                            id = DocumentPropertyId.Manager;
                            value = xProperty.Value;
                            break;

                        case "Pages":
                            id = DocumentPropertyId.Pages;
                            value = XmlConvert.ToInt32( xProperty.Value );
                            break;

                        case "Paragraphs":
                            id = DocumentPropertyId.Paragraphs;
                            value = XmlConvert.ToInt32( xProperty.Value );
                            break;

                        case "ScaleCrop":
                            id = DocumentPropertyId.ScaleCrop;
                            value = XmlConvert.ToBoolean( xProperty.Value );
                            break;

                        case "SharedDoc":
                            id = DocumentPropertyId.SharedDoc;
                            value = XmlConvert.ToBoolean( xProperty.Value );
                            break;

                        case "Template":
                            id = DocumentPropertyId.Template;
                            value = xProperty.Value;
                            break;

                        case "TitlesOfParts":
                            id = DocumentPropertyId.TitlesOfParts;
                            break;

                        case "TotalTime":
                            id = DocumentPropertyId.TotalTime;
                            value = TimeSpan.FromSeconds( XmlConvert.ToInt32( xProperty.Value ) );
                            break;

                        case "Words":
                            id = DocumentPropertyId.Words;
                            value = XmlConvert.ToInt32( xProperty.Value );
                            break;

                        default:
                            id = null;
                            break;
                    }

                    if (id != null)
                    {
                        properties.Add( id.Value, value );
                    }
                }
            }

            return properties;
        }

        private static DocumentPropertyCollection ReadFromStructuredStorage( string path )
        {
            IStorage storage;
            var result = NativeMethods.StgOpenStorage( path, null, STGM.READ | STGM.SHARE_DENY_WRITE, IntPtr.Zero, 0, out storage );

            if (result != 0)
            {
                var e = Marshal.GetExceptionForHR( result );
                throw e;
            }

            var propertySetStorage = storage as IPropertySetStorage;
            var properties = new DocumentPropertyCollection();

            foreach (var statPropSetStg in propertySetStorage.AsEnumerable())
            {
                IPropertyStorage propertyStorage;
                var fmtid = statPropSetStg.fmtid;
                propertySetStorage.Open( ref fmtid, (uint) ( STGM.READ | STGM.SHARE_EXCLUSIVE ), out propertyStorage );

                foreach (var statPropStg in propertyStorage.AsEnumerable())
                {
                    var propSpecArray = new PROPSPEC[1];
                    const uint PRSPEC_PROPID = 1;
                    propSpecArray[ 0 ].ulKind = PRSPEC_PROPID;
                    propSpecArray[ 0 ].unionmember = new IntPtr( statPropStg.PROPID );
                    var propVariantArray = new PropVariant[1];
                    propertyStorage.ReadMultiple( 1, propSpecArray, propVariantArray );
                    var propVariant = propVariantArray[ 0 ];
                    DocumentPropertyId id;

                    try
                    {
                        var value = propVariant.Value;

                        if (statPropSetStg.fmtid == PropertySetId.Summary)
                        {
                            switch ((StgSummaryPropertyId) statPropStg.PROPID)
                            {
                                case StgSummaryPropertyId.Author:
                                    id = DocumentPropertyId.Creator;
                                    break;

                                case StgSummaryPropertyId.CharCount:
                                    id = DocumentPropertyId.Characters;
                                    break;

                                case StgSummaryPropertyId.Comments:
                                    id = DocumentPropertyId.Description;
                                    break;

                                case StgSummaryPropertyId.CreateDate:
                                    id = DocumentPropertyId.Created;
                                    break;

                                case StgSummaryPropertyId.CreatingApplicationName:
                                    id = DocumentPropertyId.Application;
                                    break;

                                case StgSummaryPropertyId.Keywords:
                                    id = DocumentPropertyId.Keywords;
                                    break;

                                case StgSummaryPropertyId.LastAuthor:
                                    id = DocumentPropertyId.LastModifiedBy;
                                    break;

                                case StgSummaryPropertyId.LastPrinted:
                                    id = DocumentPropertyId.LastPrinted;
                                    break;

                                case StgSummaryPropertyId.LastSaveDate:
                                    id = DocumentPropertyId.Modified;
                                    break;

                                case StgSummaryPropertyId.PageCount:
                                    id = DocumentPropertyId.Pages;
                                    break;

                                case StgSummaryPropertyId.RevisionNumber:
                                    id = DocumentPropertyId.Revision;
                                    break;

                                case StgSummaryPropertyId.Security:
                                    id = DocumentPropertyId.DocSecurity;
                                    break;

                                case StgSummaryPropertyId.Subject:
                                    id = DocumentPropertyId.Subject;
                                    break;

                                case StgSummaryPropertyId.Template:
                                    id = DocumentPropertyId.Template;
                                    break;

                                case StgSummaryPropertyId.Title:
                                    id = DocumentPropertyId.Title;
                                    break;

                                case StgSummaryPropertyId.TotalEditingTime:
                                    id = DocumentPropertyId.TotalTime;
                                    var dateTime = (DateTime) value;
                                    var timeSpan = dateTime - DateTime.FromFileTimeUtc( 0 );
                                    value = timeSpan;
                                    break;

                                case StgSummaryPropertyId.WordCount:
                                    id = DocumentPropertyId.Words;
                                    break;

                                default:
                                    throw new NotImplementedException();
                            }
                        }
                        else if (statPropSetStg.fmtid == PropertySetId.DocumentSummary)
                        {
                            id = DocumentPropertyId.None;

                            switch ((StgDocumentSummaryPropertyId) statPropStg.PROPID)
                            {
                                case StgDocumentSummaryPropertyId.ByteCount:
                                    break;

                                case StgDocumentSummaryPropertyId.Category:
                                    id = DocumentPropertyId.Category;
                                    break;

                                case StgDocumentSummaryPropertyId.Company:
                                    id = DocumentPropertyId.Company;
                                    break;

                                case StgDocumentSummaryPropertyId.HeadingPairs:
                                    break;

                                case StgDocumentSummaryPropertyId.HiddenSlideCount:
                                    break;

                                case StgDocumentSummaryPropertyId.LineCount:
                                    id = DocumentPropertyId.Lines;
                                    break;

                                case StgDocumentSummaryPropertyId.LinksUpToDate:
                                    id = DocumentPropertyId.LinksUpToDate;
                                    break;

                                case StgDocumentSummaryPropertyId.Manager:
                                    id = DocumentPropertyId.Manager;
                                    break;

                                case StgDocumentSummaryPropertyId.MMClipCount:
                                    break;

                                case StgDocumentSummaryPropertyId.NoteCount:
                                    break;

                                case StgDocumentSummaryPropertyId.ParagraphCount:
                                    id = DocumentPropertyId.Paragraphs;
                                    break;

                                case StgDocumentSummaryPropertyId.PresentationTarget:
                                    break;

                                case StgDocumentSummaryPropertyId.ScaleCrop:
                                    id = DocumentPropertyId.ScaleCrop;
                                    break;

                                case StgDocumentSummaryPropertyId.SlideCount:
                                    break;

                                case StgDocumentSummaryPropertyId.TitlesofParts:
                                    break;

                                default:
                                    break;
                            }
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }

                        if (id != DocumentPropertyId.None)
                        {
                            properties.Add( id, value );
                        }
                    }
                    catch
                    {
                    }
                }
            }

            return properties;
        }
    }
}