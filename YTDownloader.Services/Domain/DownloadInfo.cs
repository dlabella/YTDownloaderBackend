using Common;
using Common.Extensions;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace YTDownloader.Services.Domain
{
    [Serializable]
    public class DownloadInfo
    {
        public DownloadInfo():this(string.Empty,null,string.Empty,false)
        {

        }
        public DownloadInfo(string file, Uri source,string mode):this(file,source,mode, false)
        {

        }
        public DownloadInfo(string file, Uri source, string mode, bool isQueued):this(source?.ToString(),file,source, mode,isQueued,false)
        {
        }
        public DownloadInfo(string id, string file, Uri source,string mode) : this(id, file, source, mode, false, false)
        {
        }
        public DownloadInfo(string id, string file, Uri source, string mode, bool isQueued, bool disableTracking)
        {
            Id = id;
            File = file?.ToSafePath();
            Source = source;
            IsQueued = isQueued;
            DisableTracking = disableTracking;
            Mode=mode;
        }
        [JsonIgnore]
        public bool DisableTracking { get; set; }
        DateTime _startDate = DateTime.MinValue;
        int _bytesReceived;
        [JsonProperty("name")]
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(File)) return "Unknown";
                FileInfo finfo = new FileInfo(File);
                return finfo.Name;
            }
        }
        [JsonProperty("mode")]
        public string Mode { get;set;}
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("file")]
        public string File { get; set; }
        [JsonProperty("isQueued")]
        public bool IsQueued { get; set; }
        [JsonProperty("source")]
        public Uri Source { get; set; }
        [JsonProperty("bytesTotal")]
        public int BytesTotal { get; set; }
        [JsonProperty("cancelled")]
        public bool Cancelled { get;set;}
        [JsonProperty("bytesReceived")]
        public int BytesReceived
        {
            get { return _bytesReceived; }
            set
            {
                _bytesReceived = value;
                if (_bytesReceived > 0 && _startDate == DateTime.MinValue)
                {
                    _startDate = DateTime.Now;
                }
            }
        }
        [JsonProperty("bytesTransferred")]
        public int BytesTransfered { get; set; }
        [JsonProperty("timeSpent")]
        public TimeSpan TimeSpent
        {
            get { return (DateTime.Now - _startDate); }
        }
        [JsonProperty("isEmpty")]
        public bool IsEmpty { get; private set; }
        [JsonProperty("isCompleted")]
        public bool IsCompleted => (BytesReceived >= BytesTotal && BytesTotal > 0);
        [JsonProperty("percentCompleted")]
        public float PercentCompleted
        {
            get {
                if (BytesReceived>0 && BytesTotal > 0)
                {
                    return ((BytesReceived / (float)BytesTotal) * 100);
                }
                return 0;
            }
        }
        [JsonProperty("downloadFaulted")]
        public bool DownloadFaulted { get; set; }
        [JsonIgnore]
        public CookieCollection Cookies { get; set; }
        [JsonIgnore]
        public Process DownloadProcess { get;set;}
    }
}
