using Common.Extensions;
using Common.Logging;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using YTDownloader.Services.Domain;
using YTDownloader.Services.Domain.EventHandlers;

namespace YTDownloader.Services
{
    public class DownloadManager
    {
        private readonly ConcurrentQueue<DownloadInfo> _queue = new ConcurrentQueue<DownloadInfo>();
        private static readonly ConcurrentDictionary<string, DownloadInfo> ActiveDownloads = new ConcurrentDictionary<string, DownloadInfo>();
        private const int MaxParallelDownloads = 2;
        private readonly YoutubeDl _downloader;

        public DownloadManager(string youtubedlBin)
        {
            _downloader = new YoutubeDl(youtubedlBin);
        }

        public void Enqueue(string id, string filePath, Uri url, string mode, CookieContainer cookieContainer = null, bool forceDownload = false, bool disableTracking = false)
        {
            var di = BuildDownloadInfo(id, filePath, url, mode, cookieContainer, true, disableTracking);
            if (EnqueueDownload(di, forceDownload))
            {
                ProcessQueue();
            }
            else
            {
                Logger.Debug("The url:" + url + " is already in queue or downloading...");
            }
        }

        private static DownloadInfo BuildDownloadInfo(string id, string filePath, Uri url, string mode, CookieContainer cookieContainer = null, bool isQueued = true, bool disableTracking = false)
        {
            var di = new DownloadInfo(id, filePath, url, mode, isQueued, disableTracking);

            if (cookieContainer != null)
            {
                di.Cookies = cookieContainer.GetCookies(url);
            }
            return di;
        }

        private bool EnqueueDownload(DownloadInfo downloadInfo, bool forceDownload)
        {
            if (_queue.Any(x => x.Id == downloadInfo.Id) ||
                ActiveDownloads.ContainsKey(downloadInfo.Id) && !forceDownload)
            {
                return false;
            }
            _queue.Enqueue(downloadInfo);
            return true;
        }

        private DownloadInfo DequeueDownload()
        {
            if (_queue.Count <= 0)
            {
                return null;
            }
            _queue.TryDequeue(out var item);
            item.IsQueued = false;
            return item;
        }

        private void ProcessQueue()
        {
            if (!CanProcessNextQueueItem())
            {
                Logger.Debug("Cannot process next item");
                Logger.Debug("Queue Count: " + _queue.Count);
                Logger.Debug("Active downloads: " + ActiveDownloads.Values.Count(x => !x.IsQueued));
                return;
            }
            try
            {
                Logger.Debug("Dequeuing download item ...");
                ThreadPool.QueueUserWorkItem((state) =>
                {
                    var item = DequeueDownload(); 
                    ProcessDownload(item);
                });
            }
            catch (Exception ex)
            {
                Logger.Exception("Download Exception: " + ex.Message, ex);
            }
        }
        private bool CanProcessNextQueueItem()
        {
            if (_queue.Count == 0)
            {
                return false;
            }

            return ActiveDownloads.Values.Count(x => !x.IsQueued) < MaxParallelDownloads;
        }
        public IEnumerable<DownloadInfo> GetDownloads()
        {
            var active = new List<DownloadInfo>(ActiveDownloads.Values);
            active.AddRange(_queue);
            return active;
        }

        public void CancelDownload(string downloadId)
        {
            if (ActiveDownloads.ContainsKey(downloadId))
            {
                ActiveDownloads.TryGetValue(downloadId, out var cancelledDownload);
                if (cancelledDownload!=null && cancelledDownload.DownloadProcess!=null)
                {
                    cancelledDownload.DownloadProcess.Kill();
                }
            }
            foreach(var item in _queue)
            {
                if (item.Id == downloadId)
                {
                    item.Cancelled=true;
                }
            }
        }

        private void ProcessDownload(DownloadInfo di)
        {
            if (di.Cancelled){
                ActiveDownloads.TryRemove(di.Id, out var cancelledItem);
                return;
            }

            ActiveDownloads.AddOrUpdate(di.Id, di, (o, n) => di);

            _downloader.Download(di.Id, di.File.ToSafePath(), di.Source, di.Mode, GetCookieDictionary(di.Cookies), di.DisableTracking,
               UpdateActiveDownload,
               ActiveDownloadCompleted
            );
        }

        private static Dictionary<string, string> GetCookieDictionary(IEnumerable cookies)
        {
            var cookieDictionary = new Dictionary<string, string>();
            if (cookies == null)
            {
                return cookieDictionary;
            }
            foreach (Cookie cookie in cookies)
            {
                cookieDictionary.Add(cookie.Name, cookie.Value);
            }
            return cookieDictionary;
        }

        private void ActiveDownloadCompleted(object sender, DownloadCompletedEventArgs e)
        {
            ActiveDownloads.TryRemove(e.Info.Id, out _);
            ProcessQueue();

            Logger.Debug("Download of " + e.Info.File + " is completed");
        }

        private static void UpdateActiveDownload(object sender, DownloadChangedEventArgs e)
        {
            ActiveDownloads.AddOrUpdate(e.Info.Id, e.Info, (o, n) =>
            {
                if (e.Info.DownloadProcess != null && e.Info.DownloadProcess != n.DownloadProcess)
                {
                    n.DownloadProcess = e.Info.DownloadProcess;
                }
                if (e.Info.BytesTotal > n.BytesTotal)
                {
                    n.BytesTotal = e.Info.BytesTotal;
                }
                if (e.Info.BytesReceived > n.BytesReceived)
                {
                    n.BytesReceived = e.Info.BytesReceived;
                }
                if (!string.IsNullOrEmpty(e.Info.File) && n.File != e.Info.File)
                {
                    n.File = e.Info.File;
                }
                return n;
            });

            if (e.Info.BytesReceived > 0 && e.Info.BytesTotal > 0)
            {
                Logger.Debug(((e.Info.BytesReceived / e.Info.BytesTotal) * 100) + " - " + e.Info.File);
            }
            else
            {
                Logger.Debug("??% - " + e.Info.File);
            }
        }
    }
}
