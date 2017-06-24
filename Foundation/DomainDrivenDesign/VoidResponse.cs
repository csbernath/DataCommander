namespace Foundation.DomainDrivenDesign
{
    public class VoidResponse : IResponse
    {
        public static readonly VoidResponse Default = new VoidResponse();

        private VoidResponse()
        {
        }
    }
}