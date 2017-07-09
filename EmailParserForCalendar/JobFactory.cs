using System;
using FluentScheduler;
using Microsoft.Extensions.DependencyInjection;

namespace EmailParserForCalendar
{
    public class JobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public JobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public IJob GetJobInstance<T>() where T : IJob
        {
            var t = typeof(T);
            T service = _serviceProvider.GetRequiredService<T>();
            return service;
        }
    }
}