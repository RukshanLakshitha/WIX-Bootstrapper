using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System;
using System.Windows;
using System.Windows.Interop;

namespace Compello.Bookkeeper.Setup.Bootstrapper.Models
{
    public class BootstrapperApplicationModel
    {
        private IntPtr hwnd;

        public BootstrapperApplication BootstrapperApplication
        {
            get;
            private set;
        }

        public int FinalResult { get; set; }

        public BootstrapperApplicationModel(BootstrapperApplication bootstrapperApplication)
        {
            BootstrapperApplication = bootstrapperApplication;
            hwnd = IntPtr.Zero;
        }

        public void SetBurnVariable(string variableName, string value)
        {
            BootstrapperApplication.Engine.StringVariables[variableName] = value;
        }

        public void SetWindowHandle(Window view)
        {
            hwnd = new WindowInteropHelper(view).Handle;
        }

        public void PlanAction(LaunchAction action)
        {
            BootstrapperApplication.Engine.Plan(action);
        }

        public void ApplyAction()
        {
            BootstrapperApplication.Engine.Apply(hwnd);
        }

        public void LogMessage(string message)
        {
            BootstrapperApplication.Engine.Log(LogLevel.Standard, message);
        }
    }
}
