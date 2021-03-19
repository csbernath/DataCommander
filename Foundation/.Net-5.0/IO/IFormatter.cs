using System.Text;

namespace Foundation.IO
{
    public interface IFormatter
    {
        void AppendTo(StringBuilder sb, object[] args);
    }
}