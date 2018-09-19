using System;

namespace YTDownloader.Services.Domain
{
    public abstract class DownloadInfoLineParser
    {
        public abstract DownloadInfo Parse(string line,string id=null, Uri source = null, string file=null);
    }
}
