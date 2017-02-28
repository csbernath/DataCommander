namespace Binarit.Foundation.DocumentProperties
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout( LayoutKind.Sequential, Pack = 4 )]
    internal struct STATPROPSTG
    {
        [ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPOLESTR" ), MarshalAs( UnmanagedType.LPWStr )]
        public String lpwstrName;
        [ComAliasName( "Microsoft.VisualStudio.OLE.Interop.PROPID" )]
        public uint PROPID;
        [ComAliasName( "Microsoft.VisualStudio.OLE.Interop.VARTYPE" )]
        public ushort vt;
    }
}