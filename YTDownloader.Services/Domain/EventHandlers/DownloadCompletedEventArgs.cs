namespace YTDownloader.Services.Domain.EventHandlers
{
    public class DownloadCompletedEventArgs : DownloadChangedEventArgs
    {
        public DownloadCompletedEventArgs(DownloadInfo e) : base(e) { }
    }
}
