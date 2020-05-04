using Framework.Generic.Utility;
using Gma.System.MouseKeyHook;
using MouseRecorder.CSharp.DataModel.Actions;
using MouseRecorder.CSharp.DataModel.Configuration;
using MouseRecorder.CSharp.DataModel.Zone;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MouseRecorder.CSharp.Business.Services
{
    public interface IGlobalRecordingService
    {
        /// <summary>
        /// Additional action that is performed when a key is pressed. Serves as a
        /// tie-in for the front-end of an application.
        /// </summary>
        Action<KeyEventArgs> AdditionalActionOnKeyDown { get; set; }

        /// <summary>
        /// Additional action that is performed when a key is released. Serves as a
        /// tie-in for the front-end of an application.
        /// </summary>
        Action<KeyEventArgs> AdditionalActionOnKeyUp { get; set; }

        /// <summary>
        /// Additional action that is performed when a mouse button is pressed. Serves as a
        /// tie-in for the front-end of an application.
        /// </summary>
        Action<MouseEventArgs> AdditionalActionOnMouseDown { get; set; }

        /// <summary>
        /// Additional action that is performed when a mouse button is released. Serves as a
        /// tie-in for the front-end of an application.
        /// </summary>
        Action<MouseEventArgs> AdditionalActionOnMouseUp { get; set; }

        /// <summary>
        /// Additional action that is performed when the mouse wheel is scrolled. Serves as a
        /// tie-in for the front-end of an application.
        /// </summary>
        Action<MouseEventArgs> AdditionalActionOnMouseWheel { get; set; }

        /// <summary>
        /// Additional action that is performed when the mouse is moved. Serves as a
        /// tie-in for the front-end of an application.
        /// </summary>
        Action<MouseEventArgs> AdditionalActionOnMouseMove { get; set; }

        /// <summary>
        /// Additional action that is performed when the recording is started. Serves as
        /// a tie-in for the front-end of an application
        /// </summary>
        Action AdditionalActionOnStartRecording { get; set; }

        /// <summary>
        /// Additional action that is performed when the recording is stopped. Serves as
        /// a tie-in for the front-end of an application
        /// </summary>
        Action AdditionalActionOnStopRecording { get; set; }

        /// <summary>
        /// Registers the <paramref name="startRecordingCombination"/> and <paramref name="stopRecordingCombination"/> combinations
        /// for starting and stopping the recording.
        /// </summary>
        /// <param name="startRecordingCombination">The key combination that starts the recording.</param>
        /// <param name="stopRecordingCombination">The key combination that stops the recording.</param>
        void RegisterCombinations(Combination startRecordingCombination, Combination stopRecordingCombination);

        /// <summary>
        /// Unsubscribes the recording from the service and stops all global listeners if any are running.
        /// </summary>
        void Unsubscribe();

        /// <summary>
        /// Subscribes the recording to global keyboard and mouse events, and listens for the stop recording combination.
        /// </summary>
        /// <param name="appendToExisting">Indicates whether the recording should append any new actions to the current recording.</param>
        void StartRecording(bool appendToExisting = true);

        /// <summary>
        /// Unsubscribes the recording to global keyboard and mouse events, and listens for the start recording combination.
        /// </summary>
        void StopRecording();

        /// <summary>
        /// Removes any existing recorded actions and any click-zones if the <paramref name="removeClickZones"/> flag is set.
        /// </summary>
        void ResetRecordedActions(bool removeClickZones = true);

        /// <summary>
        /// Saves the recording to the desired <paramref name="filePath"/>.
        /// </summary>
        void Save(string filePath);
    }

    public class GlobalRecordingService : IGlobalRecordingService
    {
        private readonly IFileService _fileService;
        private IRecording _currentRecording;
        private Combination _startRecordingCombination;
        private Combination _stopRecordingCombination;
        private ISet<Keys> _pressedKeys;
        private bool _isRecording;

        private static IKeyboardMouseEvents _globalKeyMouseEvents;
        private static IKeyboardMouseEvents GlobalEventsHook
        {
            get 
            {
                if (_globalKeyMouseEvents == null)
                    _globalKeyMouseEvents = Hook.GlobalEvents();

                return _globalKeyMouseEvents;
            }
            set => _globalKeyMouseEvents = value;
        }
        
        #region Additional Recording Event Action Properties

        /// <summary>
        /// Additional action that is performed when a key is pressed. Serves as a
        /// tie-in for the front-end of an application.
        /// </summary>
        public Action<KeyEventArgs> AdditionalActionOnKeyDown { get; set; }

        /// <summary>
        /// Additional action that is performed when a key is released. Serves as a
        /// tie-in for the front-end of an application.
        /// </summary>
        public Action<KeyEventArgs> AdditionalActionOnKeyUp { get; set; }

        /// <summary>
        /// Additional action that is performed when a mouse button is pressed. Serves as a
        /// tie-in for the front-end of an application.
        /// </summary>
        public Action<MouseEventArgs> AdditionalActionOnMouseDown { get; set; }

        /// <summary>
        /// Additional action that is performed when a mouse button is released. Serves as a
        /// tie-in for the front-end of an application.
        /// </summary>
        public Action<MouseEventArgs> AdditionalActionOnMouseUp { get; set; }

        /// <summary>
        /// Additional action that is performed when the mouse wheel is scrolled. Serves as a
        /// tie-in for the front-end of an application.
        /// </summary>
        public Action<MouseEventArgs> AdditionalActionOnMouseWheel { get; set; }

        /// <summary>
        /// Additional action that is performed when the mouse is moved. Serves as a
        /// tie-in for the front-end of an application.
        /// </summary>
        public Action<MouseEventArgs> AdditionalActionOnMouseMove { get; set; }

        /// <summary>
        /// Additional action that is performed when the recording is started. Serves as
        /// a tie-in for the front-end of an application
        /// </summary>
        public Action AdditionalActionOnStartRecording { get; set; }

        /// <summary>
        /// Additional action that is performed when the recording is stopped. Serves as
        /// a tie-in for the front-end of an application
        /// </summary>
        public Action AdditionalActionOnStopRecording { get; set; }

        #endregion 

        public GlobalRecordingService() : this(new FileService()) { }
        public GlobalRecordingService(IFileService fileService)
        {
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            ResetRecordedActions();
        }

        #region IGlobalRecordingService Methods

        /// <summary>
        /// Registers the <paramref name="startRecordingCombination"/> and <paramref name="stopRecordingCombination"/> combinations
        /// for starting and stopping the recording.
        /// </summary>
        /// <param name="startRecordingCombination">The key combination that starts the recording.</param>
        /// <param name="stopRecordingCombination">The key combination that stops the recording.</param>
        public void RegisterCombinations(Combination startRecordingCombination, Combination stopRecordingCombination)
        {
            _startRecordingCombination = startRecordingCombination;
            _stopRecordingCombination = stopRecordingCombination;

            // Subscribe to the appropriate key combination depending on if it is current recording.
            SetSubscriptionToPressedKeysKeyboardEvents(true);
            SubscribeToStartStopCombinations();
        }

        /// <summary>
        /// Unsubscribes the recording from the service and stops all global listeners if any are running.
        /// </summary>
        public void Unsubscribe()
        {
            if (GlobalEventsHook == null)
                return;

            UnsubscribeAllRecordingEventHandlers();

            // Dispose the global listeners.
            GlobalEventsHook.Dispose();
            GlobalEventsHook = null;
        }

        /// <summary>
        /// Subscribes the recording to global keyboard and mouse events, and listens for the stop recording combination.
        /// </summary>
        /// <param name="appendToExisting">Indicates whether the recording should append any new actions to the current recording.</param>
        public void StartRecording(bool appendToExisting=true)
        {
            // Don't allow the recording to start numerous times.
            if (_isRecording)
                return;

            _isRecording = true;

            if (!appendToExisting)
                _currentRecording.Actions = new List<IRecordedAction>();

            // Unsubscribe from any previous key and mouse event handlers.
            UnsubscribeAllRecordingEventHandlers();

            // Subscribe to key & mouse events
            SetSubscriptionToRecordingKeyboardEvents(true);
            SetSubscriptionToRecordingMouseEvents(true);
            SetSubscriptionToPressedKeysKeyboardEvents(true);
            SubscribeToStartStopCombinations();

            // Add object to list of recorded actions.
            _currentRecording.Actions.Add(new RecordedStart() 
            { 
                TimeRecorded = SystemTime.Now().Ticks 
            });

            // Invoke the added action
            AdditionalActionOnStartRecording?.Invoke();
        }

        /// <summary>
        /// Unsubscribes the recording to global keyboard and mouse events, and listens for the start recording combination.
        /// </summary>
        public void StopRecording()
        {
            // Don't allow the recording to stop numerous times.
            if (!_isRecording)
                return;

            _isRecording = false;

            // Add object to list of recorded actions.
            _currentRecording.Actions.Add(new RecordedStop()
            {
                TimeRecorded = SystemTime.Now().Ticks
            });

            // Unsubscribe from any previous key & mouse event handlers.
            // This is especially useful if this method is called numerous times by accident.
            UnsubscribeAllRecordingEventHandlers();

            // Subscribe to the start stop key event handlers.
            SetSubscriptionToPressedKeysKeyboardEvents(true);
            SubscribeToStartStopCombinations();

            // Forget any pressed keys
            _pressedKeys = new HashSet<Keys>();

            // Invoke the added action
            AdditionalActionOnStopRecording?.Invoke();
        }

        /// <summary>
        /// Removes any existing recorded actions and any click-zones if the <paramref name="removeClickZones"/> flag is set.
        /// </summary>
        public void ResetRecordedActions(bool removeClickZones=true)
        {
            if (_isRecording)
                StopRecording();

            if (_currentRecording == null)
                _currentRecording = new Recording();

            _currentRecording.Actions = new List<IRecordedAction>();
            _currentRecording.Zones = removeClickZones || _currentRecording.Zones == null ? new List<IClickZone>() : _currentRecording.Zones;

            _pressedKeys = new HashSet<Keys>();
        }

        /// <summary>
        /// Saves the recording to the desired <paramref name="filePath"/>.
        /// </summary>
        public void Save(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));

            StopRecording();

            _currentRecording.Date = SystemTime.Now();
            _currentRecording.FilePath = filePath;

            // Save the recording.
            _fileService.SaveRecording(_currentRecording);
        }

        #endregion
        #region Subscription Helpers
        /// <summary>
        /// Unsubscribes the recording from all key and mouse event handlers.
        /// </summary>
        private void UnsubscribeAllRecordingEventHandlers()
        {
            SetSubscriptionToPressedKeysKeyboardEvents(false);
            SetSubscriptionToRecordingKeyboardEvents(false);
            SetSubscriptionToRecordingMouseEvents(false);
            SetSubscriptionToStartRecordingCombinationEvents(false);
            SetSubscriptionToStopRecordingCombinationEvents(false);
        }

        /// <summary>
        /// Subscribes to the appropriate start/stop key combinations depending on if it is
        /// currently recording.
        /// </summary>
        private void SubscribeToStartStopCombinations()
        {
            SetSubscriptionToStartRecordingCombinationEvents(!_isRecording);
            SetSubscriptionToStopRecordingCombinationEvents(_isRecording);
        }

        /// <summary>
        /// Sets subscription to the key event handlers that store and remove pressed keys.
        /// </summary>
        /// <param name="subscribe">Sets the subscription status of the global key event handlers.</param>
        private void SetSubscriptionToPressedKeysKeyboardEvents(bool subscribe)
        {
            GlobalEventsHook.KeyDown -= OnKeyDownStorePressedKey;
            GlobalEventsHook.KeyUp -= OnKeyUpRemovePressedKey;

            if (subscribe)
            {
                GlobalEventsHook.KeyDown += OnKeyDownStorePressedKey;
                GlobalEventsHook.KeyUp += OnKeyUpRemovePressedKey;
            }
        }

        /// <summary>
        /// Subscribes or unsubscribes the recording from global key listeners.
        /// </summary>
        /// <param name="subscribe">Sets the subscription status of the global key listeners.</param>
        private void SetSubscriptionToRecordingKeyboardEvents(bool subscribe)
        {
            GlobalEventsHook.KeyDown -= OnKeyDownAddRecordedAction;
            GlobalEventsHook.KeyUp -= OnKeyUpAddRecordedAction;

            if (subscribe)
            {
                // Reset subscription to shared keyboard events so that the shared
                // keyboard event handlers fire last.
                GlobalEventsHook.KeyDown += OnKeyDownAddRecordedAction;
                GlobalEventsHook.KeyUp += OnKeyUpAddRecordedAction;
            }
        }

        /// <summary>
        /// Subscribes or unsubscribes the recording from global mouse listeners.
        /// </summary>
        /// <param name="subscribe">Sets the subscription status of the global mouse listeners.</param>
        private void SetSubscriptionToRecordingMouseEvents(bool subscribe)
        {
            GlobalEventsHook.MouseDown -= OnMouseDownAddRecordedAction;
            GlobalEventsHook.MouseUp -= OnMouseUpAddRecordedAction;
            GlobalEventsHook.MouseMove -= OnMouseMoveAddRecordedAction;
            GlobalEventsHook.MouseWheel -= OnMouseWheelAddRecordedAction;

            if (subscribe)
            {
                GlobalEventsHook.MouseDown += OnMouseDownAddRecordedAction;
                GlobalEventsHook.MouseUp += OnMouseUpAddRecordedAction;
                GlobalEventsHook.MouseMove += OnMouseMoveAddRecordedAction;
                GlobalEventsHook.MouseWheel += OnMouseWheelAddRecordedAction;
            }
        }

        /// <summary>
        /// Subscribes or unsubscribes the recording from global key listeners waiting for the start recording combination.
        /// </summary>
        /// <param name="subscribe">Sets the subscription status of the global key combination listeners.</param>
        private void SetSubscriptionToStartRecordingCombinationEvents(bool subscribe)
        {
            GlobalEventsHook.KeyDown -= OnKeyDownCheckIfStartRecordingCombinationPressed;

            if (subscribe && _startRecordingCombination != null)
            {
                GlobalEventsHook.KeyDown += OnKeyDownCheckIfStartRecordingCombinationPressed;
            }
        }

        /// <summary>
        /// Subscribes or unsubscribes the recording from global key listeners waiting for the stop recording combination.
        /// </summary>
        /// <param name="subscribe">Sets the subscription status of the global key combination listeners.</param>
        private void SetSubscriptionToStopRecordingCombinationEvents(bool subscribe)
        {
            GlobalEventsHook.KeyDown -= OnKeyDownCheckIfStopRecordingCombinationPressed;

            if (subscribe && _stopRecordingCombination != null)
            {
                GlobalEventsHook.KeyDown += OnKeyDownCheckIfStopRecordingCombinationPressed;
            }
        }

        /// <summary>
        /// Event that triggers the recording to start if the start recording key combination is pressed.
        /// </summary>
        private void OnKeyDownCheckIfStartRecordingCombinationPressed(object sender, KeyEventArgs e)
        {
            OnCombinationPerformAction(_startRecordingCombination, () => StartRecording());
        }

        /// <summary>
        /// Event that triggers the recording to stop if the stop recording key combination is pressed.
        /// </summary>
        private void OnKeyDownCheckIfStopRecordingCombinationPressed(object sender, KeyEventArgs e)
        {
            OnCombinationPerformAction(_stopRecordingCombination, () => StopRecording());
        }
        
        /// <summary>
        /// Performs the desired <paramref name="action"/> if the key <paramref name="combination"/> is currently pressed.
        /// </summary>
        /// <param name="combination">The key combination that triggers the <paramref name="action"/> to start.</param>
        /// <param name="action">The action to perform if the <paramref name="combination"/> is pressed.</param>
        private void OnCombinationPerformAction(Combination combination, Action action)
        {
            // If all combination keys are pressed, invoke the action.
            if (_pressedKeys.Contains(combination.TriggerKey) && combination.Chord.All(ck => _pressedKeys.Contains(ck)))
            {
                action.Invoke();
            }
        }

        /// <summary>
        /// Event handler for key presses that updates the variable storing which keys are currently pressed.
        /// </summary>
        private void OnKeyDownStorePressedKey(object sender, KeyEventArgs e)
        {
            if (!_pressedKeys.Contains(e.KeyCode))
                _pressedKeys.Add(e.KeyCode);
        }

        /// <summary>
        /// Event handler for key releases that updates the variable storing which keys are currently pressed.
        /// </summary>
        private void OnKeyUpRemovePressedKey(object sender, KeyEventArgs e)
        {
            if (_pressedKeys.Contains(e.KeyCode))
                _pressedKeys.Remove(e.KeyCode);
        }

        /// <summary>
        /// Event that adds a new keyboard button press object to the recording when triggered.
        /// </summary>
        private void OnKeyDownAddRecordedAction(object sender, KeyEventArgs e)
        {
            _currentRecording.Actions.Add(new RecordedKeyboardButtonPress()
            {
                TimeRecorded = SystemTime.Now().Ticks,
                Key = e.KeyCode
            });

            AdditionalActionOnKeyDown?.Invoke(e);
        }

        /// <summary>
        /// Event that adds a new keyboard button release object to the recording when triggered.
        /// </summary>
        private void OnKeyUpAddRecordedAction(object sender, KeyEventArgs e)
        {
            _currentRecording.Actions.Add(new RecordedKeyboardButtonRelease()
            {
                TimeRecorded = SystemTime.Now().Ticks,
                Key = e.KeyCode
            });

            AdditionalActionOnKeyUp?.Invoke(e);
        }

        /// <summary>
        /// Event that adds a new mouse move object to the recording when triggered.
        /// </summary>
        private void OnMouseMoveAddRecordedAction(object sender, MouseEventArgs e)
        {
            _currentRecording.Actions.Add(new RecordedMouseMove()
            {
                TimeRecorded = SystemTime.Now().Ticks,
                XCoordinate = e.X,
                YCoordinate = e.Y
            });

            AdditionalActionOnMouseMove?.Invoke(e);
        }

        /// <summary>
        /// Event that adds a new mouse button press object to the recording when triggered.
        /// </summary>
        private void OnMouseDownAddRecordedAction(object sender, MouseEventArgs e)
        {
            _currentRecording.Actions.Add(new RecordedMouseButtonPress()
            {
                TimeRecorded = SystemTime.Now().Ticks,
                Button = e.Button,
                PixelColor = Color.Red
            });

            AdditionalActionOnMouseDown?.Invoke(e);
        }

        /// <summary>
        /// Event that adds a new mouse button release object to the recording when triggered.
        /// </summary>
        private void OnMouseUpAddRecordedAction(object sender, MouseEventArgs e)
        {
            _currentRecording.Actions.Add(new RecordedMouseButtonRelease()
            {
                TimeRecorded = SystemTime.Now().Ticks,
                Button = e.Button,
                PixelColor = Color.Red
            });

            AdditionalActionOnMouseUp?.Invoke(e);
        }

        /// <summary>
        /// Event that adds a new mouse wheel scroll object to the recording when triggered.
        /// </summary>
        private void OnMouseWheelAddRecordedAction(object sender, MouseEventArgs e)
        {
            _currentRecording.Actions.Add(new RecordedMouseWheelScroll()
            {
                TimeRecorded = SystemTime.Now().Ticks,
                Delta = e.Delta
            });

            AdditionalActionOnMouseWheel?.Invoke(e);
        }

        #endregion
    }
}
