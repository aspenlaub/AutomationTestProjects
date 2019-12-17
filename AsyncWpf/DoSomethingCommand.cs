using System;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Vishizhukel.Interfaces.Application;

namespace Aspenlaub.Net.GitHub.CSharp.AsyncWpf {
    public class DoSomethingCommand : IApplicationCommand {
        public bool MakeLogEntries => false;
        public string Name => "DoSomething";
        public bool CanExecute() { return true; }

        public async Task Execute(IApplicationCommandExecutionContext context) {
            context.Report("Starting a task", false);

            // DoSomethingAsync(false) = WriteOutput WILL FAIL because it does not happen on the GUID thread
            var task = DoSomethingAsync(true, context);

            // task.Wait(); SYSTEM FREEZES!!
            context.Report("Awaiting the outcome", false);
            await task;
        }

        private async Task DoSomethingAsync(bool calledFromMainThread, IApplicationCommandExecutionContext context) {
            var val = 13;
            context.Report("Wait a second", false);
            await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(calledFromMainThread);
            val *= 2;
            context.Report("Wait another second", false);
            await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(calledFromMainThread);
            context.Report(val.ToString(), false);
        }
    }
}