#if FOUNDATION_3_5

namespace DataCommander.Foundation.Threading.Tasks
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class Task<TResult> : Task
    {
        private readonly Func<object, TResult> function;
        private TResult result;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="function"></param>
        /// <param name="state"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="taskCreationOptions"></param>
        public Task( Func<object, TResult> function, object state, CancellationToken cancellationToken, TaskCreationOptions taskCreationOptions )
        {
            Contract.Requires<ArgumentNullException>( function != null );
            this.function = function;
            this.Construct( this.Invoke, state, cancellationToken, taskCreationOptions );
        }

        /// <summary>
        /// 
        /// </summary>
        public TResult Result
        {
            get
            {
                if (!this.IsCompleted)
                {
                    throw new InvalidOperationException();
                }

                if (this.Exception != null)
                {
                    throw this.Exception;
                }

                return this.result;
            }
        }

        private void Invoke( object state )
        {
            this.result = this.function( state );
        }
    }
}

#endif