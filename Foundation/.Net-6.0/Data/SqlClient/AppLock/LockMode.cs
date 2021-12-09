
namespace Foundation.Data.SqlClient.AppLock
{
    public enum LockMode
    {
        Shared,
        Update,
        IntentShared,
        IntentExclusive,
        Exclusive
    }
}