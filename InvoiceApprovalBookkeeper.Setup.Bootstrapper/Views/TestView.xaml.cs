using InvoiceApprovalBookkeeper.Setup.Bootstrapper.ViewModels;
using System.Windows.Controls;

namespace InvoiceApprovalBookkeeper.Setup.Bootstrapper.Views
{
    /// <summary>
    /// Interaction logic for TestView2.xaml
    /// </summary>
    public partial class TestView : UserControl
    {
        public TestView()
        {
            InitializeComponent();
            DataContext = new TestViewModel();
        }
    }
}
