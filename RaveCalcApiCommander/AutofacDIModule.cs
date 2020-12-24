using Autofac;
using Microsoft.AspNetCore.Authorization;
using RaveCalcApiCommander.Abstraction;
using RaveCalcApiCommander.Authorization;
using RaveCalcApiCommander.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RaveCalcApiCommander
{
    public class AutofacDIModule : Module
    {
        public string ConnectionStringConfig { get; set; }
        public string DatabseNameConfig { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c =>
            {
                return new MongoDbSettings()
                {
                    ConnectionString = ConnectionStringConfig,
                    DatabaseName = DatabseNameConfig
                };
            }).As<IMongoDbSettings>();

            builder.RegisterGeneric(typeof(MongoDbRepository<>))
                .As(typeof(IMongoDbRepository<>))
                .SingleInstance();
            builder.RegisterType<UsersRepository>().As<IUserRepository>().SingleInstance();
            builder.RegisterType<MocRaveRepository>().As<IRaveRepository>().SingleInstance();
            builder.RegisterType<AuthHandler>().As<IAuthorizationHandler>();
            builder.RegisterType<MocEmbededResourceService>().As<IEmbededResourceService>().SingleInstance();
        }
    }
}
