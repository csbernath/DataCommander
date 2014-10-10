namespace Binarit.Foundation.DocumentProperties
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [ComImport, InterfaceType( (short) 1 ), Guid( "0000013B-0000-0000-C000-000000000046" )]
    internal interface IEnumSTATPROPSETSTG
    {
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        Int32 Next( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.ULONG" )] uint celt, [Out, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.STATPROPSETSTG" ), MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 0 )] STATPROPSETSTG[] rgelt, [ComAliasName( "Microsoft.VisualStudio.OLE.Interop.ULONG" )] out uint pceltFetched );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        Int32 Skip( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.ULONG" )] uint celt );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        Int32 Reset();
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void Clone( [MarshalAs( UnmanagedType.Interface )] out IEnumSTATPROPSETSTG ppEnum );
    }
}