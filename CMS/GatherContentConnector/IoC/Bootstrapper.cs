using GatherContentConnector.IoC;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Bootstrapper), "Initialize")]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(Bootstrapper), "Dispose")]

namespace GatherContentConnector.IoC
{
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Castle.Windsor.Installer;

    /// <summary>
    /// 
    /// </summary>
    public static class Bootstrapper
    {
        private static IWindsorContainer _container;

        /// <summary>
        /// 
        /// </summary>
        public static void Dispose()
        {
            _container.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Initialize()
        {
            _container = new WindsorContainer().Install(FromAssembly.InDirectory(new AssemblyFilter("bin").FilterByName(a => a.Name.StartsWith("GatherContentConnector"))));
        }
    }
}