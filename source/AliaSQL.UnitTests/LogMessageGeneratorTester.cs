using AliaSQL.Core.Model;
using AliaSQL.Core.Services;
using AliaSQL.Core.Services.Impl;
using NUnit.Framework;

namespace AliaSQL.UnitTests
{
	[TestFixture]
	public class LogMessageGeneratorTester
	{
		[Test]
		public void Creates_initial_log_message_for_database_rebuild()
		{
			ILogMessageGenerator generator = new LogMessageGenerator();
            
			var settings = new ConnectionSettings("server", "db", true, null, null);
		    var taskAttributes = new TaskAttributes(settings, "c:\\scripts") {RequestedDatabaseAction = RequestedDatabaseAction.Rebuild};
			string message = generator.GetInitialMessage(taskAttributes);

			Assert.That(message, Is.EqualTo("Rebuild db on server using scripts from c:\\scripts\n"));
		}

		[Test]
		public void Creates_initial_log_message_for_database_create()
		{
			ILogMessageGenerator generator = new LogMessageGenerator();

			var settings = new ConnectionSettings("server", "db", true, null, null);
            var taskAttributes = new TaskAttributes(settings, "c:\\scripts");
			string message = generator.GetInitialMessage(taskAttributes);

			Assert.That(message, Is.EqualTo("Create db on server using scripts from c:\\scripts\n"));
		}

		[Test]
		public void Creates_initial_log_message_for_database_update()
		{
			ILogMessageGenerator generator = new LogMessageGenerator();

			var settings = new ConnectionSettings("server", "db", true, null, null);
            var taskAttributes = new TaskAttributes(settings, "c:\\scripts") {RequestedDatabaseAction = RequestedDatabaseAction.Update};
			string message = generator.GetInitialMessage(taskAttributes);

			Assert.That(message, Is.EqualTo("Update db on server using scripts from c:\\scripts\n"));
		}

		[Test]
		public void Creates_initial_log_message_for_database_drop()
		{
			ILogMessageGenerator generator = new LogMessageGenerator();

			var settings = new ConnectionSettings("server", "db", true, null, null);
            var taskAttributes = new TaskAttributes(settings, "c:\\scripts") {RequestedDatabaseAction = RequestedDatabaseAction.Drop};
			string message = generator.GetInitialMessage(taskAttributes);

			Assert.That(message, Is.EqualTo("Drop db on server\n"));
		}

	}
}