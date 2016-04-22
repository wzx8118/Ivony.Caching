﻿using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Ivony.Caching.Test
{
  /// <summary>
  /// MonitorTesting 的摘要说明
  /// </summary>
  [TestClass]
  public class MonitorTesting
  {

    /// <summary>
    ///获取或设置测试上下文，该上下文提供
    ///有关当前测试运行及其功能的信息。
    ///</summary>
    public TestContext TestContext { get; set; }

    #region 附加测试特性
    //
    // 编写测试时，可以使用以下附加特性: 
    //
    // 在运行类中的第一个测试之前使用 ClassInitialize 运行代码
    // [ClassInitialize()]
    // public static void MyClassInitialize(TestContext testContext) { }
    //
    // 在类中的所有测试都已运行之后使用 ClassCleanup 运行代码
    // [ClassCleanup()]
    // public static void MyClassCleanup() { }
    //
    // 在运行每个测试之前，使用 TestInitialize 来运行代码
    // [TestInitialize()]
    // public void MyTestInitialize() { }
    //
    // 在每个测试运行完之后，使用 TestCleanup 来运行代码
    // [TestCleanup()]
    // public void MyTestCleanup() { }
    //
    #endregion




    [TestMethod]
    public void MonitorExceptionTest()
    {

      using ( var provider = new MemoryCacheProvider( "Test" ).AsAsyncProvider() )
      {
        var cacheService = new CacheService( provider );
        cacheService.RegisterMonitor( new CacheServiceMonitorWithException() );


        Task.Run( async () =>
        {

          await cacheService.FetchOrAdd( "Test", async () => { return "Test"; }, CachePolicy.Expired( TimeSpan.FromHours( 1 ) ) );
          await cacheService.FetchOrAdd( "Test", async () => { return "Test"; }, CachePolicy.Expired( TimeSpan.FromHours( 1 ) ) );

          await Task.Delay( 100 );

          await cacheService.FetchOrAdd( "Test", async () => { return "Test"; }, CachePolicy.Expired( TimeSpan.FromHours( 1 ) ) );
          await cacheService.FetchOrAdd( "Test", async () => { return "Test"; }, CachePolicy.Expired( TimeSpan.FromHours( 1 ) ) );

        }
        ).Wait();

      }

    }


    public class CacheServiceMonitorWithException : ICacheServiceMonitor
    {
      public void OnCacheHitted( string cacheKey )
      {
        throw new Exception();
      }

      public void OnCacheMissed( string cacheKey )
      {
        throw new Exception();
      }
    }
  }
}
