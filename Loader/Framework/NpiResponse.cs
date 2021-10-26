namespace NPPES.Loader
{
    public class NpiResponse
    {
        public string Contents { get; private set; }
        public NPIRequest Request { get; private set; }
        private NpiResponse(NPIRequest request, string json)
        {
            this.Request = request;
            this.Contents = json;

    }
        public static NpiResponse Create(NPIRequest request, string response)
        {
            return new NpiResponse(request, response);
        }
    }
}
