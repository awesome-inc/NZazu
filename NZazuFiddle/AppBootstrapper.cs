using System.Reflection;
using System.Windows;
using Autofac;
using Caliburn.Micro.Autofac;
using NZazuFiddle.Samples;

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
            builder.RegisterType<ShellViewModel>().As<IShell>().SingleInstance();
            builder.RegisterType<SampleTemplate>().As<IHaveSample>().AsImplementedInterfaces();

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