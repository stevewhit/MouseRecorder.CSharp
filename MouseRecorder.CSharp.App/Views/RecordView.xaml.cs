using Gma.System.MouseKeyHook;
using MouseRecorder.CSharp.App.ViewModel;
using MouseRecorder.CSharp.Business.Services;
using MouseRecorder.CSharp.DataModel.Configuration;
using System.Windows;
using SWControls=System.Windows.Controls;
using System.Windows.Forms;
using System.Linq;
using System;
using Framework.Generic.Utility;

namespace MouseRecorder.CSharp.App.Views
{
    /// <summary>
    /// Interaction logic for RecordView.xaml
    /// </summary>
    public partial class RecordView : SWControls.UserControl
    {
        private readonly RecordViewModel _model;
        private readonly IGlobalRecordingService _service;

        public RecordView()
        {
            InitializeComponent();

            var config = new RecordingConfiguration
            {
                RecordKeyboardInputs = true,
                RecordMouseInputs = true,
                StartRecordingCombination = Combination.TriggeredBy(Keys.A).With(Keys.S),
                StopRecordingCombination = Combination.TriggeredBy(Keys.A).With(Keys.S)
            };

            // Set the datacontext
            _model = new RecordViewModel();
            DataContext = _model;

            // Subscribe to the recording service
            _service = new GlobalRecordingService();
            _service.Subscribe(config); 
        }

        #region Recorded Key Mouse Event Handlers

        /// <summary>
        /// Subscribes additional actions for the recorded key mouse events.
        /// </summary>
        private void SubscribeToKeyMouseEvents()
        {
            _service.AdditionalActionOnKeyDown = OnKeyDown;
            _service.AdditionalActionOnKeyUp = OnKeyUp;
            _service.AdditionalActionOnMouseMove = OnMouseMove;
            _service.AdditionalActionOnMouseDown = OnMouseDown;
            _service.AdditionalActionOnMouseUp = OnMouseUp;
            _service.AdditionalActionOnMouseWheel = OnMouseWheel;
            _service.AdditionalActionOnStartRecording = OnStartRecording;
            _service.AdditionalActionOnStopRecording = OnStopRecording;
        }

        private void UnsubscribeToKeyMouseEvents()
        {
            _service.AdditionalActionOnKeyDown = null;
            _service.AdditionalActionOnKeyUp = null;
            _service.AdditionalActionOnMouseMove = null;
            _service.AdditionalActionOnMouseDown = null;
            _service.AdditionalActionOnMouseUp = null;
            _service.AdditionalActionOnMouseWheel = null;
            _service.AdditionalActionOnStartRecording = null;
            _service.AdditionalActionOnStopRecording = null;
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnKeyDown(KeyEventArgs e)
        {
            _model.Actions.Add($"{SystemTime.Now().Ticks} - Key Down: {e.KeyCode}");
            ListViewActions.ScrollIntoView(_model.Actions.LastOrDefault());
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnKeyUp(KeyEventArgs e)
        {
            _model.Actions.Add($"{SystemTime.Now().Ticks} - Key Up: {e.KeyCode}");
            ListViewActions.ScrollIntoView(_model.Actions.LastOrDefault());
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnMouseMove(MouseEventArgs e)
        {
            _model.Actions.Add($"{SystemTime.Now().Ticks} - Mouse Move: {e.Location}");
            ListViewActions.ScrollIntoView(_model.Actions.LastOrDefault());
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnMouseDown(MouseEventArgs e)
        {
            _model.Actions.Add($"{SystemTime.Now().Ticks} - Mouse Down: {e.Button}");
            ListViewActions.ScrollIntoView(_model.Actions.LastOrDefault());
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnMouseUp(MouseEventArgs e)
        {
            _model.Actions.Add($"{SystemTime.Now().Ticks} - Mouse Up: {e.Button}");
            ListViewActions.ScrollIntoView(_model.Actions.LastOrDefault());
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnMouseWheel(MouseEventArgs e)
        {
            _model.Actions.Add($"{SystemTime.Now().Ticks} - Mouse Wheel: {e.Delta}");
            ListViewActions.ScrollIntoView(_model.Actions.LastOrDefault());
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnStartRecording()
        {
            if (!_model.IsRecording)
            {
                _model.Actions.Add($"{SystemTime.Now().Ticks} - Recording started..");
                ListViewActions.ScrollIntoView(_model.Actions.LastOrDefault());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnStopRecording()
        {
            if (_model.IsRecording)
            {
                _model.Actions.Add($"{SystemTime.Now().Ticks} - Recording stopped..");
                ListViewActions.ScrollIntoView(_model.Actions[_model.Actions.Count - 1]);
            }
        }

        #endregion
        #region Control Event Handlers

        private void BtnView_Clicked(object sender, RoutedEventArgs e)
        {
            // TODO: prompt to save..?
            _service.Unsubscribe();
        }

        private void BtnRecord_Clicked(object sender, RoutedEventArgs e)
        {
            StartRecording();
        }

        private void BtnStop_Clicked(object sender, RoutedEventArgs e)
        {
            StopRecording();
        }

        private void CheckBoxShowRecordedActions_Changed(object sender, RoutedEventArgs e)
        {
            if (_model.ShowRecordedActions)
                SubscribeToKeyMouseEvents();
            else
                UnsubscribeToKeyMouseEvents();
        }

        private void CheckBoxShowStartingPositionOverlay_Changed(object sender, RoutedEventArgs e)
        {
            if (_model.ShowStartingPositionOverlay)
                throw new NotImplementedException();
            else
                throw new NotImplementedException();
        }

        #endregion

        private void StartRecording()
        {
            _model.IsRecording = true;
            _service.StartRecording();
        }

        private void StopRecording()
        {
            _model.IsRecording = false;
            _service.StopRecording();
        }
    }
}
