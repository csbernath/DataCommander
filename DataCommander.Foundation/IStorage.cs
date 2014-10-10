namespace DataCommander.Foundation
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using CT = System.Runtime.InteropServices.ComTypes;

    [ComImport, InterfaceType( (short) 1 ), ComConversionLoss, Guid( "0000000B-0000-0000-C000-000000000046" )]
    internal interface IStorage
    {
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void CreateStream( [In, MarshalAs( UnmanagedType.LPWStr )] String pwcsName, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.DWORD" )] uint grfMode, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.DWORD" )] uint reserved1, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.DWORD" )] uint reserved2, [MarshalAs( UnmanagedType.Interface )] out CT.IStream ppstm );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void OpenStream( [In, MarshalAs( UnmanagedType.LPWStr )] String pwcsName, [In] IntPtr reserved1, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.DWORD" )] uint grfMode, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.DWORD" )] uint reserved2, [MarshalAs( UnmanagedType.Interface )] out CT.IStream ppstm );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void CreateStorage( [In, MarshalAs( UnmanagedType.LPWStr )] String pwcsName, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.DWORD" )] uint grfMode, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.DWORD" )] uint reserved1, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.DWORD" )] uint reserved2, [MarshalAs( UnmanagedType.Interface )] out IStorage ppstg );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void OpenStorage( [In, MarshalAs( UnmanagedType.LPWStr )] String pwcsName, [In, MarshalAs( UnmanagedType.Interface )] IStorage pstgPriority, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.DWORD" )] uint grfMode, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.SNB" )] IntPtr snbExclude, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.DWORD" )] uint reserved, [MarshalAs( UnmanagedType.Interface )] out IStorage ppstg );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void CopyTo( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.DWORD" )] uint ciidExclude, [In, MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 0 )] Guid[] rgiidExclude, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.SNB" )] IntPtr snbExclude, [In, MarshalAs( UnmanagedType.Interface )] IStorage pstgDest );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void MoveElementTo( [In, MarshalAs( UnmanagedType.LPWStr )] String pwcsName, [In, MarshalAs( UnmanagedType.Interface )] IStorage pstgDest, [In, MarshalAs( UnmanagedType.LPWStr )] String pwcsNewName, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.DWORD" )] uint grfFlags );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void Commit( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.DWORD" )] uint grfCommitFlags );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void Revert();
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void EnumElements( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.DWORD" )] uint reserved1, [In] IntPtr reserved2, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.DWORD" )] uint reserved3, [MarshalAs( UnmanagedType.Interface )] out IEnumSTATSTG ppEnum );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void DestroyElement( [In, MarshalAs( UnmanagedType.LPWStr )] String pwcsName );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void RenameElement( [In, MarshalAs( UnmanagedType.LPWStr )] String pwcsOldName, [In, MarshalAs( UnmanagedType.LPWStr )] String pwcsNewName );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void SetElementTimes( [In, MarshalAs( UnmanagedType.LPWStr )] String pwcsName, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.FILETIME" ), MarshalAs( UnmanagedType.LPArray )] CT.FILETIME[] pctime, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.FILETIME" ), MarshalAs( UnmanagedType.LPArray )] CT.FILETIME[] patime, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.FILETIME" ), MarshalAs( UnmanagedType.LPArray )] CT.FILETIME[] pmtime );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void SetClass( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.REFCLSID" )] ref Guid clsid );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void SetStateBits( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.DWORD" )] uint grfStateBits, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.DWORD" )] uint grfMask );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void Stat( [Out, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.STATSTG" ), MarshalAs( UnmanagedType.LPArray )] CT.STATSTG[] pstatstg, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.DWORD" )] uint grfStatFlag );
    }
}
