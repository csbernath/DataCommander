namespace DataCommander.Foundation
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using CT = System.Runtime.InteropServices.ComTypes;

    [ComImport, Guid( "0000000D-0000-0000-C000-000000000046" ), InterfaceType( (short) 1 )]
    internal interface IEnumSTATSTG
    {
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int Next( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.ULONG" )] uint celt, [Out, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.STATSTG" ), MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 0 )] CT.STATSTG[] rgelt, [ComAliasName( "Microsoft.VisualStudio.OLE.Interop.ULONG" )] out uint pceltFetched );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int Skip( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.ULONG" )] uint celt );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int Reset();
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void Clone( [MarshalAs( UnmanagedType.Interface )] out IEnumSTATSTG ppEnum );
    }
}
