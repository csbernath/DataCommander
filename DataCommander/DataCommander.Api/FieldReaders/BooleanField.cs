namespace DataCommander.Api.FieldReaders;

public sealed class BooleanField(bool value)
{
    public override string ToString()
    {
        int int32Value;

        if (value)
        {
            int32Value = 1;
        }
        else
        {
            int32Value = 0;
        }

        return int32Value.ToString();
    }
}