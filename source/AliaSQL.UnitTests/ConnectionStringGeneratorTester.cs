using AliaSQL.Core.Model;
using AliaSQL.Core.Services;
using AliaSQL.Core.Services.Impl;
using NUnit.Framework;

namespace AliaSQL.UnitTests
{
	[TestFixture]
	public class ConnectionStringGeneratorTester
	{
		[Test]
		public void CorrectlyGeneratesConnectionStringWithDatabaseAndIntegratedSecurity()
		{
			IConnectionStringGenerator generator = new ConnectionStringGenerator();

			ConnectionSettings settings = new ConnectionSettings("server", "db", true, string.Empty, string.Empty);
			string connString = generator.GetConnectionString(settings, true);
			
			Assert.AreEqual("Data Source=server;Initial Catalog=db;Integrated Security=True;", connString);
		}
		
		[Test]
		public void CorrectlyGeneratesConnectionStringWithDatabaseAndIntegratedSecurityAndDatabaseExcluded()
		{
			IConnectionStringGenerator generator = new ConnectionStringGenerator();

			ConnectionSettings settings = new ConnectionSettings("server", "db", true, string.Empty, string.Empty);
			string connString = generator.GetConnectionString(settings, false);
			
			Assert.AreEqual("Data Source=server;Integrated Security=True;", connString);
		}

		[Test]
		public void CorrectlyGeneratesConnectionStringWithDatabaseAndUserSecurity()
		{
			IConnectionStringGenerator generator = new ConnectionStringGenerator();

			ConnectionSettings settings = new ConnectionSettings("server", "db", false, "usr", "pwd");
			string connString = generator.GetConnectionString(settings, true);
			
			Assert.AreEqual("Data Source=server;Initial Catalog=db;User ID=usr;Password=pwd;", connString);
		}
		
		[Test]
		public void CorrectlyGeneratesConnectionStringWithDatabaseAndUserSecurityAndDatabaseExcluded()
		{
			IConnectionStringGenerator generator = new ConnectionStringGenerator();

			ConnectionSettings settings = new ConnectionSettings("server", "db", false, "usr", "pwd");
			string connString = generator.GetConnectionString(settings, false);

			Assert.AreEqual("Data Source=server;User ID=usr;Password=pwd;", connString);
		}
	}
}