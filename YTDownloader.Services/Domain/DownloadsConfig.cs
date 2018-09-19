using System;
using System.Collections.Generic;
using System.Text;

namespace YTDownloader.Services.Domain
{
    [Serializable]
    public class DownloadsConfig
    {
        public string FilesDownloadPath { get;set;}
        public string AudioDownloadPath { get;set;}
    }
}
