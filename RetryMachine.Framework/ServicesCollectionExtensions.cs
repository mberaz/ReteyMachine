using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace RetryMachine.Framework
{
    public static class ServicesCollectionExtensions
    {
        public static void RegisterRetryMachine(this IServiceCollection serviceCollection,
            bool shouldRegisterImplementations = true, int delayInSeconds = 30)
        {
            serviceCollection.AddSingleton(new RetrySettings { DelayInSeconds = delayInSeconds });
            serviceCollection.AddScoped<IRetryMachineRunner, RetryMachineRunner>();

            if (shouldRegisterImplementations)
            {
                RegisterImplementations(serviceCollection, typeof(IRetryable));
            }
        }

        public static void RegisterImplementations(IServiceCollection services, Type interfaceType)
        {
            var concreteTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => interfaceType.IsAssignableFrom(p) && !p.IsInterface);

            foreach (var type in concreteTypes)
            {
                services.AddTransient(interfaceType, type);
            }
        }
    }
}
