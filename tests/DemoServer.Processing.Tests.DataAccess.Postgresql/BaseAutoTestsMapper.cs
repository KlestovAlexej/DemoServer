﻿using NUnit.Framework;
using ShtrihM.DemoServer.Common;
using ShtrihM.DemoServer.Processing.DataAccess.PostgreSql;
using ShtrihM.Wattle3.Testing.Databases.PostgreSql;
using System;
using System.Diagnostics.CodeAnalysis;
using ShtrihM.DemoServer.Testing;

// ReSharper disable once CheckNamespace
namespace ShtrihM.DemoServer.Processing.Generated.Tests;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public abstract partial class BaseAutoTestsMapper
{
    protected string m_serverConnectionString;
    protected string m_dbName;
    protected bool m_dropDb;
    protected bool m_addTags;

    partial void DoBase_BeginSetUp()
    {
        m_dropDb = true;
        m_dbName = $"test_{Constants.ProductTag.ToLower()}_" + Guid.NewGuid().ToString("N") + "_" + DateTime.Now.ToString("yyyMMddhhmmss");
        var sqlScript = Deploy.GetSqlScript();

        /*
        Если адрес, логин и пароль БД PostgreSQL не указаны явно то они берутся из реестра Windows.
        Файл с настройками реестра.
        src\DemoServer.Testing\WindowsRegisterTestingEnvirioment.reg
        */
        m_serverConnectionString = PostgreSqlDbHelper.GetServerConnectionString(userCredentials: BaseDbTests.PostgreSqlUserCredentials, serverAdress: BaseDbTests.PostgreSqlServerAdress);

        /*
        Если адрес, логин и пароль БД PostgreSQL не указаны явно то они берутся из реестра Windows.
        Файл с настройками реестра.
        src\DemoServer.Testing\WindowsRegisterTestingEnvirioment.reg
        */
        m_dbConnectionString = PostgreSqlDbHelper.GetDatabaseConnectionString(m_dbName, userCredentials: BaseDbTests.PostgreSqlUserCredentials, serverAdress: BaseDbTests.PostgreSqlServerAdress);

        DoPreCreateDb();

        PostgreSqlDbHelper.CreateDb(
            m_dbName,
            tag: m_addTags ? TestContext.CurrentContext.Test.FullName : null,
            sqlScript: sqlScript,
            serverConnectionString: m_serverConnectionString,
            databaseConnectionString: m_dbConnectionString);
    }

    partial void DoBase_TearDown()
    {
        if (m_dropDb)
        {
            PostgreSqlDbHelper.DropDb(
                m_dbName,
                serverConnectionString: m_serverConnectionString);
        }
    }

    protected virtual void DoPreCreateDb()
    {
        /* NONE */
    }
}