﻿using Microsoft.Extensions.Logging;
using ShtrihM.DemoServer.Processing.Api.Common;
using ShtrihM.DemoServer.Processing.Common;
using ShtrihM.DemoServer.Processing.Model.Interfaces;
using ShtrihM.Wattle3.DomainObjects.DomainObjects;
using ShtrihM.Wattle3.DomainObjects.Interfaces;
using ShtrihM.Wattle3.Mappers.Primitives.MutableFields;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ShtrihM.DemoServer.Processing.Generated.Interface;
using ShtrihM.Wattle3.DomainObjects.DomainObjectActivators;
using ShtrihM.Wattle3.Primitives;

#pragma warning disable CA2254
#pragma warning disable IDE0052

namespace ShtrihM.DemoServer.Processing.Model.DomainObjects.DemoObjectX;

[DomainObjectDataMapper(WellknownMappersAsText.DemoObjectX, DomainObjectDataTarget.Create, DomainObjectDataTarget.Update, DomainObjectDataTarget.Delete)]
public sealed class DomainObjectDemoObjectX : DomainObjectMutable<DomainObjectDemoObjectX>, IDomainObjectDemoObjectX, IDomainObjectActivatorPostCreate
{
    [DomainObjectFieldValue(DomainObjectDataTarget.Create, DomainObjectDataTarget.Update, DtoFiledName = nameof(DemoObjectXDtoChanged.Enabled))]
    private MutableField<bool> m_enabled;

    [DomainObjectFieldValue(DomainObjectDataTarget.Create, DomainObjectDataTarget.Update, DtoFiledName = nameof(DemoObjectXDtoChanged.Name))]
    private MutableFieldStringLimitedEx m_name;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DomainObjectDemoObjectX(
        ICustomEntryPoint entryPoint,
        DemoObjectXDtoActual data)
        : base(entryPoint, data)
    {
        CreateDate = data.CreateDate.SpecifyKindLocal();
        ModificationDate = data.ModificationDate.SpecifyKindLocal();
        m_name = new MutableFieldStringLimitedEx(FieldsConstants.DemoObjectXNameMaxLength, data.Name);
        m_enabled = new MutableField<bool>(data.Enabled);
        Key1 = data.Key1;
        Key2 = data.Key2;
        Group = data.Group;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DomainObjectDemoObjectX(
        ICustomEntryPoint entryPoint,
        long identity,
        DomainObjectTemplateDemoObjectX template)
        : base(entryPoint, identity)
    {
        CreateDate = m_entryPoint.TimeService.NowDateTime;
        ModificationDate = CreateDate;
        m_name = new MutableFieldStringLimitedEx(FieldsConstants.DemoObjectXNameMaxLength, template.Name);
        m_enabled = new MutableField<bool>(template.Enabled);
        Key1 = template.Key1;
        Key2 = template.Key2;
        Group = template.Group;
    }

    public override Guid TypeId => WellknownDomainObjects.DemoObjectX;

    public bool Enabled
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_enabled.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            m_enabled.SetValue(value);
            if (m_enabled.Changed)
            {
                DoUpdate();
            }
        }
    }

    public string Name
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_name.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            m_name.SetValue(value);
            if (m_name.Changed)
            {
                DoUpdate();
            }
        }
    }

    [DomainObjectFieldValue(DomainObjectDataTarget.Create, DomainObjectDataTarget.Update)]
    public DateTime CreateDate
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }

    [DomainObjectFieldValue(DomainObjectDataTarget.Create, DomainObjectDataTarget.Update)]
    public Guid Key1
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }

    [DomainObjectFieldValue(DomainObjectDataTarget.Create, DomainObjectDataTarget.Update)]
    public string Key2
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }

    [DomainObjectFieldValue(DomainObjectDataTarget.Create, DomainObjectDataTarget.Update)]
    public long Group
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }

    [DomainObjectFieldValue(DomainObjectDataTarget.Create, DomainObjectDataTarget.Update)]
    public DateTime ModificationDate
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private set;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DemoObjectXAlternativeKey GetKey()
    {
        var result = new DemoObjectXAlternativeKey(Key1, Key2);

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Delete()
    {
        m_entryPoint.CurrentUnitOfWork.AddDelete(this);

        var register = (DomainObjectRegisterDemoObjectX)m_entryPoint.Registers.GetRegister<IDomainObjectRegisterDemoObjectX>();
        var domainBehaviour = m_entryPoint.CreateDomainBehaviourWithСonfirmation();

        register.RemoveDomainObject(domainBehaviour, Identity, GetKey());
    }

    public void PostCreate()
    {
        DoPostCreate();

        var register = (DomainObjectRegisterDemoObjectX)m_entryPoint.Registers.GetRegister<IDomainObjectRegisterDemoObjectX>();
        var domainBehaviour = m_entryPoint.CreateDomainBehaviourWithСonfirmation();

        register.AddDomainObject(domainBehaviour, Identity, GetKey(), Group);
    }

    public ValueTask PostCreateAsync(CancellationToken cancellationToken)
    {
        DoPostCreate();

        var register = (DomainObjectRegisterDemoObjectX)m_entryPoint.Registers.GetRegister<IDomainObjectRegisterDemoObjectX>();
        var domainBehaviour = m_entryPoint.CreateDomainBehaviourWithСonfirmation();

        return register.AddDomainObjectAsync(domainBehaviour, Identity, GetKey(), Group, cancellationToken);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DoPostCreate()
    {
        var domainBehaviour = m_entryPoint.CreateDomainBehaviourWithСonfirmation();

        m_entryPoint.UnitOfWorkProvider.Instance.AddBehaviour(domainBehaviour);

        var identity = Identity;

        domainBehaviour.SetFailAll(
            () =>
            {
                var logger = m_entryPoint.LoggerFactory.CreateLogger<DomainObjectDemoObjectX>();
                var messgae = $"Не удалось создать объект X с идентификатором '{identity}'.";
                logger.LogError(messgae);
                Console.WriteLine(messgae);
            });

        domainBehaviour.SetSuccessfulAll(
            () =>
            {
                var logger = m_entryPoint.LoggerFactory.CreateLogger<DomainObjectDemoObjectX>();
                var messgae = $"Создан объект X с идентификатором '{identity}'.";
                logger.LogError(messgae);
                Console.WriteLine(messgae);
            });
    }

    protected override void DoUpdate()
    {
        base.DoUpdate();

        ModificationDate = m_entryPoint.TimeService.NowDateTime;
    }
}