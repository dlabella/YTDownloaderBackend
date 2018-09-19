using SimpleWebApiServer.ApiParameters;
using System;
using System.IO;

namespace YTDownloader.Api.RequestParameters
{
    public class CancelDownloadRequestParameters : ApiParametersBase
    {
        public string Id { get; set; }
        public override bool AreValid => ParametersAreValid();

        private bool ParametersAreValid()
        {
            if (!string.IsNullOrEmpty(Id))
            {
                return true;
            }
            return false;
        }
    }
}
