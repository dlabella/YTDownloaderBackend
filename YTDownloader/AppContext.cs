
using YTDownloader.Services;

namespace YTDownloader
{
    public static class AppContext
    {
        public static Configuration Config;
        public static DownloadManager DownloadManager;
        public static void Initialize()
        {
            Config = Configuration.Load();
            DownloadManager = new DownloadManager(Config.YoutubeDlBin);
        }
    }
}
