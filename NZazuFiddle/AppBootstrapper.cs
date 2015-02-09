using System.Windows;
using Autofac;
using Caliburn.Micro;
using Caliburn.Micro.Autofac;
using NZazu;
using NZazu.Layout;
using NZazu.Xceed;

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
            builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();

            builder.RegisterInstance(Example.FormDefinition);
            builder.RegisterInstance(Example.FormData);
            builder.RegisterType<FormDefinitionViewModel>().As<IFormDefinitionViewModel>().SingleInstance();
            builder.RegisterType<FormDataViewModel>().As<IFormDataViewModel>().SingleInstance();
            builder.RegisterType<PreviewViewModel>().As<IPreviewViewModel>().SingleInstance();

            builder.RegisterType<XceedFieldFactory>().As<INZazuWpfFieldFactory>();
            builder.RegisterType<GridLayoutStrategy>().As<INZazuWpfLayoutStrategy>();
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