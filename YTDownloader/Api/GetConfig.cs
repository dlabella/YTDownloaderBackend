using Newtonsoft.Json;
using SimpleWebApiServer;
using SimpleWebApiServer.ApiParameters;
using System;
using System.Net;
using System.Threading.Tasks;
using YTDownloader.Services.Domain;

namespace YTDownloader.Api
{
    public class GetConfig : ApiGetRequestResponse<EmptyApiParameters>
    {
        private static readonly EmptyApiParameters _emptyParameters = new EmptyApiParameters();
        public GetConfig(HttpListenerRequestCache cache = null) : base("/api/GetConfig", cache) { }
        protected override EmptyApiParameters ParseParameters(SimpleWebApiServer.RequestParameters parameters)
        {
            return _emptyParameters;
        }

        protected override Task<string> ProcessGetRequest(HttpListenerRequest request, EmptyApiParameters parameters)
        {
            var task = Task.Run(() => {
                var cfg =new DownloadsConfig
                {
                    FilesDownloadPath = AppContext.Config.FilesDownloadPath,
                    AudioDownloadPath = AppContext.Config.AudioDownloadPath,
                };
                return JsonConvert.SerializeObject(cfg);
            });

            task.ConfigureAwait(false);
            return task;
        }
    }
}
