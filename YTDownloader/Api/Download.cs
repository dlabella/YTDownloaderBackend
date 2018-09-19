using SimpleWebApiServer;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using YTDownloader.Api.RequestParameters;

namespace YTDownloader.Api
{
    public class Download : ApiPostRequestResponse<DownloadRequestParameters>
    {
        public Download(HttpListenerRequestCache cache = null) : base("/api/Download", cache) { }
        public override bool IsCacheable => false;

        private string GetDownloadPath(string mode, string path, string url)
        {
            var result = path;
            if (string.IsNullOrEmpty(path) && mode?.ToUpper().StartsWith("DA") == true)
            {
                result = AppContext.Config.AudioDownloadPath;
            }
            else if (string.IsNullOrEmpty(path))
            {
                var start = url.LastIndexOf("/")+1;
                var fileName = url.Substring(start, url.Length-start); 
                result = Path.Combine(AppContext.Config.FilesDownloadPath, fileName);
            }
            return result;
        }

        protected override Task ProcessPostRequest(HttpListenerRequest request, DownloadRequestParameters postData)
        {
            if (Uri.TryCreate(postData.Url, UriKind.RelativeOrAbsolute, out var uri))
            {
                return Task.Run(() =>
                {

                    var path = GetDownloadPath(postData.Mode, postData.Path, postData.Url);

                    AppContext.DownloadManager.Enqueue(postData.Url, path, uri, postData.Mode);
                    return string.Empty;
                });
            }
            else
            {
                return Task.FromResult("Invalid Uri");
            }
        }
    }
}
