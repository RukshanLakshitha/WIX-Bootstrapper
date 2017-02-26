using Compello.Bookkeeper.Setup.Bootstrapper.Models;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System;
using System.Windows.Input;

namespace Compello.Bookkeeper.Setup.Bootstrapper.ViewModels
{
    public class InstallViewModel : BindableBase
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
        private int progress;

        private int cacheProgress;
        private int executeProgress;

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
                    SetProperty(ref message, value);
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
                    Message = state.ToString();
                    SetProperty(ref state, value);
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

        public int Progress
        {
            get
            {
                return progress;
            }

            set
            {
                SetProperty(ref progress, value);
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
                    BootstrapperApp.Dispatcher.InvokeShutdown();
                }
            }, () => State != InstallState.Cancelled);

            this.model.BootstrapperApplication.CacheAcquireProgress += (sender, args) =>
            {
                cacheProgress = args.OverallPercentage;
                Progress = (cacheProgress + executeProgress) / 2;
            };

            this.model.BootstrapperApplication.ExecuteProgress += (sender, args) =>
            {
                executeProgress = args.OverallPercentage;
                Progress = (cacheProgress + executeProgress) / 2;
            };
        }

        protected void DetectPackageComplete(object sender, DetectPackageCompleteEventArgs e)
        {
            if (e.PackageId.Equals("Compello.BookKeeping.Setup_1.0.6247.27560.msi", StringComparison.Ordinal))
            {
                State = e.State == PackageState.Present ? InstallState.Present : InstallState.NotPresent;
            }
        }

        protected void PlanComplete(object sender, PlanCompleteEventArgs e)
        {
            if (State == InstallState.Cancelled)
            {
                BootstrapperApp.Dispatcher.InvokeShutdown();
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
            BootstrapperApp.Dispatcher.InvokeShutdown();
        }

        private void Refresh()
        {
            BootstrapperApp.Dispatcher.Invoke(
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
