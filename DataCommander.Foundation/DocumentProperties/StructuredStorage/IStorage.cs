namespace DataCommander.Foundation.DocumentProperties
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using CT = System.Runtime.InteropServices.ComTypes;

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
        void ReadPropertyNames( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.ULONG" )] uint cpropid, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.PROPID" ), MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 0 )] uint[] rgpropid, [Out, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPOLESTR" ), MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 0 )] string[] rglpwstrName );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void WritePropertyNames( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.ULONG" )] uint cpropid, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.PROPID" ), MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 0 )] uint[] rgpropid, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPOLESTR" ), MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 0 )] string[] rglpwstrName );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void DeletePropertyNames( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.ULONG" )] uint cpropid, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.PROPID" ), MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 0 )] uint[] rgpropid );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void Commit( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.DWORD" )] uint grfCommitFlags );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void Revert();
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void Enum( [MarshalAs( UnmanagedType.Interface )] out IEnumSTATPROPSTG ppEnum );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void SetTimes( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.FILETIME" ), MarshalAs( UnmanagedType.LPArray )] CT.FILETIME[] pctime, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.FILETIME" ), MarshalAs( UnmanagedType.LPArray )] CT.FILETIME[] patime, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.FILETIME" ), MarshalAs( UnmanagedType.LPArray )] CT.FILETIME[] pmtime );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void SetClass( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.REFCLSID" )] ref Guid clsid );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void Stat( [Out, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.STATPROPSETSTG" ), MarshalAs( UnmanagedType.LPArray )] STATPROPSETSTG[] pstatpsstg );
    }

    [ComImport, InterfaceType( (short) 1 ), Guid( "0000013B-0000-0000-C000-000000000046" )]
    internal interface IEnumSTATPROPSETSTG
    {
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int Next( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.ULONG" )] uint celt, [Out, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.STATPROPSETSTG" ), MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 0 )] STATPROPSETSTG[] rgelt, [ComAliasName( "Microsoft.VisualStudio.OLE.Interop.ULONG" )] out uint pceltFetched );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int Skip( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.ULONG" )] uint celt );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int Reset();
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void Clone( [MarshalAs( UnmanagedType.Interface )] out IEnumSTATPROPSETSTG ppEnum );
    }

    [StructLayout( LayoutKind.Explicit, Pack = 4 )]
    internal struct PROPSPEC
    {
        // Fields
        [ComAliasName( "Microsoft.VisualStudio.OLE.Interop.ULONG" ), FieldOffset( 0 )]
        public uint ulKind;
        [FieldOffset( 4 )]
        public IntPtr unionmember;
    }

    [ComImport, Guid( "00000139-0000-0000-C000-000000000046" ), InterfaceType( (short) 1 )]
    internal interface IEnumSTATPROPSTG
    {
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int Next( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.ULONG" )] uint celt, [Out, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.STATPROPSTG" ), MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 0 )] STATPROPSTG[] rgelt, [ComAliasName( "Microsoft.VisualStudio.OLE.Interop.ULONG" )] out uint pceltFetched );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int Skip( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.ULONG" )] uint celt );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int Reset();
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void Clone( [MarshalAs( UnmanagedType.Interface )] out IEnumSTATPROPSTG ppEnum );
    }

    [StructLayout( LayoutKind.Sequential, Pack = 4 )]
    internal struct STATPROPSETSTG
    {
        public Guid fmtid;
        public Guid clsid;
        [ComAliasName( "Microsoft.VisualStudio.OLE.Interop.DWORD" )]
        public uint grfFlags;
        [ComAliasName( "Microsoft.VisualStudio.OLE.Interop.FILETIME" )]
        public CT.FILETIME mtime;
        [ComAliasName( "Microsoft.VisualStudio.OLE.Interop.FILETIME" )]
        public CT.FILETIME ctime;
        [ComAliasName( "Microsoft.VisualStudio.OLE.Interop.FILETIME" )]
        public CT.FILETIME atime;
        [ComAliasName( "Microsoft.VisualStudio.OLE.Interop.DWORD" )]
        public uint dwOSVersion;
    }

    [StructLayout( LayoutKind.Sequential, Pack = 4 )]
    internal struct STATPROPSTG
    {
        [ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPOLESTR" ), MarshalAs( UnmanagedType.LPWStr )]
        public string lpwstrName;
        [ComAliasName( "Microsoft.VisualStudio.OLE.Interop.PROPID" )]
        public uint PROPID;
        [ComAliasName( "Microsoft.VisualStudio.OLE.Interop.VARTYPE" )]
        public ushort vt;
    }
}