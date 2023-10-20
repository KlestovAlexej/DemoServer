﻿using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using ShtrihM.DemoServer.Processing.Api.Common.Dtos.DemoObject;
using ShtrihM.DemoServer.Processing.Api.Common.Dtos.DemoObject.Update;
using ShtrihM.DemoServer.Processing.Model.Interfaces;
using ShtrihM.DemoServer.Processing.Tests.Model.Environment;
using ShtrihM.Wattle3.Testing;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShtrihM.DemoServer.Processing.Tests.Model;

public class TestsDemoObjectControllerService : BaseTestsDomainObjects
{
    private IDemoObjectControllerService m_demoObjectControllerService;
    private bool m_skipCountInternalException;

    [SetUp]
    public void SetUp()
    {
        m_skipCountInternalException = false;
        m_demoObjectControllerService = m_entryPoint.ServiceProvider.GetService<IDemoObjectControllerService>();
    }

    [TearDown]
    public void TearDown()
    {
        var snapShot = m_entryPoint.ExceptionPolicy.InfrastructureMonitor.GetSnapShot();

        if (false == m_skipCountInternalException)
        {
            Assert.AreEqual(0, snapShot.CountInternalException, GetDbLogs());
        }

        Assert.AreEqual(0, snapShot.CountMapperException, GetDbLogs());
        Assert.AreEqual(0, snapShot.CountUnexpectedException, GetDbLogs());
    }

    [Test]
    [Timeout(TestTimeout.Unit)]
    [Category(TestCategory.Unit)]
    public async Task Test_CreateAsync()
    {
        var info =
            await m_demoObjectControllerService.CreateAsync(
                new DemoObjectCreate
                {
                    Enabled = true,
                    Name = "Name",
                });
        Assert.True(info.Enabled);
        Assert.AreEqual("Name", info.Name);
    }

    [Test]
    [Timeout(TestTimeout.Unit)]
    [Category(TestCategory.Unit)]
    public async Task Test_ReadAsync()
    {
        var info =
            await m_demoObjectControllerService.CreateAsync(
                new DemoObjectCreate
                {
                    Enabled = true,
                    Name = "Name",
                });

        info = await m_demoObjectControllerService.ReadAsync(info.Id);
        Assert.True(info.Enabled);
        Assert.AreEqual("Name", info.Name);
    }

    [Test]
    [Timeout(TestTimeout.Unit)]
    [Category(TestCategory.Unit)]
    public async Task Test_DemoObjectUpdateAsync()
    {
        var info =
            await m_demoObjectControllerService.CreateAsync(
                new DemoObjectCreate
                {
                    Enabled = true,
                    Name = "Name",
                });

        info =
            await m_demoObjectControllerService.UpdateAsync(
                new DemoObjectUpdate
                {
                    Id = info.Id,
                    Fields =
                        new List<BaseDemoObjectUpdateFieldValue>
                        {
                            new DemoObjectUpdateFieldValueOfEnabled
                            {
                                Enabled = false,
                            },
                            new DemoObjectUpdateFieldValueOfName
                            {
                                Name = "Name2",
                            },
                        },
                });
        Assert.False(info.Enabled);
        Assert.AreEqual("Name2", info.Name);

        info = await m_demoObjectControllerService.ReadAsync(info.Id);
        Assert.False(info.Enabled);
        Assert.AreEqual("Name2", info.Name);
    }
}
