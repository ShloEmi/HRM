using Prism.Mvvm;

namespace Norav.HRM.Client.WPF.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string title;
        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        public MainWindowViewModel()
        {
            Title = "Heartbeat Test";
        }
    }
}