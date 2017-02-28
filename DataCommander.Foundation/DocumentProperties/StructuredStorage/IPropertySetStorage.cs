namespace Binarit.Foundation.DocumentProperties
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [ComImport, InterfaceType( (short) 1 ), Guid( "0000013A-0000-0000-C000-000000000046" )]
    internal interface IPropertySetStorage
    {
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void Create( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.REFFMTID" )] ref Guid rfmtid, [In] ref Guid pClsid, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.DWORD" )] uint grfFlags, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.DWORD" )] uint grfMode, [MarshalAs( UnmanagedType.Interface )] out IPropertyStorage ppprstg );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void Open( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.REFFMTID" )] ref Guid rfmtid, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.DWORD" )] uint grfMode, [MarshalAs( UnmanagedType.Interface )] out IPropertyStorage ppprstg );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void Delete( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.REFFMTID" )] ref Guid rfmtid );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void Enum( [MarshalAs( UnmanagedType.Interface )] out IEnumSTATPROPSETSTG ppEnum );
    }
}