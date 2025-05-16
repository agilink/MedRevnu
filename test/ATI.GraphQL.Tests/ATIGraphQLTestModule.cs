using Abp.Modules;
using Abp.Reflection.Extensions;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using ATI.Configure;
using ATI.Startup;
using ATI.Test.Base;

namespace ATI.GraphQL.Tests
{
    [DependsOn(
        typeof(ATIGraphQLModule),
        typeof(ATITestBaseModule))]
    public class ATIGraphQLTestModule : AbpModule
    {
        public override void PreInitialize()
        {
            IServiceCollection services = new ServiceCollection();
            
            services.AddAndConfigureGraphQL();

            WindsorRegistrationHelper.CreateServiceProvider(IocManager.IocContainer, services);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ATIGraphQLTestModule).GetAssembly());
        }
    }
}