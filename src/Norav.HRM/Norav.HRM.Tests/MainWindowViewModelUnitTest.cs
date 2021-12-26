using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Reactive.Testing;
using Norav.HRM.Client.WPF.Interfaces;
using Norav.HRM.Client.WPF.ViewModels;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Reactive.Concurrency;
using Norav.HRM.Client.WPF.Modules.HeartbeatSimulation;

namespace Norav.HRM.Tests.Windows
{
    public class Tests
    {
        private IServiceProvider serviceProvider;

        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();

            var ecgProvider = A.Fake<IHeartbeatProvider>();
            var plotPresenter = A.Fake<IPlotPresenter>();
            
            services.AddTransient(_ => ecgProvider);
            services.AddTransient(_ => plotPresenter);

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
            /*
                { @"c:\myfile.txt", new MockFileData("Testing is meh.") },
                { @"c:\demo\jQuery.js", new MockFileData("some js") },
                { @"c:\demo\image.gif", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            */
            });
            services.AddTransient<IFileSystem>(provider => fileSystem);
            services.AddTransient<IScheduler>(provider => new TestScheduler());

            services.AddTransient<MainWindowViewModel>();

            serviceProvider = services.BuildServiceProvider();
        }

        [Test]
        public void ctor_called_ExpectWorkingSetup()
        {
            serviceProvider.GetService<MainWindowViewModel>();
        }

        [Test]
        public void Start_Executed_ExpectIsExecutingShouldBeTrue()
        {
            var uut = serviceProvider.GetService<MainWindowViewModel>();

            uut.IsExecuting.Should().BeFalse();

            //act
            uut.Start.Execute();


            uut.IsExecuting.Should().BeTrue();
        }

        /* TODO: Shlomi, more test coverage... this is just to show ability.. */
    }
}