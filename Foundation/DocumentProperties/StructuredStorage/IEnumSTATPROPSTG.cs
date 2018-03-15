namespace Foundation.DocumentProperties
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [ComImport, Guid( "00000139-0000-0000-C000-000000000046" ), InterfaceType( (short) 1 )]
    internal interface IEnumSTATPROPSTG
    {
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        Int32 Next( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.ULONG" )] uint celt, [Out, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.STATPROPSTG" ), MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 0 )] STATPROPSTG[] rgelt, [ComAliasName( "Microsoft.VisualStudio.OLE.Interop.ULONG" )] out uint pceltFetched );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        Int32 Skip( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.ULONG" )] uint celt );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        Int32 Reset();
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void Clone( [MarshalAs( UnmanagedType.Interface )] out IEnumSTATPROPSTG ppEnum );
    }
}