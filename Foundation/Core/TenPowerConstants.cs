namespace Foundation.Core;

public static class TenPowerConstants
{
    public const short TenPower3 = 1000;
    public const int TenPower6 = TenPower3 * TenPower3;
    public const int TenPower9 = TenPower3 * TenPower6;
    public const long TenPower12 = (long)TenPower3 * TenPower9;
    public const long TenPower15 = TenPower3 * TenPower12;
    public const long TenPower18 = TenPower3 * TenPower15;
}