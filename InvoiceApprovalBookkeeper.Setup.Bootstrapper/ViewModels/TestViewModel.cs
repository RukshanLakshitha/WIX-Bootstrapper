using InvoiceApprovalBookkeeper.Setup.Bootstrapper.Models;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using System;

namespace InvoiceApprovalBookkeeper.Setup.Bootstrapper.ViewModels
{
    class TestViewModel : BindableBase
    {
        private TestModel testModel;
        private string textVal;

        public DelegateCommand PressCommand { get; set; }

        public TestModel TestModel
        {
            get
            {
                return testModel;
            }
            set
            {
                SetProperty(ref testModel, value);
            }
        }

        public string TextVal
        {
            get
            {
                return textVal;
            }

            set
            {
                SetProperty(ref textVal, value);
            }
        }

        public TestViewModel()
        {
            testModel = new TestModel();
            testModel.Message = "This Is Prism Example";

            PressCommand = new DelegateCommand(HandlePressCommand);
        }

        private void HandlePressCommand()
        {
            Console.Write("Name: " + TextVal);
        }
    }
}
