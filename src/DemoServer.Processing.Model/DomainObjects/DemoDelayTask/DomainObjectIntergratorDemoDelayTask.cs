﻿using ShtrihM.DemoServer.Processing.Generated.Interface;
using ShtrihM.DemoServer.Processing.Model.Interfaces;
using ShtrihM.Wattle3.DomainObjects.DomainObjectActivators;
using ShtrihM.Wattle3.DomainObjects.DomainObjectDataMappers;
using ShtrihM.Wattle3.DomainObjects.DomainObjectIntergrators;
using ShtrihM.Wattle3.DomainObjects.DomainObjectsRegisters;
using Unity;

namespace ShtrihM.DemoServer.Processing.Model.DomainObjects.DemoDelayTask;

[DomainObjectIntergrator]
// ReSharper disable once UnusedMember.Global
public class DomainObjectIntergratorDemoDelayTask : BaseDomainObjectIntergrator<IUnityContainer>
{
    protected override void DoRun(IUnityContainer container)
    {
        var entryPoint = container.Resolve<ICustomEntryPoint>();
        var dataMapper =
            DomainObjectDataMapperNoDeleteDefaultFactory.Create<IMapperDemoDelayTask>(
                entryPoint.Context,
                entryPoint.SystemSettings.IdentityCachesSettings.Value.DemoDelayTask.Value,
                identityGroupId: entryPoint.PartitionsDay);
        container.Resolve<DomainObjectDataMappers>().AddMapper(dataMapper);

        container.Resolve<DomainObjectRegisters>().AddRegister(
            new DomainObjectRegisterStateless(
                entryPoint.Context,
                dataMapper,
                new DomainObjectDataActivatorForActualStateDtoDefault<DemoDelayTaskDtoActual, DomainObjectDemoDelayTask>(entryPoint),
                new DomainObjectActivatorDefault<DomainObjectDemoDelayTask.Template, DomainObjectDemoDelayTask>(entryPoint.UnitOfWorkProvider, entryPoint)));
    }
}