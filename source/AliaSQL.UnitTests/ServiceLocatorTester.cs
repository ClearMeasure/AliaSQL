using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Tarantino.Core.DatabaseManager.Services;
using Tarantino.DatabaseManager.Core;

namespace Tarantino.IntegrationTests.Core.Commons.Services.Configuration
{
	[TestFixture]
	public class ServiceLocatorTester 
	{
		[Test]
		public void Correctly_Constructs_Instance()
		{
            InfrastructureDependencyRegistrar.RegisterInfrastructure();
			IServiceLocator serviceLocator = new ServiceLocator();

			var instance = serviceLocator.CreateInstance<IResourceFileLocator>();

			Assert.That(instance, Is.Not.Null);
		}

		[Test]
		public void Correctly_Constructs_Instance_With_Key()
		{
            InfrastructureDependencyRegistrar.RegisterInfrastructure();
			IServiceLocator serviceLocator = new ServiceLocator();

			var instance = serviceLocator.CreateInstance<IDatabaseActionExecutor>("Create");

			Assert.That(instance, Is.Not.Null);
		}
	}
}