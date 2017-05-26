using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Foundation.IO
{
    /// <summary>
    /// 
    /// </summary>
    public class AsyncTextWriter
    {
        #region Private Fields

        private readonly TextWriter _textWriter;
        private readonly List<AsyncTextWriterListItem> _list = new List<AsyncTextWriterListItem>();
        private readonly object _syncObject = new object();
        private readonly ManualResetEvent _waitHandle = new ManualResetEvent(false);
        private RegisteredWaitHandle _registeredWaitHandle;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textWriter"></param>
        public AsyncTextWriter(TextWriter textWriter)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(textWriter != null);
#endif

            this._textWriter = textWriter;
        }

        private void Flush()
        {
            var sb = new StringBuilder();

            while (this._list.Count > 0)
            {
                AsyncTextWriterListItem[] items;
                lock (this._list)
                {
                    var count = this._list.Count;
                    items = new AsyncTextWriterListItem[count];
                    this._list.CopyTo(items);
                    this._list.Clear();
                }

                for (var i = 0; i < items.Length; i++)
                {
                    items[i].AppendTo(sb);
                }
            }

            this._textWriter.Write(sb);
            this._textWriter.Flush();
        }

        private void Unregister()
        {
            lock (this._syncObject)
            {
                if (this._registeredWaitHandle != null)
                {
                    ////log.Write(LogLevel.Trace,"Unregister...");
                    var succeeded = this._registeredWaitHandle.Unregister(null);
                    this._registeredWaitHandle = null;
                    ////log.Write(LogLevel.Trace,"Unregister succeeded.");
                }
            }
        }

        private void Callback(object state, bool timedOut)
        {
            if (this._list.Count > 0)
            {
                this.Flush();
            }
            else
            {
                this.Unregister();
            }
        }

        private void Write(AsyncTextWriterListItem item)
        {
            lock (this._list)
            {
                this._list.Add(item);
            }

            const int timeout = 10000; // 10 seconds

            lock (this._syncObject)
            {
                if (this._registeredWaitHandle == null)
                {
                    // log.Write("ThreadPool.RegisterWaitForSingleObject",LogLevel.Trace);
                    this._registeredWaitHandle = ThreadPool.RegisterWaitForSingleObject(this._waitHandle, this.Callback,
                        null, timeout, false);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void Write(string value)
        {
            var item = new AsyncTextWriterListItem(DefaultFormatter.Instance, value);
            this.Write(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formatter"></param>
        /// <param name="args"></param>
        public void Write(IFormatter formatter, params object[] args)
        {
            var item = new AsyncTextWriterListItem(formatter, args);
            this.Write(item);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Close()
        {
            this.Unregister();
            this.Flush();
            this._textWriter.Close();
        }
    }
}