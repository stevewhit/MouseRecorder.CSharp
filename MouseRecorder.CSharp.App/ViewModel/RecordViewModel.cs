﻿using System.Collections.ObjectModel;
using System.Linq;

namespace MouseRecorder.CSharp.App.ViewModel
{
    public class RecordViewModel : PageViewModelBase
    {
        private bool _isRecording;
        public bool IsRecording
        {
            get => _isRecording;
            set
            { 
                Set(ref _isRecording, value); 
                RaisePropertyChanged("CanSaveRecording"); 
            }
        }

        public bool CanSaveRecording
        {
            get => !_isRecording && (_actions?.Any() ?? false);
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
            set => _showRecordedActions = value;
        }

        public bool _showStartingPositionOverlay;
        public bool ShowStartingPositionOverlay 
        {
            get => _showStartingPositionOverlay;
            set => _showStartingPositionOverlay = value;// Set(ref _showStartingPositionOverlay, value);
        }

        public RecordViewModel()
        {
            _showRecordedActions = true;
            ResetActions();
        }

        public void ResetActions()
        {
            Actions = new ObservableCollection<string>();
            RaisePropertyChanged("CanSaveRecording");
        }
    }
}
