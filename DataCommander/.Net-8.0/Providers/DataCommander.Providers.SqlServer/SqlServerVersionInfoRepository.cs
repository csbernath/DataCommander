using System.Collections.Generic;
using System.Linq;

namespace DataCommander.Providers.SqlServer;

internal static class SqlServerVersionInfoRepository
{
    private static readonly Dictionary<string, SqlServerVersionInfo> SqlServerVersionInfosByVersion = Init();

    public static bool TryGetByVersion(string version, out SqlServerVersionInfo? sqlServerVersionInfo)
    {
        return SqlServerVersionInfosByVersion.TryGetValue(version, out sqlServerVersionInfo);
    }

    private static Dictionary<string, SqlServerVersionInfo> Init()
    {
        return new SqlServerVersionInfo[]
        {
            new("08.00.0194", "SQL Server 2000 RTM"),
            new("08.00.0760", "SQL Server 2000 SP3"),
            new("08.00.0878", "SQL Server 2000 SP3a"),
            new("08.00.2039", "SQL Server 2000 SP4"),
            new("08.00.2040", "SQL Server 2000 (after SP4)"),
            new("08.00.2187", "SQL Server 2000 post SP4 hotfix build (build 2187)"),
            new("09.00.1399", "SQL Server 2005"),
            new("09.00.2047", "SQL Server 2005 (before SP1)"),
            new("09.00.2153", "SQL Server 2005 SP1"),
            new("09.00.3042", "SQL Server 2005 SP2"),
            new("09.00.3073",
                "SQL Server 2005 SP2 + 954606 MS08-052: Description of the security update for GDI+ for SQL Server 2005 Service Pack 2 GDR: September 9, 2008"),
            new("09.00.3080",
                "SQL Server 2005 SP2 + 970895 MS09-062: Description of the security update for GDI+ for SQL Server 2005 Service Pack 2 GDR: October 13, 2009"),
            new("09.00.3186", "SQL Server 2005 SP2 + cumulative update (August 20, 2007)"),
            new("09.00.4035", "SQL Server 2005 Service Pack 3"),
            new("09.00.4053", "SQL Server 2005 Service Pack 3 GDR: October 13, 2009"),
            new("09.00.5000", "SQL Server 2005 Service Pack 4 (SP4): December 17, 2010"),
            new("09.00.5057", "Security update for SQL Server 2005 Service Pack 4 GDR: June 14, 2011"),
            new("10.00.1600", "SQL Server 2008 (RTM)"),
            new("10.50.1600", "SQL Server 2008 R2 RTM: April 21, 2010"),
            new("10.50.6000", "SQL Server 2008 R2 Service Pack 3 (SP3): September 26, 2014"),
            new("11.00.2100", "SQL Server 2012 RTM: March 6, 2012"),
            new("11.00.5058", "SQL Server 2012 Service Pack 2 (SP2): June 10, 2014"),
            new("12.00.2430", "2999197 Cumulative update package 4 (CU4) for SQL Server 2014: October 21, 2014"),
            new("13.00.4001", "Microsoft SQL Server 2016 Service Pack 1 (SP1)"),
            new("13.00.5026", "Microsoft SQL Server 2016 Service Pack 2 (SP2)"),
            new("13.00.6300", "Microsoft SQL Server 2016 Service Pack 3 (SP3)"),
            new("13.00.6435", "5029186 Security update for SQL Server 2016 SP3 GDR: October 10, 2023"),
            new("14.00.1000", "Microsoft SQL Server 2017 (RTM)"),
            new("14.00.3045", "Microsoft SQL Server 2017 (RTM-CU12) (KB4464082)"),
            new("14.00.3048", "Microsoft SQL Server 2017 (RTM-CU13) (KB4466404)"),
            new("14.00.3162", "Microsoft SQL Server 2017 (RTM-CU15) (KB4498951)"),
            new("15.00.2070", "4517790 Servicing Update (GDR1) for SQL Server 2019 RTM"),
            new("15.00.2080", "Microsoft SQL Server 2019 (RTM-GDR) (KB4583458)"),
            new("16.00.1000", "Microsoft SQL Server 2022 RTM"),
            new("16.00.1105", "5029379 Security update for SQL Server 2022 GDR: October 10, 2023")
        }.ToDictionary(i => i.Version);
    }
}