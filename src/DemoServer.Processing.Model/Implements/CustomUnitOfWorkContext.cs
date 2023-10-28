﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShtrihM.DemoServer.Processing.DataAccess.PostgreSql.EfModels;
using ShtrihM.Wattle3.DomainObjects.Interfaces;
using ShtrihM.Wattle3.DomainObjects.UnitOfWorkLocks;
using ShtrihM.Wattle3.DomainObjects.UnitOfWorks;
using ShtrihM.Wattle3.Mappers.Interfaces;
using System;
using System.Runtime.CompilerServices;
using ShtrihM.DemoServer.Processing.Generated.Interface;
using ShtrihM.Wattle3.QueueProcessors.Interfaces;

namespace ShtrihM.DemoServer.Processing.Model.Implements;

public sealed class CustomUnitOfWorkContext : UnitOfWorkContext
{
    public CustomUnitOfWorkContext(
        IEntryPoint entryPoint,
        IDomainObjectDataMappers dataMappers,
        IMappers mappers,
        IExceptionPolicy exceptionPolicy,
        WorkflowExceptionPolicy workflowExceptionPolicy,
        ILogger logger,
        bool addStackTrace,
        IUnitOfWorkProvider unitOfWorkProvider,
        IDbContextFactory<ProcessingDbContext> pooledDbContextFactory,
        IServiceProvider serviceProvider,
        IUnitOfWorkLocksHub unitOfWorkLocksHub,
        IQueueItemProcessor queueEmergencyDomainBehaviour,
        IMapperChangeTracker mapperChangeTracker)
        : base(
            entryPoint,
            dataMappers,
            mappers,
            exceptionPolicy,
            workflowExceptionPolicy,
            logger,
            addStackTrace,
            unitOfWorkProvider,
            serviceProvider)
    {
        QueueEmergencyDomainBehaviour = queueEmergencyDomainBehaviour ?? throw new ArgumentNullException(nameof(queueEmergencyDomainBehaviour));
        UnitOfWorkLocksHub = unitOfWorkLocksHub ?? throw new ArgumentNullException(nameof(unitOfWorkLocksHub));
        PooledDbContextFactory = pooledDbContextFactory ?? throw new ArgumentNullException(nameof(pooledDbContextFactory));
        MapperChangeTracker = mapperChangeTracker ?? throw new ArgumentNullException(nameof(mapperChangeTracker));
    }

    public readonly IMapperChangeTracker MapperChangeTracker;
    public readonly IQueueItemProcessor QueueEmergencyDomainBehaviour;
    public readonly IUnitOfWorkLocksHub UnitOfWorkLocksHub;
    public readonly IDbContextFactory<ProcessingDbContext> PooledDbContextFactory;

    public new WorkflowExceptionPolicy WorkflowExceptionPolicy
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (WorkflowExceptionPolicy)base.WorkflowExceptionPolicy;
    }
}