using Simple.Web;

namespace HelloEventStore.Web
{
    [UriTemplate("/")]
    public class Index : IGet
    {
        public Status Get()
        {
            return Status.OK;
        }
    }
}