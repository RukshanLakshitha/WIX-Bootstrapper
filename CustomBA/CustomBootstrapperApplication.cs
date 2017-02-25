using CustomBA.Models;
using CustomBA.ViewModels;
using CustomBA.Views;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System.Windows.Threading;

namespace CustomBA
{
    public class CustomBootstrapperApplication : BootstrapperApplication
    {
        public static Dispatcher Dispatcher { get; set; }

        protected override void Run()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            var model = new BootstrapperApplicationModel(this);
            var viewModel = new InstallViewModel(model);
            var view = new InstallView(viewModel);
            model.SetWindowHandle(view);
            Engine.Detect();
            view.Show();
            Dispatcher.Run();
            Engine.Quit(model.FinalResult);
        }
    }
}
