using Common.Extensions;
using Common.Logging;
using System;
using System.Globalization;
using YTDownloader.Services.Domain;

namespace YTDownloader.Services
{
    public class YoutubeDlInfoParser 
    {
        public DownloadInfo Parse(string line, string id, Uri source, string mode, string file = null)
        {
            var data = new DownloadInfo(id, file, source, mode);

            if (string.IsNullOrEmpty(line) ||
                !line.Contains("%"))
            {
                return data;
            }
            Logger.Debug("Downloader Line: " + line);
            var items = line.Split(" ", 7, StringSplitOptions.RemoveEmptyEntries);
            if (items.Length < 7) return data;
            data.Id = id;
            if (items[2] == "of")
            {
                data.BytesTotal = items[3].ToByteSize();
            }
            else
            {
                data.BytesTotal = items[2].ToByteSize();
            }
            var percentStr = items[1].Replace("%", "");
            if (percentStr != null && data.BytesTotal>0)
            {
                var percent = float.Parse(percentStr, NumberStyles.Number, new CultureInfo("en-US"));
                if (percent > 0)
                {
                    data.BytesReceived = (int)((data.BytesTotal * percent) / 100);
                }
                else
                {
                    data.BytesReceived = 0;
                }

            }
            Logger.Debug($"BytesTotal: {data.BytesTotal}");
            Logger.Debug($"BytesRecieved: {data.BytesReceived}");
            return data;
        }
    }
}
