using System.Collections.Generic;
using System.Windows;
using Autofac;
using Caliburn.Micro;
using Caliburn.Micro.Autofac;
using NZazuFiddle.Samples;
using NZazuFiddle.TemplateManagement;
using NZazuFiddle.TemplateManagement.Contracts;
using NZazuFiddle.TemplateManagement.Data;

namespace NZazuFiddle
{
    public class AppBootstrapper : AutofacBootstrapper<FiddleViewModel>
    {
        public AppBootstrapper()
        {
            Initialize();
        }

        protected override void ConfigureContainer(ContainerBuilder builder)
        {

            var globalEvents = new EventAggregator();
            var session = new Session("", new List<ISample>(), globalEvents);

            builder.RegisterInstance(session).As<ISession>().SingleInstance();
            builder.RegisterInstance(globalEvents).As<IEventAggregator>().SingleInstance();
            builder.RegisterType<ShellViewModel>().As<IShell>().SingleInstance();
            builder.RegisterType<EndpointViewModel>().As<IEndpointViewModel>().SingleInstance();
            builder.RegisterType<FileMenuViewModel>().As<IFileMenuViewModel>().SingleInstance();
            builder.RegisterType<AddTemplateViewModel>().As<IAddTemplateViewModel>().SingleInstance();

            builder.RegisterModule<TemplateModule>();

            //register all samples
            //builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
            //   .Where(t => typeof(IHaveSample).IsAssignableFrom(t))
            //   .AsImplementedInterfaces();
        }

        protected override void ConfigureBootstrapper()
        {
            base.ConfigureBootstrapper();
            EnforceNamespaceConvention = false;
        }
        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<IShell>();
        }
    }
}