
using Gma.System.MouseKeyHook;
using MouseRecorder.CSharp.Business.Services;
using MouseRecorder.CSharp.DataModel.Actions;
using MouseRecorder.CSharp.DataModel.Configuration;
using System.Collections;
using System.Collections.ObjectModel;

namespace MouseRecorder.CSharp.App.ViewModel
{
    public class RecordViewModel : PageViewModelBase
    {
        private ObservableCollection<string> _actions;
        public ObservableCollection<string> Actions { get => _actions; set => Set(ref _actions, value); }

        public RecordViewModel()
        {
            Actions = new ObservableCollection<string>();
        }
    }
}
