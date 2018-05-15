using System;
using System.Threading;
using System.Windows;
using Aspenlaub.Net.GitHub.CSharp.Vishizhukel.Application;
using Aspenlaub.Net.GitHub.CSharp.Vishizhukel.Interfaces.Application;

namespace Aspenlaub.Net.GitHub.CSharp.AsyncWpf {
    public partial class MainWindow {
        protected readonly ApplicationCommandController CommandController;

        public MainWindow() {
            InitializeComponent();
            Title = Properties.Resources.MainWindowTitle;
            CommandController = new ApplicationCommandController(ApplicationFeedbackHandler);
            CommandController.AddCommand(new DoSomethingCommand(), true);
            CommandController.AddCommand(new PrimeNumbersCommand(), true);
            DoSomething.Click += ExecuteRoutedCommand<DoSomethingCommand>;
            PrimeNumbers.Click += ExecuteRoutedCommand<PrimeNumbersCommand>;
            ApplicationFeedbackHandler(new FeedbackToApplication { Type = FeedbackType.CommandsEnabledOrDisabled });
        }

        private async void ExecuteRoutedCommand<TCommandType>(object sender, RoutedEventArgs e) where TCommandType : IApplicationCommand {
            await CommandController.Execute(typeof(TCommandType));
        }

        public void ApplicationFeedbackHandler(IFeedbackToApplication feedBack) {
            if (!CommandController.IsMainThread()) {
                throw new ThreadStateException("Feedback handler must only be called on the UI thread");
            }

            string threadInfo;
            switch (feedBack.Type) {
                case FeedbackType.ImportantMessage :
                case FeedbackType.MessageOfNoImportance :
                case FeedbackType.CommandExecutionCompleted : {
                    threadInfo = CommandController.IsMainThread() ? "MAIN" : Thread.CurrentThread.ManagedThreadId.ToString();
                    Output.Text += "\r\n" + threadInfo + ": " + (feedBack.Type == FeedbackType.CommandExecutionCompleted ? "Done" : feedBack.Message);
                } break;
                case FeedbackType.EnableCommand:
                case FeedbackType.DisableCommand: {
                    throw new ArgumentException("Enable/disable command should be handled by the application command controller");
                }
                case FeedbackType.MessagesOfNoImportanceWereIgnored : {
                    threadInfo = CommandController.IsMainThread() ? "MAIN" : Thread.CurrentThread.ManagedThreadId.ToString();
                    Output.Text += "\r\n" + threadInfo + ": ...";
                } break;
                case FeedbackType.CommandIsDisabled : {
                    threadInfo = CommandController.IsMainThread() ? "MAIN" : Thread.CurrentThread.ManagedThreadId.ToString();
                    Output.Text += "\r\n" + threadInfo + ": COMMAND IS DISABLED";
                }
                break;
                case FeedbackType.CommandsEnabledOrDisabled: {
                    DoSomething.IsEnabled = CommandController.Enabled(typeof(DoSomethingCommand));
                    PrimeNumbers.IsEnabled = CommandController.Enabled(typeof(PrimeNumbersCommand));
                }
                break;
                default: {
                    throw new NotImplementedException();
                }
            }
        }
    }
}
