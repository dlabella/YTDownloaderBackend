using SimpleWebApiServer.ApiParameters;
using System;
using System.IO;

namespace YTDownloader.Api.RequestParameters
{
    public class DownloadRequestParameters : ApiParametersBase
    {
        public string Url { get; set; }
        public string Path { get; set; }
        public string Mode { get; set; }
        public override bool AreValid => ParametersAreValid();

        private bool ParametersAreValid()
        {
            if (Uri.TryCreate(Url,UriKind.RelativeOrAbsolute, out var uri) ||
                IsValidMode(Mode) ||
                !string.IsNullOrEmpty(Path))
            {
                return true;
            }
            return false;
        }

        private bool IsValidMode(string mode)
        {
            if (string.IsNullOrEmpty(Mode) ||
                Mode == "DAPL" ||
                Mode == "DA"  ||
                Mode == "DF")
            {
                return true;
            }
            return false;
        }
    }
}
