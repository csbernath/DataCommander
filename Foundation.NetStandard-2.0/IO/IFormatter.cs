using System.Text;

namespace Foundation.IO
{
    /// <summary>
    /// 
    /// </summary>
    public interface IFormatter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="args"></param>
        void AppendTo( StringBuilder sb, object[] args );
    }
}