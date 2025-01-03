using System;
using Foundation.Log;

namespace Foundation.Threading;

/// <summary>
/// This class is a specialized <see cref="WorkerThread"/> which
/// repeats an operation until the operation is stopped.
/// E.g Windows Services can use this class.
/// </summary>
/// <remarks>
/// There are two ways to use this class:
/// <list type="table">
/// <item>
///        <term>With inheritance</term>
///        <description>Call ctor</description>
///    </item>
/// <item>
///        <term>Without inheritance (as a member field)</term>
///        <description>Call <see cref="Initialize"/></description>
///    </item>
/// </list>
/// </remarks>
public class LoopThread
{
    private static readonly ILog Log = LogFactory.Instance.GetCurrentTypeLog();
    private ILoopable _loopable;

    /// <summary>
    /// Inherited class must call this constructor.
    /// </summary>
    protected LoopThread()
    {
    }

    /// <summary>
    /// If this class is not inherited the caller can initialize the instance here.
    /// </summary>
    /// <param name="loopable"></param>
    public LoopThread(ILoopable loopable)
    {
        Initialize(loopable);
    }

    /// <summary>
    /// Gets the underlying <see cref="WorkerThread"/>.
    /// </summary>
    public WorkerThread Thread { get; private set; }

    /// <summary>
    /// Inherited class must initialize the base class with this method.
    /// </summary>
    /// <param name="loopable"></param>
    protected void Initialize(ILoopable loopable)
    {
        _loopable = loopable;
        Thread = new WorkerThread(Start);
    }

    private void Start()
    {
        Exception exception = null;

        while (!Thread.IsStopRequested)
        {
            try
            {
                if (!Thread.IsStopRequested)
                {
                    _loopable.First(exception);
                    exception = null;

                    while (!Thread.IsStopRequested)
                    {
                        _loopable.Next();
                    }
                }
            }
            catch (Exception e)
            {
                exception = e;
                Log.Write(LogLevel.Error, "LoopThread({0},{1}) exception:\r\n{2}", Thread.Name,
                    Thread.ManagedThreadId, e.ToString());
            }
        }

        try
        {
            _loopable.Last();
        }
        catch (Exception e)
        {
            Log.Write(LogLevel.Error, "LoopThread({0},{1}) exception:\r\n{2}", Thread.Name,
                Thread.ManagedThreadId, e.ToString());
        }
    }
}