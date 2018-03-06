﻿using Autofac;
using NZazuFiddle.Samples;
using NZazuFiddle.TemplateManagement;
using NZazuFiddle.TemplateManagement.Contracts;

namespace NZazuFiddle
{
    class TemplateModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ElasticSearchTemplateDbClient>().As<ITemplateDbClient>().SingleInstance();
            builder.RegisterType<JsonTemplateFileIo>().As<ITemplateFileIo>().SingleInstance();
            builder.RegisterType<TemplateSample>().As<IHaveSample>();
            builder.RegisterType<TemplateRepoManager>().As<ITemplateRepoManager>().SingleInstance();
        }
    }
}