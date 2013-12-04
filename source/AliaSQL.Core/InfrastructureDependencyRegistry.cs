using StructureMap.Configuration.DSL;
using Tarantino.Core.DatabaseManager.Services;
using Tarantino.Core.DatabaseManager.Services.Impl;

namespace Tarantino.Infrastructure
{
	public class InfrastructureDependencyRegistry : Registry
	{
		protected override void configure()
		{
			Scan(y =>
			     	{
						y.TheCallingAssembly();
						y.WithDefaultConventions();
			     	});

			ForRequestedType<IDatabaseActionExecutor>()
				.AddInstances(y =>
				              	{
				              		y.OfConcreteType<DatabaseCreator>().Name = "Create";
				              		y.OfConcreteType<DatabaseDropper>().Name = "Drop";
				              		y.OfConcreteType<DatabaseUpdater>().Name = "Update";
				              	});

		}
	}
}