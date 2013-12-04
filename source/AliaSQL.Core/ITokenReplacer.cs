

namespace AliaSQL.Core
{
    public interface ITokenReplacer
    {
        void Replace(string token, string tokenValue);
        string Text { get; set; }
    }
}