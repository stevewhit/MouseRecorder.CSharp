﻿using Framework.Generic.Utility;
using Framework.Generic.Graphics;
using Gma.System.MouseKeyHook;
using MouseRecorder.CSharp.DataModel.Actions;
using MouseRecorder.CSharp.DataModel.Configuration;
using MouseRecorder.CSharp.DataModel.Zone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

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
        /// Returns the number of ticks that have elapsed for the current recording.
        /// </summary>
        long CurrentRecordingTicks { get; }

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
        /// <param name="removeClickZones">Flag to indicate if the click-zones should be removed.</param>
        void ResetRecording(bool removeClickZones = true);

        /// <summary>
        /// Saves the recording to the desired <paramref name="filePath"/>.
        /// </summary>
        /// <param name="filePath">The file path where the recording should be saved.</param>
        void Save(string filePath);

        /// <summary>
        /// Adds a click-zone rectangle to the recording.
        /// </summary>
        /// <param name="clickZoneRectangle">The click-zone rectangle to add to the recording.</param>
        void AddClickZone(Rectangle clickZoneRectangle);

        /// <summary>
        /// Adds a range of click-zone rectangles to the recording.
        /// </summary>
        /// <param name="clickZoneRectangles">The click-zone rectangles to add to the recording.</param>
        void AddClickZones(IEnumerable<Rectangle> clickZoneRectangles);
    }

    public class GlobalRecordingService : IGlobalRecordingService
    {
        private readonly IFileService _fileService;
        private IRecording _currentRecording;
        private Combination _startRecordingCombination;
        private Combination _stopRecordingCombination;
        private ISet<Keys> _pressedKeys;
        private bool _isRecordingActions;        
        private readonly Stopwatch _recordingStopwatch;

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

        /// <summary>
        /// Returns the number of ticks that have elapsed for the current recording.
        /// </summary>
        public long CurrentRecordingTicks => _recordingStopwatch.Elapsed.Ticks;

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
            _recordingStopwatch = new Stopwatch();

            ResetRecording();
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
            if (_isRecordingActions)
                return;

            _isRecordingActions = true;

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
                TimeRecorded = CurrentRecordingTicks
            });

            // Start the stopwatch if it isn't running.
            if (!_recordingStopwatch.IsRunning)
                _recordingStopwatch.Start();

            // Invoke the added action
            AdditionalActionOnStartRecording?.Invoke();
        }

        /// <summary>
        /// Unsubscribes the recording to global keyboard and mouse events, and listens for the start recording combination.
        /// </summary>
        public void StopRecording()
        {
            // Don't allow the recording to stop numerous times.
            if (!_isRecordingActions)
                return;

            _isRecordingActions = false;

            // Add object to list of recorded actions.
            _currentRecording.Actions.Add(new RecordedStop()
            {
                TimeRecorded = CurrentRecordingTicks
            });

            // Unsubscribe from any previous key & mouse event handlers.
            // This is especially useful if this method is called numerous times by accident.
            UnsubscribeAllRecordingEventHandlers();

            // Remove the start/stop combination keys from the recording that was just stopped.
            RemoveStartStopCombinationKeysFromLastRecording();

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
        /// Also resets the recording timer.
        /// </summary>
        /// <param name="removeClickZones">Flag to indicate if the click-zones should be removed.</param>
        public void ResetRecording(bool removeClickZones=true)
        {
            if (_isRecordingActions)
                StopRecording();

            if (_currentRecording == null)
                _currentRecording = new Recording();

            _currentRecording.Actions = new List<IRecordedAction>();
            _currentRecording.Zones = removeClickZones || _currentRecording.Zones == null ? new List<IClickZone>() : _currentRecording.Zones;

            _pressedKeys = new HashSet<Keys>();

            _recordingStopwatch.Reset();
        }

        /// <summary>
        /// Saves the recording to the desired <paramref name="filePath"/>.
        /// </summary>
        /// <param name="filePath">The file path where the recording should be saved.</param>
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

        /// <summary>
        /// Adds a click-zone rectangle to the recording.
        /// </summary>
        /// <param name="clickZoneRectangle">The click-zone rectangle to add to the recording.</param>
        public void AddClickZone(Rectangle clickZoneRectangle)
        {
            if (clickZoneRectangle == null)
                throw new ArgumentNullException(nameof(clickZoneRectangle));

            _currentRecording.Zones.Add(new ClickZone() { Shape = clickZoneRectangle });
        }

        /// <summary>
        /// Adds a range of click-zone rectangles to the recording.
        /// </summary>
        /// <param name="clickZoneRectangles">The click-zone rectangles to add to the recording.</param>
        public void AddClickZones(IEnumerable<Rectangle> clickZoneRectangles)
        {
            if (clickZoneRectangles == null)
                throw new ArgumentNullException(nameof(clickZoneRectangles));

            clickZoneRectangles.ForEach(r => AddClickZone(r));
        }

        #endregion
        #region Subscription Event Handlers
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
            SetSubscriptionToStartRecordingCombinationEvents(!_isRecordingActions);
            SetSubscriptionToStopRecordingCombinationEvents(_isRecordingActions);
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
            // Consolidate the all variations of this key (LShift == RShift)
            var pressedKey = e.KeyCode.Consolidate();

            if (!_pressedKeys.Contains(pressedKey))
                _pressedKeys.Add(pressedKey);
        }

        /// <summary>
        /// Event handler for key releases that updates the variable storing which keys are currently pressed.
        /// </summary>
        private void OnKeyUpRemovePressedKey(object sender, KeyEventArgs e)
        {
            // Consolidate the all variations of this key (LShift == RShift)
            var pressedKey = e.KeyCode.Consolidate();

            if (_pressedKeys.Contains(pressedKey))
                _pressedKeys.Remove(pressedKey);
        }

        /// <summary>
        /// Event that adds a new keyboard button press object to the recording when triggered.
        /// </summary>
        private void OnKeyDownAddRecordedAction(object sender, KeyEventArgs e)
        {
            _currentRecording.Actions.Add(new RecordedKeyboardButtonPress()
            {
                TimeRecorded = CurrentRecordingTicks,
                Key = e.KeyCode.Consolidate()
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
                TimeRecorded = CurrentRecordingTicks,
                Key = e.KeyCode.Consolidate()
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
                TimeRecorded = CurrentRecordingTicks,
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
                TimeRecorded = CurrentRecordingTicks,
                Button = e.Button,
                PixelARGBValue = Monitor.GetPixelARGB(e.X, e.Y)
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
                TimeRecorded = CurrentRecordingTicks,
                Button = e.Button
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
                TimeRecorded = CurrentRecordingTicks,
                Delta = e.Delta
            });

            AdditionalActionOnMouseWheel?.Invoke(e);
        }

        #endregion
        #region Additional Methods

        private void RemoveStartStopCombinationKeysFromLastRecording()
        {
            var lastRecordedStartAction = _currentRecording.Actions.LastOrDefault(a => a is IRecordedStart);
            var lastRecordingActions = _currentRecording.Actions.Where(a => a.TimeRecorded >= lastRecordedStartAction.TimeRecorded);

            // Serialize the start recording key combination keys
            var startRecordingKeys = new List<Keys> { _startRecordingCombination.TriggerKey };
            startRecordingKeys.AddRange(_startRecordingCombination.Chord);

            // Cycle through each of the keys in the start recording key combination, and verify 
            // that the beginning of the last recording doesn't have a key release before a key press.
            foreach (var key in startRecordingKeys)
            {
                var firstKeyUpAction = lastRecordingActions.FirstOrDefault(a => (a as RecordedKeyboardButtonRelease)?.Key == key);
                var firstKeyDownAction = lastRecordingActions.FirstOrDefault(a => (a as RecordedKeyboardButtonPress)?.Key == key);

                // If there aren't any key-up actions for this key, skip.
                if (firstKeyUpAction == null)
                    continue;

                // If there is a key-up action and no key-down action at all OR
                // If the key-up action takes place before the first key-down action,
                //  ==> Remove the first key-up action.
                else if (firstKeyDownAction == null || firstKeyUpAction.TimeRecorded < firstKeyDownAction.TimeRecorded)
                {
                    _currentRecording.Actions.Remove(firstKeyUpAction);
                }
            }

            // Serialize the stop recording key combination keys
            var stopRecordingKeys = new List<Keys> { _stopRecordingCombination.TriggerKey };
            stopRecordingKeys.AddRange(_stopRecordingCombination.Chord);

            // Cycle through each of the keys in the stop recording key combination, and verify
            // that the end of the current recording doesn't have a key press before a key release.
            foreach (var key in stopRecordingKeys)
            {
                var lastKeyUpAction = lastRecordingActions.LastOrDefault(a => (a as RecordedKeyboardButtonRelease)?.Key == key);
                var lastKeyDownAction = lastRecordingActions.LastOrDefault(a => (a as RecordedKeyboardButtonPress)?.Key == key);

                // If there aren't any key-down actions for this key, skip.
                if (lastKeyDownAction == null)
                    continue;

                // If there is a key-down action and no key-up action at all OR
                // If the key-down action takes place after the last key-up action,
                //  ==> Remove the last key-down action.
                else if (lastKeyUpAction == null || lastKeyDownAction.TimeRecorded > lastKeyUpAction.TimeRecorded)
                {
                    _currentRecording.Actions.Remove(lastKeyDownAction);
                }
            }
        }

        #endregion
    }
}
