namespace Foundation.Text
{
    public sealed class IndentedLine
    {
        public readonly int Indentation;
        public readonly string Text;

        public IndentedLine(int indentation, string text)
        {
            Indentation = indentation;
            Text = text;
        }

        public IndentedLine Indent(int indentation) => new IndentedLine(Indentation + indentation, Text);
    }
}