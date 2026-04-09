using Autofac;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Infrastructure.Data;
using Web.Infrastructure.Repositories;

namespace Web.Infrastructure.AutofacModules
{
    /// <summary>
    /// 仓储模块
    /// </summary>
    public class RepositoryModule : Autofac.Module
    {
        private readonly string _connectionString;

        public RepositoryModule(string connectionString)
        {
            _connectionString = connectionString;
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
                optionBuilder.UseMySql(_connectionString, new MySqlServerVersion(new Version(8, 0, 36)));
                return new MyDbContext(optionBuilder.Options);
            })
            .As<MyDbContext>()
            .InstancePerLifetimeScope();

            //注册仓库
            builder.RegisterGeneric(typeof(BaseRepository<>))
                   .As(typeof(IBaseRepository<>))
                   .InstancePerLifetimeScope();
        }
    }
}
