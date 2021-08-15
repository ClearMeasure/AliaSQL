using System;
using System.Configuration;

namespace AliaSQL.Core.Services.Impl
{
    internal class ApplicationSettings : IApplicationSettings
    {
        public TimeSpan? Timeout()
        {
            var timeoutValue = ConfigurationManager.AppSettings["timeout"];

            if (string.IsNullOrEmpty(timeoutValue))
            {
                return null;
            }

            if (!TimeSpan.TryParse(timeoutValue, out var time))
            {
                return null;
            }

            return time;
        }
    }
}
