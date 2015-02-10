using System.Reflection;
using System.Windows;
using Autofac;
using Caliburn.Micro.Autofac;
using NZazu;
using NZazu.Xceed;

namespace Sample
{
    public class AppBootstrapper : AutofacBootstrapper<ShellViewModel>
    {
        public AppBootstrapper()
        {
            Initialize();
        }
        protected override void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
               .Where(t => t.Namespace != null && t.Namespace.Contains("Samples"))
               .AsImplementedInterfaces();

            builder.RegisterType<XceedFieldFactory>().As<INZazuWpfFieldFactory>().SingleInstance();
        }

        protected override void ConfigureBootstrapper()
        {
            base.ConfigureBootstrapper();
            EnforceNamespaceConvention = false;
        }
        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }
    }
}