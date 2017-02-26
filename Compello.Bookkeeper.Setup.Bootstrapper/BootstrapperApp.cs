using Compello.Bookkeeper.Setup.Bootstrapper.Models;
using Compello.Bookkeeper.Setup.Bootstrapper.ViewModels;
using Compello.Bookkeeper.Setup.Bootstrapper.Views;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System.Windows.Threading;

namespace Compello.Bookkeeper.Setup.Bootstrapper
{
    public class BootstrapperApp : BootstrapperApplication
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
