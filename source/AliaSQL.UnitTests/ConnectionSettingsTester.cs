using AliaSQL.Core.Model;
using NUnit.Framework;

namespace AliaSQL.UnitTests
{
	[TestFixture]
	public class ConnectionSettingsTester
	{
		[Test]
		public void PropertyAccessorsWorkProperly()
		{
			ConnectionSettings settings = new ConnectionSettings("server", "database", true, "username", "password");

			Assert.AreEqual("server", settings.Server);
			Assert.AreEqual("database", settings.Database);
			Assert.AreEqual(true, settings.IntegratedAuthentication);
			Assert.AreEqual("username", settings.Username);
			Assert.AreEqual("password", settings.Password);
		}

		[Test]
		public void ProperlyComparesTwoIdenticalConnectionSettings()
		{
			ConnectionSettings settings1 = new ConnectionSettings("server", "database", true, "username", "password");
			ConnectionSettings settings2 = new ConnectionSettings("server", "database", true, "username", "password");
			
			Assert.AreEqual(settings1, settings2);
		}

		[Test]
		public void ProperlyComparesTwoNonIdenticalConnectionSettings()
		{
			ConnectionSettings settings1 = new ConnectionSettings("server", "database", true, "username", "password");
			ConnectionSettings settings2 = new ConnectionSettings("server1", "database", true, "username", "password");

			Assert.AreNotEqual(settings1, settings2);
		}

		[Test]
		public void Calculates_correct_hash_code()
		{
			ConnectionSettings settings1 = new ConnectionSettings("Server", "Database", true, "Username", "Password");
			int expectedHashCode = "ServerDatabaseUsernamePasswordTrue".GetHashCode();
			Assert.That(settings1.GetHashCode(), Is.EqualTo(expectedHashCode));
		}
	}
}