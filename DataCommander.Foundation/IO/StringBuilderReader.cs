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
        private readonly StringBuilder stringBuilder;
        private int index;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringBuilder"></param>
        public StringBuilderReader(StringBuilder stringBuilder)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(stringBuilder != null);
#endif

            this.stringBuilder = stringBuilder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int Peek()
        {
            int result;

            if (this.index < this.stringBuilder.Length)
            {
                result = this.stringBuilder[this.index];
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

            if (this.index < this.stringBuilder.Length)
            {
                result = this.stringBuilder[this.index];
                this.index++;
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
            var result = Math.Min(count, this.stringBuilder.Length - this.index);

            if (result > 0)
            {
                this.stringBuilder.CopyTo(this.index, buffer, index, result);
                this.index += result;
            }

            return result;
        }
    }
}