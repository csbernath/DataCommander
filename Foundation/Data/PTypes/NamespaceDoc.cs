namespace Foundation.Data.PTypes
{
#if DOC
    /// <summary>
    /// The <b>DataCommander.Foundation.Data.PTypes</b> namespace provides classes (parameter types) for native data types within SQL Server.
    /// These classes wrap and extend classes in <b>System.Data.SqlTypes</b> namespace.
    /// The data access layer generator utility (DataAccesslayerGenerator.exe) generates a class which inherits from <see cref="Database"/>.
    /// Value types for calling Microsoft SQL Server
    /// <para>A PType (<see cref="PInt32"/>,<see cref="PString"/> etc.) has 4 states:
    /// <list type="table">
    /// <item><term><c>Value</c></term><description>a concrete value</description></item>
    /// <item><term><c>Null</c></term><description>Microsoft SQL Server NULL value</description></item>
    /// <item><term><c>Default</c></term><description>Microsoft SQL Server stored procedures can have parameters with default values</description></item>
    /// <item><term><c>Empty</c></term><description>The returned 1x1 table has no rows.
    ///</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// A stored procedure input parameter can be: <c>Value, Null, Default</c>
    /// </para>
    /// <para>
    /// A stored procedure output parameter can be: <c>Value, Null, Default</c>
    /// </para>
    /// <para>
    ///Returned value of stored procedure/function can be: <c>Value,Null,Empty</c>
    /// </para>
    /// </summary>
    internal class NamespaceDoc
    {
    }
#endif
}