using Autofac;
using B2B.API.Helpers;
using B2B.Dao.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace B2B.API.Modules
{
    public class AutofacModuleRegister : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<JwtHelper>().SingleInstance();

            // Register Entity Framework
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<B2BDbContext>().UseOracle("Data Source=10.18.1.162:1521/EVA;User ID=REX;Password=rex#1018");

            builder.RegisterType<B2BDbContext>()
                   .WithParameter("options", dbContextOptionsBuilder.Options)
                   .InstancePerLifetimeScope();


            //根據名稱約定（服務層的介面和實現均以Service結尾），實現服務介面和服務實現的依賴
            builder.RegisterAssemblyTypes(Assembly.Load("B2B.Service"))
                   .Where(t => t.Name.EndsWith("Service"))
                   .AsImplementedInterfaces();
        }
    }
}
