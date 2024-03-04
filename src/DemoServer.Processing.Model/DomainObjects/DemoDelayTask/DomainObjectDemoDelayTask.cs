﻿using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ShtrihM.DemoServer.Processing.Common;
using ShtrihM.DemoServer.Processing.Generated.Interface;
using ShtrihM.DemoServer.Processing.Model.DomainObjects.Common;
using ShtrihM.DemoServer.Processing.Model.DomainObjects.DemoDelayTask.Scenarios;
using ShtrihM.DemoServer.Processing.Model.DomainObjects.DemoDelayTask.ScenarioStates;
using ShtrihM.DemoServer.Processing.Model.Interfaces;
using ShtrihM.Wattle3.CodeGeneration.Generators;
using ShtrihM.Wattle3.Common.Exceptions;
using ShtrihM.Wattle3.DomainObjects.DomainObjects;
using ShtrihM.Wattle3.DomainObjects.Interfaces;
using ShtrihM.Wattle3.DomainObjects.Json;
using ShtrihM.Wattle3.Mappers.Primitives.MutableFields;

namespace ShtrihM.DemoServer.Processing.Model.DomainObjects.DemoDelayTask;

[DomainObjectDataMapper]
// ReSharper disable once ClassNeverInstantiated.Global
public sealed class DomainObjectDemoDelayTask : BaseDomainObjectMutable<DomainObjectDemoDelayTask>, IDomainObjectDemoDelayTask
{
    #region Template - шаблон создания объекта

    /// <summary>
    /// Шаблон создания объекта <see cref="DomainObjectDemoDelayTask"/>.
    /// </summary>
    public class Template : IDomainObjectTemplate
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // ReSharper disable once ConvertToPrimaryConstructor
        public Template(
            string scenario, 
            DateTimeOffset? startDate)
        {
            Scenario = scenario;
            StartDate = startDate;
        }

        /// <summary>
        /// <seealso cref="DemoDelayTaskScenario"/>.
        /// </summary>
        public readonly string Scenario;

        public readonly DateTimeOffset? StartDate;
    }

    #endregion

    #region Изменяемы поля

    [DomainObjectFieldValue]
    private readonly MutableField<bool> m_available;

    [DomainObjectFieldValue]
    private readonly MutableFieldNullable<DateTimeOffset> m_startDate;

    [DomainObjectFieldValue]
    private readonly FieldWithModel<string, DemoCycleTaskScenarioState> m_scenarioState;

    #endregion

    #region Конструкторы

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // ReSharper disable once UnusedMember.Global
    public DomainObjectDemoDelayTask(
        DemoDelayTaskDtoActual data,
        ICustomEntryPoint entryPoint)
        : base(entryPoint, data)
    {
        m_available = new MutableField<bool>(data.Available);
        CreateDate = data.CreateDate;
        ModificationDate = data.ModificationDate;
        Scenario = data.Scenario;

        m_startDate = 
            new MutableFieldNullable<DateTimeOffset>(
                // Дата-время в БД хранится с ограниченной точность.
                DbTypesCorrector.DateTimeOffset(data.StartDate));

        m_scenarioState =
            new FieldWithModel<string, DemoCycleTaskScenarioState>(
                m_entryPoint.JsonDeserializer,
                new MutableField<string>(data.ScenarioState));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // ReSharper disable once UnusedMember.Global
    public DomainObjectDemoDelayTask(
        long identity,
        Template template,
        ICustomEntryPoint entryPoint)
        : base(entryPoint, identity)
    {
        m_available = new MutableField<bool>(true);
        CreateDate = entryPoint.TimeService.Now;
        ModificationDate = CreateDate;
        Scenario = template.Scenario;
        m_startDate = new MutableFieldNullable<DateTimeOffset>(template.StartDate);

        var scenario = m_entryPoint.JsonDeserializer.DeserializeReadOnly<DemoDelayTaskScenario>(Scenario);
        var scenarioState = string.Empty;

        switch (scenario)
        {
            case DemoDelayTaskScenarioAsDelay:
                /* NONE */

                break;

            case DemoDelayTaskScenarioAsCycle:
            {
                var scenarioStateAsCycle =
                    new DemoCycleTaskScenarioStateAsCycle
                    {
                        Index = 0,
                        RunDate = [],
                    };
                scenarioState = m_entryPoint.JsonDeserializer.SerializeReadOnly(scenarioStateAsCycle);

                break;
            }

            default:
                throw new InternalException($"Неизвестный тип сценария '{scenario.GetType().Assembly}'.");
        }

        m_scenarioState =
            new FieldWithModel<string, DemoCycleTaskScenarioState>(
                m_entryPoint.JsonDeserializer,
                new MutableField<string>(scenarioState));
    }

    #endregion

    public override Guid TypeId => WellknownDomainObjects.DemoDelayTask;

    [DomainObjectFieldValue]
    public DateTimeOffset CreateDate
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }

    [DomainObjectFieldValue]
    public DateTimeOffset ModificationDate
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private set;
    }

    [DomainObjectFieldValue]
    public string Scenario
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }

    public string ScenarioState
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_scenarioState.Value;
    }

    public DateTimeOffset? StartDate
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_startDate.Value;
    }

    public async ValueTask<(bool IsCompleted, CancellationToken? CommitCancellationToken)> ProcessAsync(bool isRemoved, long count, CancellationToken cancellationToken)
    {
        var scenario = m_entryPoint.JsonDeserializer.DeserializeReadOnly<DemoDelayTaskScenario>(Scenario);

        if (scenario is DemoDelayTaskScenarioAsDelay scenarioAsDelay)
        {
            if (count > 1)
            {
                throw new InternalException("Что-то сломалось, задача исполняется несколько раз.");
            }

            Console.WriteLine($"[{m_entryPoint.TimeService.NowDateTime:O}] DemoDelayTask.Id:{Identity} ({scenarioAsDelay.Type}) - Начало ожидания '{scenarioAsDelay.Delay}' ...");
            await Task.Delay(scenarioAsDelay.Delay, cancellationToken).ConfigureAwait(false);
            Console.WriteLine($"[{m_entryPoint.TimeService.NowDateTime:O}] DemoDelayTask.Id:{Identity} ({scenarioAsDelay.Type}) - Конец ожидания.");

            m_available.SetValue(false);
        }
        else if (scenario is DemoDelayTaskScenarioAsCycle scenarioAsCycle)
        {
            if (count > scenarioAsCycle.Count)
            {
                throw new InternalException("Что-то сломалось, задача исполняется слишком много раз.");
            }

            var scenariostate = (DemoCycleTaskScenarioStateAsCycle)m_scenarioState.AsWrite;

            Console.WriteLine($"[{m_entryPoint.TimeService.NowDateTime:O}] DemoDelayTask.Id:{Identity} ({scenarioAsCycle.Type}) - Начало исполнения '{scenariostate.Index}' ...");

            if (scenariostate.Index < scenarioAsCycle.Count)
            {
                scenariostate.Index++;

                var now = m_entryPoint.TimeService.Now;
                scenariostate.RunDate.Add(now);

                if (scenarioAsCycle.NextRunTimeout.HasValue)
                {
                    m_startDate.SetValue(
                        // Дата-время в БД хранится с ограниченной точность.
                        DbTypesCorrector.DateTimeOffset(now + scenarioAsCycle.NextRunTimeout.Value));
                }
                else
                {
                    m_startDate.SetValue(null);
                }
            }

            Console.WriteLine($"[{m_entryPoint.TimeService.NowDateTime:O}] DemoDelayTask.Id:{Identity} ({scenarioAsCycle.Type}) - Конец исполнения '{scenariostate.Index}' [{(m_startDate.Value?.ToString("O") ?? "НЕТ ДАТЫ")}].");

            m_available.SetValue(scenariostate.Index < scenarioAsCycle.Count);
        }
        else
        {
            throw new InternalException($"Неизвестный тип сценария '{scenario.GetType().Assembly}'.");
        }

        await DoUpdateAsync(cancellationToken).ConfigureAwait(false);

        if (m_available.Value)
        {
            return (false, null);
        }

        return (true, null);
    }

    protected override ValueTask DoUpdateAsync(CancellationToken cancellationToken = default)
    {
        ModificationDate = m_entryPoint.TimeService.Now;

        return base.DoUpdateAsync(cancellationToken);
    }
}
