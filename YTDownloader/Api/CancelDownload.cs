using SimpleWebApiServer;
using System.Net;
using System.Threading.Tasks;
using YTDownloader.Api.RequestParameters;

namespace YTDownloader.Api
{
    public class CancelDownload : ApiPostRequestResponse<CancelDownloadRequestParameters>
    {
        public CancelDownload(HttpListenerRequestCache cache = null) : base("/api/CancelDownload", cache) { }
        public override bool IsCacheable => false;

        protected override Task ProcessPostRequest(HttpListenerRequest request, CancelDownloadRequestParameters postData)
        {
            return Task.Run(() =>
             {
                 AppContext.DownloadManager.CancelDownload(postData.Id);
                 return string.Empty;
             });
        }
    }
}
