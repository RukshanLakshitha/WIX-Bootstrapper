using CustomBA.Models;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System;
using System.Windows.Input;

namespace CustomBA.ViewModels
{
    public class InstallViewModel : NotificationObject
    {
        public enum InstallState
        {
            Initializing,
            Present,
            NotPresent,
            Applying,
            Cancelled
        }

        private InstallState state;
        private string message;
        private BootstrapperApplicationModel model;
        private string username;

        public ICommand InstallCommand { get; private set; }
        public ICommand UninstallCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public string Message
        {
            get
            {
                return message;
            }
            set
            {
                if (message != value)
                {
                    message = value;
                    RaisePropertyChanged(() => Message);
                }
            }
        }

        public InstallState State
        {
            get
            {
                return state;
            }
            set
            {
                if (state != value)
                {
                    state = value;
                    Message = state.ToString();
                    RaisePropertyChanged(() => State);
                }
            }
        }

        public string Username
        {
            get
            {
                return username;
            }

            set
            {
                username = value;
                model.SetBurnVariable("Username", username);
            }
        }

        public InstallViewModel(BootstrapperApplicationModel model)
        {
            this.model = model;
            State = InstallState.Initializing;
            WireUpEventHandlers();
            InstallCommand = new DelegateCommand(() => this.model.PlanAction(LaunchAction.Install), () => State == InstallState.NotPresent);
            UninstallCommand = new DelegateCommand(() => this.model.PlanAction(LaunchAction.Uninstall), () => State == InstallState.Present);

            CancelCommand = new DelegateCommand(() =>
            {
                this.model.LogMessage("Cancelling...");
                if (State == InstallState.Applying)
                {
                    State = InstallState.Cancelled;
                }
                else
                {
                    CustomBootstrapperApplication.Dispatcher
                    .InvokeShutdown();
                }
            }, () => State != InstallState.Cancelled);
        }

        protected void DetectPackageComplete(object sender, DetectPackageCompleteEventArgs e)
        {
            if (e.PackageId.Equals("MyInstaller.msi", StringComparison.Ordinal))
            {
                State = e.State == PackageState.Present ? InstallState.Present : InstallState.NotPresent;
            }
        }

        protected void PlanComplete(object sender, PlanCompleteEventArgs e)
        {
            if (State == InstallState.Cancelled)
            {
                CustomBootstrapperApplication.Dispatcher.InvokeShutdown();
                return;
            }
            model.ApplyAction();
        }

        protected void ApplyBegin(object sender, ApplyBeginEventArgs e)
        {
            State = InstallState.Applying;
        }

        protected void ExecutePackageBegin(object sender, ExecutePackageBeginEventArgs e)
        {
            if (State == InstallState.Cancelled)
            {
                e.Result = Result.Cancel;
            }
        }

        protected void ExecutePackageComplete(object sender, ExecutePackageCompleteEventArgs e)
        {
            if (State == InstallState.Cancelled)
            {
                e.Result = Result.Cancel;
            }
        }

        protected void ApplyComplete(object sender, ApplyCompleteEventArgs e)
        {
            model.FinalResult = e.Status;
            CustomBootstrapperApplication.Dispatcher.InvokeShutdown();
        }

        private void Refresh()
        {
            CustomBootstrapperApplication.Dispatcher.Invoke(
            (Action)(() =>
            {
                ((DelegateCommand)InstallCommand).RaiseCanExecuteChanged();
                ((DelegateCommand)UninstallCommand).RaiseCanExecuteChanged();
                ((DelegateCommand)CancelCommand).RaiseCanExecuteChanged();
            }));
        }

        private void WireUpEventHandlers()
        {
            model.BootstrapperApplication.DetectPackageComplete += DetectPackageComplete;
            model.BootstrapperApplication.PlanComplete += PlanComplete;
            model.BootstrapperApplication.ApplyComplete += ApplyComplete;
            model.BootstrapperApplication.ApplyBegin += ApplyBegin;
            model.BootstrapperApplication.ExecutePackageBegin += ExecutePackageBegin;
            model.BootstrapperApplication.ExecutePackageComplete += ExecutePackageComplete;
        }
    }
}
