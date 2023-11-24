namespace DataCommander.Providers.SqlServer;

public enum TemporalType
{
    NonTemporalTable = 0,
    HistoryTable = 1,
    SystemVersionedTemporalTable = 2
}