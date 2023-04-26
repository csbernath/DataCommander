using System.Text;

namespace Foundation.IO;

public interface IFormatter
{
    void AppendTo(StringBuilder stringBuilder, object[] args);
}