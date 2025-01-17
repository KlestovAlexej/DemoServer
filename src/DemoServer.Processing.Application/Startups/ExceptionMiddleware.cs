﻿using Microsoft.AspNetCore.Http;
using OpenTelemetry.Trace;
using Acme.DemoServer.Processing.Api.Common;
using Acme.DemoServer.Processing.Model.Implements;
using Acme.DemoServer.Processing.Model.Interfaces;
using Acme.Wattle.Common.Exceptions;
using Acme.Wattle.DomainObjects.Interfaces;
using Acme.Wattle.Json.Extensions;
using Acme.Wattle.OpenTelemetry;
using System;
using System.Net;
using System.Threading.Tasks;

#pragma warning disable IDE0290
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Acme.DemoServer.Processing.Application.Startups;

public class ExceptionMiddleware
{
    private static readonly SpanAttributes SpanAttributes;

    private readonly ICustomEntryPoint m_entryPoint;
    private readonly RequestDelegate m_next;
    private readonly Tracer? m_tracer;

    static ExceptionMiddleware()
    {
        SpanAttributes = new SpanAttributes()
            .AddModuleType<ExceptionMiddleware>();
    }

    // ReSharper disable once ConvertToPrimaryConstructor
    public ExceptionMiddleware(
        ICustomEntryPoint entryPoint,
        RequestDelegate next,
        Tracer? tracer = null)
    {
        m_entryPoint = entryPoint;
        m_next = next;
        m_tracer = tracer;
    }

    // ReSharper disable once UnusedMember.Global
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await m_next(context).ConfigureAwait(false);
        }
        catch (WorkflowException exception)
        {
            using var mainSpan = m_tracer?.StartActiveSpan($"{nameof(IExceptionPolicy.ApplyAsync)}.{nameof(WorkflowException)}", initialAttributes: SpanAttributes, kind: SpanKind.Client);

            if (mainSpan != null)
            {
                mainSpan.SetStatus(Status.Error);
                mainSpan.RecordException(exception);
            }

            var workflowException = (WorkflowException)await m_entryPoint.ExceptionPolicy.ApplyAsync(exception).ConfigureAwait(false);

            using var span = m_tracer?.StartActiveSpan(nameof(HttpResponseWritingExtensions.WriteAsync), initialAttributes: SpanAttributes, kind: SpanKind.Client);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.Conflict;

            await context.Response.WriteAsync(workflowException.GetData().ToJsonText()).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            using var mainSpan = m_tracer?.StartActiveSpan($"{nameof(IExceptionPolicy.ApplyAsync)}.{nameof(Exception)}", initialAttributes: SpanAttributes, kind: SpanKind.Client);

            if (mainSpan != null)
            {
                mainSpan.SetStatus(Status.Error);
                mainSpan.RecordException(exception);
            }

            exception.Data.Add(ExceptionPolicy.ExceptionSourceModule, ExceptionPolicy.ExceptionSourceModuleAsController);

            var workflowException = (WorkflowException)await m_entryPoint.ExceptionPolicy.ApplyAsync(exception).ConfigureAwait(false);

            if (m_entryPoint.SystemSettings.DebugMode.Value)
            {
                workflowException.Data.Add(WellknownWorkflowExceptionDataKeys.Exception2, exception.ToString());
            }

            using var span = m_tracer?.StartActiveSpan(nameof(HttpResponseWritingExtensions.WriteAsync), initialAttributes: SpanAttributes, kind: SpanKind.Client);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.Conflict;

            await context.Response.WriteAsync(workflowException.GetData().ToJsonText()).ConfigureAwait(false);
        }
    }
}