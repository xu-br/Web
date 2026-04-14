using Autofac;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Infrastructure.Data;
using Web.Infrastructure.Repositories;
using Web.Infrastructure.Services;

namespace Web.Infrastructure.AutofacModules
{
    /// <summary>
    /// 仓储模块
    /// </summary>
    public class RepositoryModule : Autofac.Module
    {
        private readonly string _connectionString;
        private readonly string _redisConnectionString;

        public RepositoryModule(string connectionString, string redisConnectionString)
        {
            _connectionString = connectionString;
            _redisConnectionString = redisConnectionString;
        }

        /// <summary>
        /// 加载模块，注册DbContext和仓库
        /// </summary>
        /// <param name="builder"></param>
        protected override void Load(ContainerBuilder builder)
        {
            //注册DbContext数据库上下文
            builder.Register(x =>
            {
                var optionBuilder = new DbContextOptionsBuilder<MyDbContext>();
                optionBuilder.UseMySql(_connectionString, new MySqlServerVersion(new Version(9,5,0)));
                return new MyDbContext(optionBuilder.Options);
            })
            .As<MyDbContext>()
            .InstancePerLifetimeScope();

            // Redis 注册
            builder.RegisterInstance(new RedisService(_redisConnectionString))
                   .AsSelf()
                   .SingleInstance();

            //注册仓库
            builder.RegisterGeneric(typeof(BaseRepository<>))
                   .As(typeof(IBaseRepository<>))
                   .InstancePerLifetimeScope();
        }
    }
}
