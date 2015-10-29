using System.Collections.Generic;
using AliaSQL.Core;
using AliaSQL.Core.Services;
using AliaSQL.Core.Services.Impl;
using AliaSQL.Core.Services.Impl.AliaSQL.Core.Services.Impl;
using NUnit.Framework;
using Rhino.Mocks;

namespace AliaSQL.UnitTests
{
	[TestFixture]
	public class DatabaseActionExecutorFactoryTester
	{
		[Test]
		public void Correctly_constructs_action_executors()
		{
			DatabaseAction[] actions = new DatabaseAction[] { DatabaseAction.Create, DatabaseAction.Update };

			MockRepository mocks = new MockRepository();
            IDatabaseActionResolver resolver = mocks.StrictMock<IDatabaseActionResolver>();
            IDataBaseActionLocator locator = mocks.StrictMock<IDataBaseActionLocator>();

            IDatabaseActionExecutor creator = mocks.StrictMock<IDatabaseActionExecutor>();
            IDatabaseActionExecutor updater = mocks.StrictMock<IDatabaseActionExecutor>();

			using (mocks.Record())
			{
				Expect.Call(resolver.GetActions(RequestedDatabaseAction.Create)).Return(actions);
				Expect.Call(locator.CreateInstance(DatabaseAction.Create)).Return(creator);
				Expect.Call(locator.CreateInstance(DatabaseAction.Update)).Return(updater);
			}

			using (mocks.Playback())
			{
				IDatabaseActionExecutorFactory factory = new DatabaseActionExecutorFactory(resolver, locator);
				IEnumerable<IDatabaseActionExecutor> executors = factory.GetExecutors(RequestedDatabaseAction.Create);
				IList<IDatabaseActionExecutor> executorList = new List<IDatabaseActionExecutor>(executors);

				Assert.That(executorList, Is.EqualTo(new IDatabaseActionExecutor[]{ creator, updater }));		
			}

			mocks.VerifyAll();
		}
	}
}