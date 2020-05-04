using MouseRecorder.CSharp.DataModel.Configuration;
using System;
using System.Collections.ObjectModel;

namespace MouseRecorder.CSharp.App.ViewModel
{
    public class RecordViewModel : PageViewModelBase
    {
        private bool _isRecording;
        public bool IsRecording
        {
            get => _isRecording;
            set => Set(ref _isRecording, value);
        }

        private ObservableCollection<string> _actions;
        public ObservableCollection<string> Actions 
        { 
            get => _actions; 
            set => Set(ref _actions, value); 
        }

        public bool _showRecordedActions;
        public bool ShowRecordedActions 
        {
            get => _showRecordedActions;
            set => _showRecordedActions = value;// Set(ref _showRecordedActions, value);
        }

        public bool _showStartingPositionOverlay;
        public bool ShowStartingPositionOverlay 
        {
            get => _showStartingPositionOverlay;
            set => _showStartingPositionOverlay = value;// Set(ref _showStartingPositionOverlay, value);
        }

        public RecordViewModel()
        {
            Actions = new ObservableCollection<string>();
        }
    }
}
