using Gma.System.MouseKeyHook;
using MouseRecorder.CSharp.App.ViewModel;
using MouseRecorder.CSharp.Business.Services;
using System.Windows;
using SWControls=System.Windows.Controls;
using System.Windows.Forms;
using System.Linq;
using System;
using Framework.Generic.Utility;
using System.Collections.Generic;

namespace MouseRecorder.CSharp.App.Views
{
    /// <summary>
    /// Interaction logic for RecordView.xaml
    /// </summary>
    public partial class RecordView : SWControls.UserControl
    {
        private const string PROMPT_SAVE_BEFORE_CONTINUE = "Warning: Your current recording progress will be deleted. Would you like to save the recording before continuing?";
        private const string PROMPT_CONFIGURE_START_RECORDING_HOTKEYS = "Press key combinations to configure the start recording hotkeys:";
        private const string PROMPT_CONFIGURE_STOP_RECORDING_HOTKEYS = "Press key combinations to configure the stop recording hotkeys:";
        private const string BTNRECORD_TOOLTIP_BASE = "Start recording";
        private const string BTNSTOP_TOOLTIP_BASE = "Stop Recording";
        private const string MENU_CONFIGURE_START_RECORDING_HOTKEY_BASE = "Configure Start Hotkeys";
        private const string MENU_CONFIGURE_STOP_RECORDING_HOTKEY_BASE = "Configure Stop Hotkeys";

        private static readonly string SAVE_FILE_INITIAL_DIRECTORY = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);
        private static Combination _startRecordingCombination = Combination.TriggeredBy(Keys.A).With(Keys.S);
        private static Combination _stopRecordingCombination = Combination.TriggeredBy(Keys.A).With(Keys.S);

        private readonly RecordViewModel _model;
        private readonly IGlobalRecordingService _service;

        private IList<ClickZoneView> _managedClickZoneViews;

        public RecordView()
        {
            InitializeComponent();

            Dispatcher.ShutdownStarted += OnDispatcherShutDownStarted;

            // Set the datacontext
            _model = new RecordViewModel();
            DataContext = _model;

            _managedClickZoneViews = new List<ClickZoneView>();

            // Subscribe to the recording service
            _service = new GlobalRecordingService();

            RegisterHotKeys();
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
            _model.Actions.Add($"{SystemTime.Now().Ticks} - Key Down: {e.KeyCode.Consolidate().GetKeyDescription()}");
            ListViewActions.ScrollIntoView(_model.Actions.LastOrDefault());
        }

        /// <summary>
        /// Event handler for when a key is released.
        /// </summary>
        private void OnKeyUp(KeyEventArgs e)
        {
            _model.Actions.Add($"{SystemTime.Now().Ticks} - Key Up: {e.KeyCode.Consolidate().GetKeyDescription()}");
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
            ///           MainWindow stores managed ClickZones and current recording/recording service..

            _service.Unsubscribe();

            // Temp -- change this.
            ResetRecording();
        }

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            // Prompt user to see if they want to save the current recording.
            var selectedAnswer = PromptYesNoCancelView.Prompt(PromptYesNoCancelView.PromptType.YesNoCancel, PROMPT_SAVE_BEFORE_CONTINUE, "Save Recording");
            if (selectedAnswer == PromptYesNoCancelView.DialogAnswer.Yes)
            {
                PromptAndSaveRecording();
            }
            else if (selectedAnswer == PromptYesNoCancelView.DialogAnswer.No)
            {
                // Remove all recorded actions and click-zones.
                ResetRecording();
            }
        }

        private void BtnRecord_Clicked(object sender, RoutedEventArgs e)
        {
            _service.StartRecording();
            OnStartRecording();
        }

        private void BtnRecordConfigure_Click(object sender, RoutedEventArgs e)
        {
            _service.Unsubscribe();

            // Prompt user to configure the hotkeys.
            _startRecordingCombination = ConfigureHotkeysView.Prompt("Configure Start Recording Hotkeys", PROMPT_CONFIGURE_START_RECORDING_HOTKEYS, _startRecordingCombination);

            // Re-register the start/stop key combinations so they can be listened for.
            RegisterHotKeys();
        }

        private void BtnStop_Clicked(object sender, RoutedEventArgs e)
        {
            _service.StopRecording();
            OnStopRecording();
        }

        private void BtnStopConfigure_Click(object sender, RoutedEventArgs e)
        {
            _service.Unsubscribe();

            // Prompt user to configure the hotkeys.
            _stopRecordingCombination = ConfigureHotkeysView.Prompt("Configure Stop Recording Hotkeys", PROMPT_CONFIGURE_STOP_RECORDING_HOTKEYS, _stopRecordingCombination);

            // Re-register the start/stop key combinations so they can be listened for.
            RegisterHotKeys();
        }

        private void BtnZone_Click(object sender, RoutedEventArgs e)
        {
            var startupLocation = Control.MousePosition;
            startupLocation.X += 50;

            // Create the view and add it to the list of managed views
            var view = ClickZoneView.Show(startupLocation, 100, 100, true);
            _managedClickZoneViews.Add(view);
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
            PromptAndSaveRecording();
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
                var selectedAnswer = PromptYesNoCancelView.Prompt(PromptYesNoCancelView.PromptType.YesNo, PROMPT_SAVE_BEFORE_CONTINUE, "Save Recording");
                if (selectedAnswer == PromptYesNoCancelView.DialogAnswer.Yes)
                {
                    PromptAndSaveRecording();
                }
            }

            _service.Unsubscribe();
        }

        #endregion
        #region Additional Helper Methods

        /// <summary>
        /// Re-registers the start/stop recording hotkeys with the service and updates all tooltips and menu items.
        /// </summary>
        private void RegisterHotKeys()
        {
            _service.RegisterCombinations(_startRecordingCombination, _stopRecordingCombination);

            _model.BtnRecordToolTip = $"{BTNRECORD_TOOLTIP_BASE} ({_startRecordingCombination})";
            _model.BtnStopToolTip = $"{BTNSTOP_TOOLTIP_BASE} ({_stopRecordingCombination})";

            _model.MenuConfigureStartRecording = $"{MENU_CONFIGURE_START_RECORDING_HOTKEY_BASE} ({_startRecordingCombination})";
            _model.MenuConfigureStopRecording = $"{MENU_CONFIGURE_STOP_RECORDING_HOTKEY_BASE} ({_stopRecordingCombination})";
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

                // Disable each of the click-zone windows.
                _managedClickZoneViews.ForEach(v => v.DisableAndMakeTransparent());
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

                // Enable each of the click-zone windows.
                _managedClickZoneViews.ForEach(v => v.EnableAndRemoveTransparency());
                _model.IsRecording = false;
            }
        }

        /// <summary>
        /// Prompts the user with the save file dialog to save the recording.
        /// </summary>
        private void PromptAndSaveRecording()
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
                _service.AddClickZones(_managedClickZoneViews.Select(v => v.GetWindowRectangle()));
                _service.Save(saveFileDialog.FileName);

                ResetRecording();
            }

            // Re-register the start/stop key combinations so they can be listened for.
            RegisterHotKeys();
        }

        /// <summary>
        /// Resets the service and view model, and closes and removes and click-zone windows.
        /// </summary>
        private void ResetRecording()
        {
            _service.ResetRecordedActions(true);
            _model.ResetActions();

            _managedClickZoneViews?.ForEach(v => v.Close());
            _managedClickZoneViews = new List<ClickZoneView>();
        }

        #endregion
    }
}
