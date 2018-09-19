using Newtonsoft.Json;
using SimpleWebApiServer;
using SimpleWebApiServer.ApiParameters;
using System;
using System.Net;
using System.Threading.Tasks;

namespace YTDownloader.Api
{
    public class GetDownloads : ApiGetRequestResponse<EmptyApiParameters>
    {
        private static readonly EmptyApiParameters _emptyParameters = new EmptyApiParameters();
        public GetDownloads(HttpListenerRequestCache cache = null) : base("/api/GetDownloads", cache) { }
        public override bool IsCacheable => false;
        protected override EmptyApiParameters ParseParameters(SimpleWebApiServer.RequestParameters parameters)
        {
            return _emptyParameters;
        }

        protected override Task<string> ProcessGetRequest(HttpListenerRequest request, EmptyApiParameters parameters)
        {
            var task = Task.Run(()=> { 
                var downloads = AppContext.DownloadManager.GetDownloads();
                return JsonConvert.SerializeObject(downloads);
            });

            task.ConfigureAwait(false);
            return task;
        }
    }
}
