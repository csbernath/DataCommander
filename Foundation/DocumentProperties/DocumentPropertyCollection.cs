using System;
using System.Collections;
using System.Collections.Generic;

namespace Foundation.DocumentProperties
{
    /// <summary>
    /// 
    /// </summary>
    public enum DocumentPropertyId
    {
        /// <summary>
        /// 
        /// </summary>
        None,

        /// <summary>
        /// 
        /// </summary>
        Category,

        /// <summary>
        /// 
        /// </summary>
        ContentStatus,

        /// <summary>
        /// 
        /// </summary>
        ContentType,

        /// <summary>
        /// 
        /// </summary>
        Created,

        /// <summary>
        /// 
        /// </summary>
        Creator,

        /// <summary>
        /// 
        /// </summary>
        Description,

        /// <summary>
        /// 
        /// </summary>
        Identifier,

        /// <summary>
        /// 
        /// </summary>
        Keywords,

        /// <summary>
        /// 
        /// </summary>
        Language,

        /// <summary>
        /// 
        /// </summary>
        LastModifiedBy,

        /// <summary>
        /// 
        /// </summary>
        LastPrinted,

        /// <summary>
        /// 
        /// </summary>
        Manager,

        /// <summary>
        /// 
        /// </summary>
        Modified,

        /// <summary>
        /// 
        /// </summary>
        Revision,

        /// <summary>
        /// 
        /// </summary>
        Subject,

        /// <summary>
        /// 
        /// </summary>
        Title,

        /// <summary>
        /// 
        /// </summary>
        Version,

        /// <summary>
        /// 
        /// </summary>
        Template,

        /// <summary>
        /// 
        /// </summary>
        TotalTime,

        /// <summary>
        /// 
        /// </summary>
        Pages,

        /// <summary>
        /// 
        /// </summary>
        Words,

        /// <summary>
        /// 
        /// </summary>
        Characters,

        /// <summary>
        /// 
        /// </summary>
        Application,

        /// <summary>
        /// 
        /// </summary>
        DocSecurity,

        /// <summary>
        /// 
        /// </summary>
        Lines,

        /// <summary>
        /// 
        /// </summary>
        Paragraphs,

        /// <summary>
        /// 
        /// </summary>
        ScaleCrop,

        /// <summary>
        /// 
        /// </summary>
        HeadingPairs,

        /// <summary>
        /// 
        /// </summary>
        TitlesOfParts,

        /// <summary>
        /// 
        /// </summary>
        Company,

        /// <summary>
        /// 
        /// </summary>
        LinksUpToDate,

        /// <summary>
        /// 
        /// </summary>
        CharactersWithSpaces,

        /// <summary>
        /// 
        /// </summary>
        SharedDoc,

        /// <summary>
        /// 
        /// </summary>
        HyperLinkChanged,

        /// <summary>
        /// 
        /// </summary>
        AppVersion
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class DocumentPropertyCollection : ICollection<DocumentProperty>
    {
        private readonly Dictionary<DocumentPropertyId, DocumentProperty> dictionary = new Dictionary<DocumentPropertyId, DocumentProperty>();

        internal DocumentPropertyCollection()
        {
        }

        internal void Add( DocumentPropertyId id, object value )
        {
            var property = new DocumentProperty( id, value );
            dictionary.Add( id, property );
        }

        #region ICollection<DocumentProperty> Members

        void ICollection<DocumentProperty>.Add( DocumentProperty item )
        {
            throw new NotImplementedException();
        }

        void ICollection<DocumentProperty>.Clear()
        {
            throw new NotImplementedException();
        }

        bool ICollection<DocumentProperty>.Contains( DocumentProperty item )
        {
            throw new NotImplementedException();
        }

        void ICollection<DocumentProperty>.CopyTo( DocumentProperty[] array, int arrayIndex )
        {
            throw new NotImplementedException();
        }

        int ICollection<DocumentProperty>.Count => throw new NotImplementedException();

        bool ICollection<DocumentProperty>.IsReadOnly => throw new NotImplementedException();

        bool ICollection<DocumentProperty>.Remove( DocumentProperty item )
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable<DocumentProperty> Members

        IEnumerator<DocumentProperty> IEnumerable<DocumentProperty>.GetEnumerator()
        {
            return dictionary.Values.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}