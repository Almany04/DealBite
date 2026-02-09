using DealBite.Application.Mappings;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;


namespace DealBite.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            services.AddMediatR(cfg=>cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            return services;
        }
    }
}
