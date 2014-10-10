namespace Binarit.Foundation.DocumentProperties
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [ComImport, InterfaceType( (short) 1 ), Guid( "00000138-0000-0000-C000-000000000046" )]
    internal interface IPropertyStorage
    {
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void ReadMultiple( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.ULONG" )] uint cpspec, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.PROPSPEC" ), MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 0 )] PROPSPEC[] rgpspec, [Out, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.PROPVARIANT" ), MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 0 )] PropVariant[] rgpropvar );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void WriteMultiple( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.ULONG" )] uint cpspec, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.PROPSPEC" ), MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 0 )] PROPSPEC[] rgpspec, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.PROPVARIANT" ), MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 0 )] PropVariant[] rgpropvar, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.PROPID" )] uint propidNameFirst );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void DeleteMultiple( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.ULONG" )] uint cpspec, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.PROPSPEC" ), MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 0 )] PROPSPEC[] rgpspec );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void ReadPropertyNames( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.ULONG" )] uint cpropid, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.PROPID" ), MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 0 )] uint[] rgpropid, [Out, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPOLESTR" ), MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 0 )] String[] rglpwstrName );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void WritePropertyNames( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.ULONG" )] uint cpropid, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.PROPID" ), MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 0 )] uint[] rgpropid, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPOLESTR" ), MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 0 )] String[] rglpwstrName );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void DeletePropertyNames( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.ULONG" )] uint cpropid, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.PROPID" ), MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 0 )] uint[] rgpropid );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void Commit( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.DWORD" )] uint grfCommitFlags );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void Revert();
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void Enum( [MarshalAs( UnmanagedType.Interface )] out IEnumSTATPROPSTG ppEnum );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void SetTimes( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.FILETIME" ), MarshalAs( UnmanagedType.LPArray )] System.Runtime.InteropServices.ComTypes.FILETIME[] pctime, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.FILETIME" ), MarshalAs( UnmanagedType.LPArray )] System.Runtime.InteropServices.ComTypes.FILETIME[] patime, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.FILETIME" ), MarshalAs( UnmanagedType.LPArray )] System.Runtime.InteropServices.ComTypes.FILETIME[] pmtime );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void SetClass( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.REFCLSID" )] ref Guid clsid );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void Stat( [Out, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.STATPROPSETSTG" ), MarshalAs( UnmanagedType.LPArray )] STATPROPSETSTG[] pstatpsstg );
    }
}