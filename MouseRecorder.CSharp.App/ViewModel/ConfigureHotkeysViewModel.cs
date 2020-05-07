using Gma.System.MouseKeyHook;

namespace MouseRecorder.CSharp.App.ViewModel
{
    public class ConfigureHotkeysViewModel : PageViewModelBase
    {
        private string _title;
        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }

        private string _promptText;
        public string PromptText
        {
            get => _promptText;
            set => Set(ref _promptText, value);
        }

        private string _keyCombinationString;
        public string KeyCombinationString
        {
            get => _keyCombinationString;
            set
            {
                Set(ref _keyCombinationString, value);
                RaisePropertyChanged("CanSave");
            }
        }

        public bool CanSave => !string.IsNullOrEmpty(KeyCombinationString);
    }
}
