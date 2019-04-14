﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using PEBakery.Core.ViewModels;
using System;
using System.IO;
using PEBakery.Helper;

namespace PEBakery.Core.Tests
{
    [TestClass]
    public class TestSetup
    {
        #region AssemblyInitalize, AssemblyCleanup
        [AssemblyInitialize]
        public static void PrepareTests(TestContext ctx)
        {
            // Set MainViewModel
            Global.MainViewModel = new MainViewModel();

            // Instance of Setting
            string emptyTempFile = Path.GetTempFileName();
            if (File.Exists(emptyTempFile))
                File.Delete(emptyTempFile);
            Global.Setting = new Setting(emptyTempFile); // Set to default

            // Load Project "TestSuite" (ScriptCache disabled)
            EngineTests.BaseDir = Path.GetFullPath(Path.Combine("..", "..", "Samples"));
            ProjectCollection projects = new ProjectCollection(EngineTests.BaseDir);
            projects.PrepareLoad();
            projects.Load(null, null);

            // Should be only one project named TestSuite
            EngineTests.Project = projects[0];
            Assert.IsTrue(projects.Count == 1);

            // Init NativeAssembly
            Global.NativeGlobalInit(AppDomain.CurrentDomain.BaseDirectory);
            EngineTests.MagicFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "magic.mgc");

            // Use InMemory Database for Tests
            Logger.DebugLevel = LogDebugLevel.PrintExceptionStackTrace;
            EngineTests.Logger = new Logger(":memory:");
            EngineTests.Logger.SystemWrite(new LogInfo(LogState.Info, "PEBakery.Tests launched"));

            // Set Global 
            Global.Logger = EngineTests.Logger;
            Global.BaseDir = EngineTests.BaseDir;
            Global.MagicFile = EngineTests.MagicFile;
            Global.BuildDate = BuildTimestamp.ReadDateTime();

            // IsOnline?
            EngineTests.IsOnline = NetworkHelper.IsOnline();
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            Global.Cleanup();
            EngineTests.Logger = null;
        }
        #endregion
    }
}
