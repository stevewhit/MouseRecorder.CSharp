using Framework.Generic.WPF;
using System;
using System.Collections.Generic;

namespace MouseRecorder.CSharp.App.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private IPage content;
        public IPage Content { get => content; private set => Set(ref content, value); }

        public RelayCommand<int> NavigateCommand => new RelayCommand<int>(Navigate);

        private readonly Dictionary<int, Lazy<IPage>> pages
            = new Dictionary<int, Lazy<IPage>>
            {
                [1] = new Lazy<IPage>(() => new PlaybackViewModel()),
                [2] = new Lazy<IPage>(() => new RecordViewModel())
            };

        public MainWindowViewModel() => Navigate(1);

        private void Navigate(int value) => Content = pages[value].Value;
    }
}
