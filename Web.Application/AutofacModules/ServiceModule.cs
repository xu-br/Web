using Autofac;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Web.Application.Mapper;

namespace Web.Application.AutofacModules
{
    /// <summary>
    /// 服务模块
    /// </summary>
    public class ServiceModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // 注册AutoMapper
            var mapperConfig = new MapperConfiguration(x =>
            {
                x.AddProfile<MappingProFile>();
            });

            // 注册IMapper实例
            builder.RegisterInstance(mapperConfig.CreateMapper()).As<IMapper>().SingleInstance();

            // 批量注册服务
            var serviceAssembly = Assembly.GetExecutingAssembly();
            builder.RegisterAssemblyTypes(serviceAssembly)
                .Where(t => t.Name.EndsWith("Service"))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}
