using System.Collections.Generic;
using AliaSQL.Core;
using AliaSQL.Core.Services;
using AliaSQL.Core.Services.Impl;
using AliaSQL.Core.Services.Impl.AliaSQL.Core.Services.Impl;
using NUnit.Framework;

namespace AliaSQL.UnitTests
{
	[TestFixture]
	public class DatabaseActionResolverTester
	{
		[Test]
		public void Correctly_determines_create_actions()
		{
			IDatabaseActionResolver resolver = new DatabaseActionResolver();

			IEnumerable<DatabaseAction> actions = resolver.GetActions(RequestedDatabaseAction.Create);
			EnumerableAssert.That(actions, Is.EqualTo(new DatabaseAction[]{ DatabaseAction.Create, DatabaseAction.Update }));
		}

		[Test]
		public void Correctly_determines_update_actions()
		{
			IDatabaseActionResolver resolver = new DatabaseActionResolver();

			IEnumerable<DatabaseAction> actions = resolver.GetActions(RequestedDatabaseAction.Update);
			EnumerableAssert.That(actions, Is.EqualTo(new DatabaseAction[]{ DatabaseAction.Update }));
		}

		[Test]
		public void Correctly_determines_drop_actions()
		{
			IDatabaseActionResolver resolver = new DatabaseActionResolver();

			IEnumerable<DatabaseAction> actions = resolver.GetActions(RequestedDatabaseAction.Drop);
			EnumerableAssert.That(actions, Is.EqualTo(new DatabaseAction[] { DatabaseAction.Drop }));
		}

		[Test]
		public void Correctly_determines_rebuild_actions()
		{
			IDatabaseActionResolver resolver = new DatabaseActionResolver();

			IEnumerable<DatabaseAction> actions = resolver.GetActions(RequestedDatabaseAction.Rebuild);
			EnumerableAssert.That(actions, Is.EqualTo(new DatabaseAction[] { DatabaseAction.Drop, DatabaseAction.Create, DatabaseAction.Update }));
		}
	}
}