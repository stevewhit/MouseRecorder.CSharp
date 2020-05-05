using Gma.System.MouseKeyHook;
using MouseRecorder.CSharp.App.ViewModel;
using MouseRecorder.CSharp.Business.Services;
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
        private const string PROMPT_SAVE_BEFORE_CONTINUE = @"Warning: Your current recording progress will be deleted. Would you like to save the recording before continuing?";
        private static readonly string SAVE_FILE_INITIAL_DIRECTORY = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static readonly Combination START_KEY_COMBINATION = Combination.TriggeredBy(Keys.A).With(Keys.S);
        private static readonly Combination STOP_KEY_COMBINATION = Combination.TriggeredBy(Keys.A).With(Keys.S);

        private readonly RecordViewModel _model;
        private readonly IGlobalRecordingService _service;

        public RecordView()
        {
            InitializeComponent();

            Dispatcher.ShutdownStarted += OnDispatcherShutDownStarted;

            // Set the datacontext
            _model = new RecordViewModel();
            DataContext = _model;

            // Subscribe to the recording service
            _service = new GlobalRecordingService();
            _service.RegisterCombinations(START_KEY_COMBINATION, STOP_KEY_COMBINATION);

            SubscribeToStartStopRecordingEvents();
        }

        #region Recorded Key Mouse Event Handlers

        /// <summary>
        /// Subscribes the start/stop recording event handlers to the recording service.
        /// </summary>
        private void SubscribeToStartStopRecordingEvents()
        {
            _service.AdditionalActionOnStartRecording = OnStartRecording;
            _service.AdditionalActionOnStopRecording = OnStopRecording;
        }

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
        }

        /// <summary>
        /// Unsubscribes all front-end key/mouse events from the recording service.
        /// </summary>
        private void UnsubscribeToKeyMouseEvents()
        {
            _service.AdditionalActionOnKeyDown = null;
            _service.AdditionalActionOnKeyUp = null;
            _service.AdditionalActionOnMouseMove = null;
            _service.AdditionalActionOnMouseDown = null;
            _service.AdditionalActionOnMouseUp = null;
            _service.AdditionalActionOnMouseWheel = null;
        }

        /// <summary>
        /// Event handler for when a key is pressed.
        /// </summary>
        private void OnKeyDown(KeyEventArgs e)
        {
            _model.Actions.Add($"{SystemTime.Now().Ticks} - Key Down: {e.KeyCode}");
            ListViewActions.ScrollIntoView(_model.Actions.LastOrDefault());
        }

        /// <summary>
        /// Event handler for when a key is released.
        /// </summary>
        private void OnKeyUp(KeyEventArgs e)
        {
            _model.Actions.Add($"{SystemTime.Now().Ticks} - Key Up: {e.KeyCode}");
            ListViewActions.ScrollIntoView(_model.Actions.LastOrDefault());
        }

        /// <summary>
        /// Event handler for when the mouse is moved
        /// </summary>
        private void OnMouseMove(MouseEventArgs e)
        {
            _model.Actions.Add($"{SystemTime.Now().Ticks} - Mouse Move: {e.Location}");
            ListViewActions.ScrollIntoView(_model.Actions.LastOrDefault());
        }

        /// <summary>
        /// Event handler for when a mouse button is pressed.
        /// </summary>
        private void OnMouseDown(MouseEventArgs e)
        {
            _model.Actions.Add($"{SystemTime.Now().Ticks} - Mouse Down: {e.Button}");
            ListViewActions.ScrollIntoView(_model.Actions.LastOrDefault());
        }

        /// <summary>
        /// Event handler for when a mouse button is released.
        /// </summary>
        private void OnMouseUp(MouseEventArgs e)
        {
            _model.Actions.Add($"{SystemTime.Now().Ticks} - Mouse Up: {e.Button}");
            ListViewActions.ScrollIntoView(_model.Actions.LastOrDefault());
        }

        /// <summary>
        /// Event handler for when the mouse wheel is scrolled.
        /// </summary>
        private void OnMouseWheel(MouseEventArgs e)
        {
            _model.Actions.Add($"{SystemTime.Now().Ticks} - Mouse Wheel: {e.Delta}");
            ListViewActions.ScrollIntoView(_model.Actions.LastOrDefault());
        }

        /// <summary>
        /// Event handler for when the recording is started.
        /// </summary>
        private void OnStartRecording()
        {
            if (!_model.IsRecording)
            {
                _model.Actions.Add($"{SystemTime.Now().Ticks} - Recording started..");
                ListViewActions.ScrollIntoView(_model.Actions.LastOrDefault());

                _model.IsRecording = true;
            }
        }

        /// <summary>
        /// Event handler for when the recording is stopped.
        /// </summary>
        private void OnStopRecording()
        {
            if (_model.IsRecording)
            {
                _model.Actions.Add($"{SystemTime.Now().Ticks} - Recording stopped..");
                ListViewActions.ScrollIntoView(_model.Actions[_model.Actions.Count - 1]);

                _model.IsRecording = false;
            }
        }

        #endregion
        #region Control Event Handlers

        private void BtnView_Clicked(object sender, RoutedEventArgs e)
        {
            /// Two options here:
            /// 
            /// 1. Force the user to save their current recording.
            /// 
            /// 2. Allow them to go back-and-forth between playback and recording
            ///     Pros: Gives user ability to add click-zones post-playback
            ///           Doesn't require user to save a temporary recording that they may/may not use
            ///     Cons: PITA

            _service.Unsubscribe();
        }

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            // Prompt user to see if they want to save the current recording.
            var selectedAnswer = PromptYesNoCancel.Prompt(PromptYesNoCancel.PromptType.YesNoCancel, PROMPT_SAVE_BEFORE_CONTINUE, "Save Recording");
            if (selectedAnswer == PromptYesNoCancel.DialogAnswer.Yes)
            {
                PromptUserWithSaveFileDialog();
            }
            else if (selectedAnswer == PromptYesNoCancel.DialogAnswer.No)
            {
                // Remove all recorded actions and click-zones.
                _service.ResetRecordedActions(true);
                _model.ResetActions();
            }
        }

        private void BtnRecord_Clicked(object sender, RoutedEventArgs e)
        {
            _service.StartRecording();
            OnStartRecording();
        }

        private void BtnStop_Clicked(object sender, RoutedEventArgs e)
        {
            _service.StopRecording();
            OnStopRecording();
        }

        private void BtnZone_Click(object sender, RoutedEventArgs e)
        {

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

        private void MenuFileSave_Click(object sender, RoutedEventArgs e)
        {
            PromptUserWithSaveFileDialog();
        }

        private void MenuFileExit_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.InvokeShutdown();
        }

        /// <summary>
        /// Event handler for when the main window is closing that prompts the user to save
        /// and pending recordings before the window closes.
        /// </summary>
        private void OnDispatcherShutDownStarted(object sender, EventArgs e)
        {
            if (_model.CanSaveRecording)
            {
                // Prompt user to see if they want to save the current recording.
                var selectedAnswer = PromptYesNoCancel.Prompt(PromptYesNoCancel.PromptType.YesNo, PROMPT_SAVE_BEFORE_CONTINUE, "Save Recording");
                if (selectedAnswer == PromptYesNoCancel.DialogAnswer.Yes)
                {
                    PromptUserWithSaveFileDialog();
                }
            }

            _service.Unsubscribe();
        }

        #endregion
        #region Additional Helper Methods

        /// <summary>
        /// Prompts the user with the save file dialog to save the recording.
        /// </summary>
        private void PromptUserWithSaveFileDialog()
        {
            // Unsubscribe from all input event handlers so the filename
            // doesn't trigger the recording to start/stop.
            _service.Unsubscribe();

            // Prompt user for file save location.
            var saveFileDialog = new SaveFileDialog
            {
                Title = "Save Recording..",
                Filter = "Mouse Recorder Recording (*.recording) | *.recording",
                InitialDirectory = SAVE_FILE_INITIAL_DIRECTORY
            };

            // If a path is chosen, save the file and reset the recorded actions.
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                _service.Save(saveFileDialog.FileName);
                _service.ResetRecordedActions();

                _model.ResetActions();
            }

            // Re-register the start/stop key combinations so they can be listened for.
            _service.RegisterCombinations(START_KEY_COMBINATION, STOP_KEY_COMBINATION);
        }

        #endregion
    }
}
