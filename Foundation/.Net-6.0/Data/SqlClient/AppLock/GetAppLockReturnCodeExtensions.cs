
namespace Foundation.Data.SqlClient.AppLock
{
    public static class GetAppLockReturnCodeExtensions
    {
        public static bool Succeeded(this GetAppLockReturnCode getAppLockReturnCode) => (int)getAppLockReturnCode >= 0;
    }
}