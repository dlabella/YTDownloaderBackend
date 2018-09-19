using System;

namespace YTDownloader.Services.Domain.EventHandlers
{
    public class DownloadChangedEventArgs : EventArgs
    {
        public DownloadInfo Info { get; private set; }
        public DownloadChangedEventArgs(DownloadInfo e)
        {
            Info = e;
        }
    }
}
