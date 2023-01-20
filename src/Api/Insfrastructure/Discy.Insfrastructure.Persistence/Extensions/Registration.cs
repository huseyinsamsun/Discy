using Discy.Insfrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discy.Insfrastructure.Persistence.Extensions
{
    public static class Registration
    {
        public static IServiceCollection AddInfrastructureRegistration(this IServiceCollection services, IConfiguration configuration)
        {
            var connStr = configuration["DiscyDbConnectionString"].ToString();
            services.AddDbContext<DiscyContext>(conf =>
            {
                conf.UseSqlServer("connStr", opt =>
                {
                    opt.EnableRetryOnFailure(); //veri tabanında hata olursa
                });
            });
            //var seedData = new SeedData();
            //seedData.SeedAsync(configuration).GetAwaiter().GetResult();
            return services;
        }
    }
}
