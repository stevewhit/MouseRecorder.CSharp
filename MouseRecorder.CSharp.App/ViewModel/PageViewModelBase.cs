using Framework.Generic.WPF;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace MouseRecorder.CSharp.App.ViewModel
{
    public abstract class PageViewModelBase : ViewModelBase, IPage, INotifyPropertyChanged
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

        
    }
}
