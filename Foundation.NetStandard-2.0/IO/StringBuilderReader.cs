using System;
using System.IO;
using System.Text;
using Foundation.Diagnostics.Contracts;

namespace Foundation.IO
{
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
            FoundationContract.Requires<ArgumentNullException>(stringBuilder != null);

            _stringBuilder = stringBuilder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int Peek()
        {
            int result;

            if (_index < _stringBuilder.Length)
            {
                result = _stringBuilder[_index];
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

            if (_index < _stringBuilder.Length)
            {
                result = _stringBuilder[_index];
                _index++;
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
            var result = Math.Min(count, _stringBuilder.Length - _index);

            if (result > 0)
            {
                _stringBuilder.CopyTo(_index, buffer, index, result);
                _index += result;
            }

            return result;
        }
    }
}