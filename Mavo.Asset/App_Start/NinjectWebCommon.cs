[assembly: WebActivator.PreApplicationStartMethod(typeof(Mavo.Assets.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(Mavo.Assets.App_Start.NinjectWebCommon), "Stop")]

namespace Mavo.Assets.App_Start
{
    using System;
    using System.Web;
    using Mavo.Assets.Models;
    using Mavo.Assets.Services;
    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using Ninject;
    using Ninject.Web.Common;
    using NinjectAdapter;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            
            RegisterServices(kernel);

            ServiceLocator.SetLocatorProvider(() => new NinjectServiceLocator(kernel));


            return kernel;
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<AssetContext>().ToSelf().InRequestScope();
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
            kernel.Bind<IAssetPicker>().To<AssetPicker>();
            kernel.Bind<IAssetActivityManager>().To<AssetActivityMananger>();
            kernel.Bind<ICurrentUserService>().To<CurrentUserService>();
        }        
    }
}
