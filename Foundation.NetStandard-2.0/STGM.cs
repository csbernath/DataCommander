using System;

namespace Foundation
{
    [Flags]
    internal enum Stgm
    {
        Direct = 0x00000000,
        Transacted = 0x00010000,
        Simple = 0x08000000,
        Read = 0x00000000,
        Write = 0x00000001,
        Readwrite = 0x00000002,
        ShareDenyNone = 0x00000040,
        ShareDenyRead = 0x00000030,
        ShareDenyWrite = 0x00000020,
        ShareExclusive = 0x00000010,
        Priority = 0x00040000,
        Deleteonrelease = 0x04000000,
        Noscratch = 0x00100000,
        Create = 0x00001000,
        Convert = 0x00020000,
        Failifthere = 0x00000000,
        Nosnapshot = 0x00200000,
        DirectSwmr = 0x00400000,
    }
}