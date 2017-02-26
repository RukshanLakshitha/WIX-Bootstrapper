using Microsoft.Practices.Prism.Mvvm;

namespace InvoiceApprovalBookkeeper.Setup.Bootstrapper.Models
{
    class TestModel : BindableBase
    {
        private string _Message;

        public string Message
        {
            get
            {
                return _Message;
            }
            set
            {
                SetProperty(ref _Message, value);
            }
        }
    }
}
