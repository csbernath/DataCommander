namespace Binarit.Foundation.DocumentProperties
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout( LayoutKind.Sequential, Pack = 4 )]
    internal struct STATPROPSETSTG
    {
        public Guid fmtid;
        public Guid clsid;
        [ComAliasName( "Microsoft.VisualStudio.OLE.Interop.DWORD" )]
        public uint grfFlags;
        [ComAliasName( "Microsoft.VisualStudio.OLE.Interop.FILETIME" )]
        public System.Runtime.InteropServices.ComTypes.FILETIME mtime;
        [ComAliasName( "Microsoft.VisualStudio.OLE.Interop.FILETIME" )]
        public System.Runtime.InteropServices.ComTypes.FILETIME ctime;
        [ComAliasName( "Microsoft.VisualStudio.OLE.Interop.FILETIME" )]
        public System.Runtime.InteropServices.ComTypes.FILETIME atime;
        [ComAliasName( "Microsoft.VisualStudio.OLE.Interop.DWORD" )]
        public uint dwOSVersion;
    }
}