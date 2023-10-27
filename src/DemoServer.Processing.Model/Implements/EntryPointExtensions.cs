﻿using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ShtrihM.DemoServer.Processing.DataAccess.PostgreSql.EfModels;
using ShtrihM.DemoServer.Processing.Model.Implements.ControllersServices;
using ShtrihM.DemoServer.Processing.Model.Interfaces;
using ShtrihM.Wattle3.DomainObjects.Interfaces;
using ShtrihM.Wattle3.Primitives;
using System;
using ShtrihM.Wattle3.DomainObjects;

namespace ShtrihM.DemoServer.Processing.Model.Implements;

public static class EntryPointExtensions
{
    public static void RegisterGlobals()
    {
        /* NONE */
    }

    // ReSharper disable once UnusedMethodReturnValue.Global
    public static WebApplication AddEntryPointServices(this WebApplication application)
    {
        /* NONE */

        return application;
    }

    // ReSharper disable once UnusedMethodReturnValue.Global
    public static IServiceCollection AddEntryPointServices(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.AddSingleton(
            _ => (ICustomEntryPoint)ServiceProviderHolder.Instance.GetRequiredService<IEntryPoint>());

        services.AddSingleton(
            provider =>
            {
                var entryPoint = provider.GetRequiredService<ICustomEntryPoint>();
                var service = new DemoObjectControllerService(entryPoint);
                var result = 
                    LoggingDispatchProxy<IDemoObjectControllerService>.CreateProxy(
                        service, 
                        entryPoint.LoggerFactory.CreateLogger<DemoObjectControllerService>(), 
                        entryPoint.Tracer);

                return result;
            });

        services.AddSingleton(
            provider =>
            {
                var entryPoint = provider.GetRequiredService<ICustomEntryPoint>();
                var service = new ServerControllerService(entryPoint);
                var result =
                    LoggingDispatchProxy<IServerControllerService>.CreateProxy(
                        service,
                        entryPoint.LoggerFactory.CreateLogger<ServerControllerService>(),
                        entryPoint.Tracer);

                return result;
            });

        services.AddPooledDbContextFactory<ProcessingDbContext>(
            o =>
            {
                // Строка должна быть неизменной т.к. используется в скрипте создания контекста EF.
                // Смотри тест ShtrihM.DemoServer.Processing.Tests.DataAccess.PostgreSql.TestsCreateEntityFrameworkDbContext
                o.UseModel(DataAccess.PostgreSql.EfModelsOptimized.ProcessingDbContextModel.Instance);

                o.UseNpgsql();

                if (o.Options.FindExtension<CoreOptionsExtension>()!.Model == null)
                {
                    throw new InvalidOperationException("Модель не определена.");
                }
            }
#if DEBUG
            , 1
#endif
        );

        return services;
    }
}
