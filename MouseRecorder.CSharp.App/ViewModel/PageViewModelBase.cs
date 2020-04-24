using Framework.Generic.WPF;
using System.Windows;

namespace MouseRecorder.CSharp.App.ViewModel
{
    public abstract class PageViewModelBase : ViewModelBase, IPage
    {
        public RelayCommand<int> NavigateCommand
        {
            get
            {
                var window = Application.Current.MainWindow as MainWindow;
                var windowViewModel = window?.DataContext as MainWindowViewModel;

                return windowViewModel?.NavigateCommand;
            }
        }

        public PageViewModelBase()
        {

        }
    }
}
