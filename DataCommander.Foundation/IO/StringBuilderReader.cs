namespace DataCommander.Foundation.IO
{
    using System;
    using System.IO;
    using System.Text;

    /// <summary>
    /// 
    /// </summary>
    public sealed class StringBuilderReader : TextReader
    {
        private readonly StringBuilder _stringBuilder;
        private int _index;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringBuilder"></param>
        public StringBuilderReader(StringBuilder stringBuilder)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(stringBuilder != null);
#endif

            this._stringBuilder = stringBuilder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int Peek()
        {
            int result;

            if (this._index < this._stringBuilder.Length)
            {
                result = this._stringBuilder[this._index];
            }
            else
            {
                result = -1;
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int Read()
        {
            int result;

            if (this._index < this._stringBuilder.Length)
            {
                result = this._stringBuilder[this._index];
                this._index++;
            }
            else
            {
                result = -1;
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override int Read(char[] buffer, int index, int count)
        {
            var result = Math.Min(count, this._stringBuilder.Length - this._index);

            if (result > 0)
            {
                this._stringBuilder.CopyTo(this._index, buffer, index, result);
                this._index += result;
            }

            return result;
        }
    }
}