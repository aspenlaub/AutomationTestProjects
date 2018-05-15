using System;
using System.IO;
using System.Threading.Tasks;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.AsyncWpf.Test {
    public class ApplicationAndWindow : IDisposable {
        public UIA3Automation Uia3Automation { get; }
        public Application Application { get; }
        public Window Window { get; }

        public ApplicationAndWindow() {
            var executableFile = typeof(MainWindow).Assembly.Location;
            Assert.IsTrue(File.Exists(executableFile));
            Application = Application.Launch(executableFile);
            Application.WaitWhileBusy();
            Uia3Automation = new UIA3Automation();
            Window = Application.GetMainWindow(Uia3Automation);
        }

        public void Dispose() {
            Window.Close();
            Application.Dispose();
        }
    }

    [TestClass]
    public class DoSomethingTest {
        protected readonly string Done = "Done";

        [TestMethod]
        public void SomethingIsNotDoneImmediately() {
            using (var applicationAndWindow = new ApplicationAndWindow()) {
                WhenDoSomethingButtonIsClicked(applicationAndWindow);
                ThenItIsNotDone(applicationAndWindow);
            }
        }

        [TestMethod]
        public async Task SoonerOrLaterSomethingIsDone() {
            using (var applicationAndWindow = new ApplicationAndWindow()) {
                WhenDoSomethingButtonIsClicked(applicationAndWindow);
                var task = WhenWaitingForSomethingToBeDone(applicationAndWindow);
                var completed = await Task.WhenAny(task, Task.Delay(10000));
                Assert.AreEqual(task, completed);
            }
        }

        [TestMethod]
        public async Task SomethingIsDoneWhenWaitingForAFewSeconds() {
            using (var applicationAndWindow = new ApplicationAndWindow()) {
                WhenDoSomethingButtonIsClicked(applicationAndWindow);
                await Task.Delay(2400);
                ThenItIsDone(applicationAndWindow);
            }
        }

        protected void WhenDoSomethingButtonIsClicked(ApplicationAndWindow applicationAndWindow) {
            var button = applicationAndWindow.Window.FindFirstDescendant(nameof(MainWindow.DoSomething)).AsButton();
            button.Click();
        }

        protected void ThenItIsNotDone(ApplicationAndWindow applicationAndWindow) {
            var textBox = applicationAndWindow.Window.FindFirstDescendant(nameof(MainWindow.Output)).AsTextBox();
            Assert.IsFalse(textBox.Text.Contains(Done));
        }

        protected void ThenItIsDone(ApplicationAndWindow applicationAndWindow) {
            var textBox = applicationAndWindow.Window.FindFirstDescendant(nameof(MainWindow.Output)).AsTextBox();
            Assert.IsTrue(textBox.Text.Contains(Done));
        }

        protected async Task WhenWaitingForSomethingToBeDone(ApplicationAndWindow applicationAndWindow) {
            var textBox = applicationAndWindow.Window.FindFirstDescendant(nameof(MainWindow.Output)).AsTextBox();
            do {
                await Task.Delay(100);
            } while (!textBox.Text.Contains(Done));
        }
    }
}
