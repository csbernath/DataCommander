namespace Foundation.DomainDrivenDesign
{
    public class VoidResponse : IResponse
    {
        private static readonly string messageType = typeof(VoidResponse).FullName;
        public static readonly VoidResponse Default = new VoidResponse();

        private VoidResponse()
        {
        }

        public string MessageType => messageType;
    }
}