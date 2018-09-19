using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using YTDownloader.Services.Domain.EventHandlers;

namespace YTDownloader.Services
{
    public class YoutubeDlInfo
    {
        public YoutubeDlInfo(
            string id,
            string outputFile,
            Uri url,
            string mode, 
            IEnumerable<KeyValuePair<string, string>> cookies = null,
            bool disableTracking = false,
            DownloadChangedEventHandler downloadChanged = null,
            DownloadCompletedEventHandler downloadCompleted = null)
        {
            Id = id;
            OutputFile = outputFile;
            Url = url;
            Mode=mode;
            Cookies = cookies;
            DisableTracking = disableTracking;
            DownloadChanged = downloadChanged;
            DownloadCompleted = downloadCompleted;
        }

        public string Id { get; set; }
        public string OutputFile { get; set; }
        public Uri Url { get; set; }
        public string Mode { get;set;}
        public IEnumerable<KeyValuePair<string, string>> Cookies { get; set; }
        public bool DisableTracking { get; set; }
        public DownloadChangedEventHandler DownloadChanged { get; set; }
        public DownloadCompletedEventHandler DownloadCompleted { get; set; }
        public Process DownloadProcess { get;set;}
        public string GetCommadArguments()
        {
            var argString = new StringBuilder();
            AddIgnoreErrors(argString);
            AddNewLineToCommand(argString);
            AddNoCheckCertificateToCommand(argString);
            AddCookiesToCommand(argString, Cookies);
            if (string.Compare(Mode,"DAPL", true)==0)
            {
                AddTemplateOutputPlaylistToCommand(argString, OutputFile);
                AddPlayList(argString);
                AddExtractAudio(argString);
            }
            else if (string.Compare(Mode, "DA", true) == 0)
            {
                AddTemplateOutputFileToCommand(argString, OutputFile);
                AddExtractAudio(argString);
            }
            else
            {
                AddOutputFileToCommand(argString, OutputFile);
            }
            
            AddUrlToCommand(argString, Url);
            return argString.ToString();
        }
        private void AddPlayList(StringBuilder sb)
        {
            sb.Append(" --yes-playlist ");
        }
        private void AddIgnoreErrors(StringBuilder sb)
        {
            sb.Append(" -ic ");
        }
        private void AddTemplateOutputPlaylistToCommand(StringBuilder sb, string path)
        {
            var fp = path;
            if (path.Contains(":\\") && !path.EndsWith("\\"))
            {
                fp+="\\";
            }
            else if (path.Contains("/") && !path.EndsWith("/"))
            {
                fp += "/";
            }
            sb.Append($" -o '{fp}%(playlist)s/%(title)s.%(ext)s' ");
        }
        private void AddTemplateOutputFileToCommand(StringBuilder sb, string path)
        {
            var fp = path;
            if (path.Contains(":\\") && !path.EndsWith("\\"))
            {
                fp += "\\";
            }
            else if (path.Contains("/") && !path.EndsWith("/"))
            {
                fp += "/";
            }
            sb.Append($" -o '{fp}%(title)s.%(ext)s' ");
        }

        private void AddExtractAudio(StringBuilder sb)
        {
            sb.Append(" -f bestaudio --extract-audio --audio-format mp3 --audio-quality 0 ");
        }
        private void AddNewLineToCommand(StringBuilder sb)
        {
            sb.Append(" --newline ");
        }
        private void AddNoCheckCertificateToCommand(StringBuilder sb)
        {
            sb.Append(" --no-check-certificate ");
        }
        private void AddUrlToCommand(StringBuilder sb, Uri url)
        {
            sb.Append(url);
        }
        private void AddOutputFileToCommand(StringBuilder sb, string filePath)
        {
            sb.Append(" -o \"")
              .Append(filePath)
              .Append("\" ");
        }
        private void AddCookiesToCommand(StringBuilder sb, IEnumerable<KeyValuePair<string, string>> cookies)
        {
            if (cookies != null)
            {
                sb.Append(" --add-header Cookie:\"");
                foreach (var cookie in cookies)
                {
                    sb.Append(cookie.Key + "=" + cookie.Value + ";");
                }
                sb.Append("\" ");
            }
        }
    }
}
