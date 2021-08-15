using System.Reflection;
using System.Transactions;

namespace AliaSQL.Core.Services.Impl
{
    internal class TransactionProvider
    {
        private readonly IApplicationSettings _applicationSettings;

        public TransactionProvider()
        {
            _applicationSettings = new ApplicationSettings();
        }
        
        private void SetTransactionManagerField(string fieldName, object value)
        {
            typeof(TransactionManager).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static)?.SetValue(null, value);
        }

        public TransactionScope CreateTransactionScope()
        {
            var timeout = _applicationSettings.Timeout();

            if (timeout == null)
            {
                return new TransactionScope();
            }

            SetTransactionManagerField("_cachedMaxTimeout", true);
            SetTransactionManagerField("_maximumTimeout", timeout.GetValueOrDefault());
            return new TransactionScope(TransactionScopeOption.RequiresNew, timeout.GetValueOrDefault());
        }
    }
}
