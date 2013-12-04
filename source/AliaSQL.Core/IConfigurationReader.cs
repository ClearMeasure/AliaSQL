using System.Collections.Generic;


namespace AliaSQL.Core
{
    public interface IConfigurationReader
    {
        string GetRequiredSetting(string key);
        int GetRequiredIntegerSetting(string key);
        bool GetRequiredBooleanSetting(string key);
        IEnumerable<string> GetStringArray(string key);
        bool? GetOptionalBooleanSetting(string key);
        string GetOptionalSetting(string key);
    }
}