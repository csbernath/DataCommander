namespace Foundation.Data.SqlClient.AppLock
{
    public enum GetAppLockReturnCode
    {
        TheLockWasSuccessfullyGrantedSynchronously = 0,
        TheLockWasGrantedSuccessfullyAfterWaitingForOtherIncompatibleLocksToBeReleased = 1,
        TheLockRequestTimedOut = -1,
        TheLockRequestWasCanceled = -2,
        TheLockRequestWasChosenAsADeadlockCictim = -3,
        IndicatesAParameterValidationOrOtherCallError = -999
    }
}