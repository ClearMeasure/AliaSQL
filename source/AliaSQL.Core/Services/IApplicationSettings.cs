using System;

namespace AliaSQL.Core.Services
{
    internal interface IApplicationSettings
    {
        TimeSpan? Timeout();
    }
}