using ShtrihM.DemoServer.Common;
using ShtrihM.Wattle3.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ShtrihM.DemoServer.Processing.Model.Implements.SystemSettings;

[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
public sealed class SystemSettingsLocal
{
    public SystemSettingsLocal(IDictionary<Guid, string> values)
    {
        if (values == null)
        {
            throw new ArgumentNullException(nameof(values));
        }

        ProductId = GetValueAsGuid(values, WellknownCommonSystemSettings.ProductId);
        ProductVersion = GetValueAsVersion(values, WellknownCommonSystemSettings.ProductVersion);
        InstallationName = GetValue(values, WellknownCommonSystemSettings.InstallationName);
    }

    /// <summary>
    /// ��� �������� <seealso cref="WellknownCommonSystemSettings.ProductId"/>.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// ������ �������� <seealso cref="WellknownCommonSystemSettings.ProductVersion"/>.
    /// </summary>
    public Version ProductVersion { get; set; }

    /// <summary>
    /// ��� ����������� <seealso cref="WellknownCommonSystemSettings.InstallationName"/>.
    /// </summary>
    public string InstallationName { get; set; }

    private string GetValue(IDictionary<Guid, string> values, Guid id)
    {
        if (values.TryGetValue(id, out var result))
        {
            return result;
        }

        throw new InternalException($"�� ������� ��������� ��������� � ��������������� '{id}' ({WellknownCommonSystemSettings.GetDisplayName(id)}).");
    }

    private Version GetValueAsVersion(IDictionary<Guid, string> values, Guid id)
    {
        var text = GetValue(values, id);
        if (Version.TryParse(text, out var result))
        {
            return result;
        }

        throw new InternalException($"�� ������� ��������� �������� ��������� ��������� � ��������������� '{id}' ({WellknownCommonSystemSettings.GetDisplayName(id)}).");
    }

    private Guid GetValueAsGuid(IDictionary<Guid, string> values, Guid id)
    {
        var text = GetValue(values, id);
        if (Guid.TryParse(text, out var result))
        {
            return result;
        }

        throw new InternalException($"�� ������� ��������� �������� ��������� ��������� � ��������������� '{id}' ({WellknownCommonSystemSettings.GetDisplayName(id)}).");
    }
}