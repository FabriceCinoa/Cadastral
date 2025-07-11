using Common.Library.Logger;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Common.Library
{
    public static  class Loggers
    {

        private static  Type  LoggerType { get; set; }
        public static IServiceCollection AddLogger(this IServiceCollection services, IConfiguration configuration)
        {
            var loggerType = typeof(ConsoleLogger) ;
            if (configuration["Logging:Type"] != null)
            {
                try
                {
                    if( LoggerType == null)
                        LoggerType= Assembly.GetExecutingAssembly().GetTypes().Where(x => x.Name.Contains(configuration["Logging:Type"])).FirstOrDefault();

                    if (LoggerType != null && LoggerType.GetInterface("ILogger") != null)
                    {
                        loggerType = LoggerType;
                    }
                    
                } catch { }
            }
            services.AddSingleton(typeof(ILogger), loggerType);
            return services;
        }
        
    }
}
