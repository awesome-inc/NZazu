using System.Windows;
using Autofac;
using Caliburn.Micro.Autofac;

namespace NZazuFiddle
{
    public class AppBootstrapper : AutofacBootstrapper<ShellViewModel>
    {
        public AppBootstrapper()
        {
            Initialize();
        }
        protected override void ConfigureContainer(ContainerBuilder builder)
        {
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