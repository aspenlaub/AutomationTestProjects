using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Paleface.Components;
using Aspenlaub.Net.GitHub.CSharp.Paleface.Helpers;
using Aspenlaub.Net.GitHub.CSharp.Paleface.Interfaces;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MainWindowResources = Aspenlaub.Net.GitHub.CSharp.AsyncWpf.Properties.Resources;

namespace Aspenlaub.Net.GitHub.CSharp.AsyncWpf.Test {
    [TestClass]
    public class DoSomethingTest {
        protected static readonly string AsyncWpfExecutable = typeof(DoSomethingTest).Assembly.Location
            .Replace(@"\Test\", @"\")
            .Replace("Aspenlaub.Net.GitHub.CSharp.AsyncWpf.Test.dll", @"Aspenlaub.Net.GitHub.CSharp.AsyncWpf.exe");

        protected static readonly string AsyncWpfProcessName = "Aspenlaub.Net.GitHub.CSharp.AsyncWpf";

        protected static readonly string AsyncWpfWindowTitle = MainWindowResources.MainWindowTitle;

        protected readonly string Done = "Done";

        private readonly IContainer vContainer;

        public DoSomethingTest() {
            var builder = new ContainerBuilder().UsePaleface();
            vContainer = builder.Build();
        }

        [TestInitialize]
        public void Initialize() {
            TestProcessHelper.ShutDownRunningProcesses(AsyncWpfProcessName);
        }

        [TestCleanup]
        public void Cleanup() {
            TestProcessHelper.ShutDownRunningProcesses(AsyncWpfProcessName);
        }

        [TestMethod]
        public void SomethingIsNotDoneImmediately() {
            using var sut = vContainer.Resolve<IAppiumSession>();
            sut.Initialize(AsyncWpfExecutable, AsyncWpfWindowTitle, () => { }, 2);
            WhenDoSomethingButtonIsClicked(sut);
            ThenItIsNotDone(sut);
        }

        [TestMethod]
        public async Task SoonerOrLaterSomethingIsDone() {
            using var sut = vContainer.Resolve<IAppiumSession>();
            sut.Initialize(AsyncWpfExecutable, AsyncWpfWindowTitle, () => { }, 2);
            WhenDoSomethingButtonIsClicked(sut);
            var task = WhenWaitingForSomethingToBeDone(sut);
            var completed = await Task.WhenAny(task, Task.Delay(10000));
            Assert.AreEqual(task, completed);
        }

        [TestMethod]
        public async Task SomethingIsDoneWhenWaitingForAFewSeconds() {
            using var sut = vContainer.Resolve<IAppiumSession>();
            sut.Initialize(AsyncWpfExecutable, AsyncWpfWindowTitle, () => { }, 2);
            WhenDoSomethingButtonIsClicked(sut);
            await Task.Delay(2400);
            ThenItIsDone(sut);
        }

        protected void WhenDoSomethingButtonIsClicked(IAppiumSession sut) {
            var button = sut.FindButton("AsyncWpfDoSomethingButton");
            button.Click();
        }

        protected void ThenItIsNotDone(IAppiumSession sut) {
            var textBox = sut.FindTextBox("AsyncWpfOutputTextBox");
            Assert.IsFalse(textBox.Text.Contains(Done));
        }

        protected void ThenItIsDone(IAppiumSession sut) {
            var textBox = sut.FindTextBox("AsyncWpfOutputTextBox");
            Assert.IsTrue(textBox.Text.Contains(Done));
        }

        protected async Task WhenWaitingForSomethingToBeDone(IAppiumSession sut) {
            var textBox = sut.FindTextBox("AsyncWpfOutputTextBox");
            do {
                await Task.Delay(100);
            } while (!textBox.Text.Contains(Done));
        }
    }
}
