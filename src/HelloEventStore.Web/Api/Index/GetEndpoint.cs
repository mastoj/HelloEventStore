using System.Collections;
using System.Collections.Generic;
using Simple.Web;
using Simple.Web.Behaviors;
using Simple.Web.Links;

namespace HelloEventStore.Web.Api.Index
{
    [UriTemplate("/api")]
    public class GetEndpoint : IGet, IOutput<IEnumerable<Link>>
    {
        public Status Get()
        {
            Output = LinkHelper.GetRootLinks();
            return Status.OK;
        }

        public IEnumerable<Link> Output { get; private set; }
    }
}