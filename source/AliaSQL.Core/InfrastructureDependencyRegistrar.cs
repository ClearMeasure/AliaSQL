using StructureMap;
using Tarantino.Infrastructure;

namespace Tarantino.DatabaseManager.Core
{
    public class InfrastructureDependencyRegistrar
    {
        public static void RegisterInfrastructure()
        {
            ObjectFactory.Initialize(x => x.Scan(s =>
                                                     {
                                                         s.Assembly(typeof(InfrastructureDependencyRegistrar).Assembly);
                                                         //s.AssemblyContainingType<InfrastructureDependencyRegistry>();
                                                         s.LookForRegistries();
                                                         s.WithDefaultConventions();
                                                     }));
        }
    }
}