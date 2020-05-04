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
        /// Subscribes the recording <paramref name="configuration"/> to the service and starts the 
        /// global start recording combination listener.
        /// </summary>
        /// <param name="configuration">The configuration to use when recording.</param>
        void Subscribe(IRecordingConfiguration configuration);

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
        /// Saves the recording to the desired <paramref name="filePath"/>.
        /// </summary>
        void Save(string filePath);
    }

    public class GlobalRecordingService : IGlobalRecordingService
    {
        private readonly IFileService _fileService;
        private readonly IRecording _currentRecording;
        private IRecordingConfiguration _subscribedConfiguration;
        private ISet<Keys> _pressedKeys;
        private static IKeyboardMouseEvents _globalKeyMouseEvents;

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
            _currentRecording = new Recording()
            {
                Actions = new List<IRecordedAction>(),
                Zones = new List<IClickZone>()
            };
            _pressedKeys = new HashSet<Keys>();
        }

        /// <summary>
        /// Subscribes the recording <paramref name="configuration"/> to the service and starts the 
        /// global start recording combination listener.
        /// </summary>
        /// <param name="configuration">The configuration to use when recording.</param>
        public void Subscribe(IRecordingConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            // Unsubscribe any previous event handlers.
            Unsubscribe();

            _subscribedConfiguration = configuration;
            _globalKeyMouseEvents = Hook.GlobalEvents();

            // Subscribe to the start recording key combination
            SetSubscriptionToSharedKeyboardEvents(true);
            SetSubscriptionToStartRecordingCombinationEvents(true);
            SetSubscriptionToStopRecordingCombinationEvents(false);
        }

        /// <summary>
        /// Unsubscribes the recording from the service and stops all global listeners if any are running.
        /// </summary>
        public void Unsubscribe()
        {
            _subscribedConfiguration = null;

            // Dispose the global listeners.
            _globalKeyMouseEvents?.Dispose();
            _globalKeyMouseEvents = null;
        }

        /// <summary>
        /// Subscribes the recording to global keyboard and mouse events, and listens for the stop recording combination.
        /// </summary>
        /// <param name="appendToExisting">Indicates whether the recording should append any new actions to the current recording.</param>
        public void StartRecording(bool appendToExisting=true)
        {
            if (!appendToExisting)
                _currentRecording.Actions = new List<IRecordedAction>();

            if (_subscribedConfiguration == null)
                throw new InvalidOperationException("The service has not been subscribed to yet.");

            // Unsubscribe from any previous key & mouse event handlers.
            // This is especially useful if this method is called numerous times by accident.
            UnsubscribeAllRecordingEventHandlers();

            // Subscribe to key & mouse events
            SetSubscriptionToRecordingKeyboardEvents(true);
            SetSubscriptionToRecordingMouseEvents(true);
            SetSubscriptionToStartRecordingCombinationEvents(false);
            SetSubscriptionToStopRecordingCombinationEvents(true);

            // Invoke the added action
            AdditionalActionOnStartRecording?.Invoke();
        }

        /// <summary>
        /// Unsubscribes the recording to global keyboard and mouse events, and listens for the start recording combination.
        /// </summary>
        public void StopRecording()
        {
            if (_subscribedConfiguration == null)
                throw new InvalidOperationException("The service has not been subscribed to yet.");

            // Unsubscribe from any previous key & mouse event handlers.
            // This is especially useful if this method is called numerous times by accident.
            UnsubscribeAllRecordingEventHandlers();

            // Unsubscribe to key & mouse events
            SetSubscriptionToRecordingKeyboardEvents(false);
            SetSubscriptionToRecordingMouseEvents(false);
            SetSubscriptionToStartRecordingCombinationEvents(true);
            SetSubscriptionToStopRecordingCombinationEvents(false);

            // Forget any pressed keys -- Needs to be moved somewhere else because we can't add time information if we want to add a key-release action. ========================================================================================================================
            _pressedKeys = new HashSet<Keys>();

            // Invoke the added action
            AdditionalActionOnStopRecording?.Invoke();
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

        /// <summary>
        /// Unsubscribes the recording from all key & mouse event handlers.
        /// </summary>
        private void UnsubscribeAllRecordingEventHandlers()
        {
            SetSubscriptionToRecordingKeyboardEvents(false);
            SetSubscriptionToRecordingMouseEvents(false);
            SetSubscriptionToStartRecordingCombinationEvents(false);
            SetSubscriptionToStopRecordingCombinationEvents(false);
        }

        /// <summary>
        /// Sets subscription to the shared global key event handlers.
        /// </summary>
        /// <param name="subscribe">Sets the subscription status of the global key event handlers.</param>
        private void SetSubscriptionToSharedKeyboardEvents(bool subscribe)
        {
            if (subscribe)
            {
                _globalKeyMouseEvents.KeyDown += OnKeyDownShared;
                _globalKeyMouseEvents.KeyUp += OnKeyUpShared;

            }
            else
            {
                _globalKeyMouseEvents.KeyDown -= OnKeyDownShared;
                _globalKeyMouseEvents.KeyUp -= OnKeyUpShared;
            }
        }

        /// <summary>
        /// Subscribes or unsubscribes the recording from global key listeners.
        /// </summary>
        /// <param name="subscribe">Sets the subscription status of the global key listeners.</param>
        private void SetSubscriptionToRecordingKeyboardEvents(bool subscribe)
        {
            if (subscribe)
            {
                // Reset subscription to shared keyboard events so that the shared
                // keyboard event handlers fire last.
                SetSubscriptionToSharedKeyboardEvents(false);
                _globalKeyMouseEvents.KeyDown += OnKeyDownAddRecordedAction;
                _globalKeyMouseEvents.KeyUp += OnKeyUpAddRecordedAction;
                SetSubscriptionToSharedKeyboardEvents(true);

            }
            else
            {
                _globalKeyMouseEvents.KeyDown -= OnKeyDownAddRecordedAction;
                _globalKeyMouseEvents.KeyUp -= OnKeyUpAddRecordedAction;
            }
        }

        /// <summary>
        /// Subscribes or unsubscribes the recording from global mouse listeners.
        /// </summary>
        /// <param name="subscribe">Sets the subscription status of the global mouse listeners.</param>
        private void SetSubscriptionToRecordingMouseEvents(bool subscribe)
        {
            if (subscribe)
            {
                _globalKeyMouseEvents.MouseDown += OnMouseDownAddRecordedAction;
                _globalKeyMouseEvents.MouseUp += OnMouseUpAddRecordedAction;
                _globalKeyMouseEvents.MouseMove += OnMouseMoveAddRecordedAction;
                _globalKeyMouseEvents.MouseWheel += OnMouseWheelAddRecordedAction;
            }
            else
            {
                _globalKeyMouseEvents.MouseDown -= OnMouseDownAddRecordedAction;
                _globalKeyMouseEvents.MouseUp -= OnMouseUpAddRecordedAction;
                _globalKeyMouseEvents.MouseMove -= OnMouseMoveAddRecordedAction;
                _globalKeyMouseEvents.MouseWheel -= OnMouseWheelAddRecordedAction;
            }
        }

        /// <summary>
        /// Subscribes or unsubscribes the recording from global key listeners waiting for the start recording combination.
        /// </summary>
        /// <param name="subscribe">Sets the subscription status of the global key combination listeners.</param>
        private void SetSubscriptionToStartRecordingCombinationEvents(bool subscribe)
        {
            if (subscribe)
            {
                _globalKeyMouseEvents.KeyDown += OnKeyDownCheckIfStartRecordingCombinationPressed;
            }
            else
            {
                _globalKeyMouseEvents.KeyDown -= OnKeyDownCheckIfStartRecordingCombinationPressed;
            }
        }

        /// <summary>
        /// Subscribes or unsubscribes the recording from global key listeners waiting for the stop recording combination.
        /// </summary>
        /// <param name="subscribe">Sets the subscription status of the global key combination listeners.</param>
        private void SetSubscriptionToStopRecordingCombinationEvents(bool subscribe)
        {
            if (subscribe)
            {
                _globalKeyMouseEvents.KeyDown += OnKeyDownCheckIfStopRecordingCombinationPressed;
            }
            else
            {
                _globalKeyMouseEvents.KeyDown -= OnKeyDownCheckIfStopRecordingCombinationPressed;
            }
        }

        /// <summary>
        /// Event handler for key presses that is active most of the time.
        /// </summary>
        private void OnKeyDownShared(object sender, KeyEventArgs e)
        {
            if (!_pressedKeys.Contains(e.KeyCode))
                _pressedKeys.Add(e.KeyCode);

            AdditionalActionOnKeyDown?.Invoke(e);
        }

        /// <summary>
        /// Event handler for key releases that is active most of the time.
        /// </summary>
        private void OnKeyUpShared(object sender, KeyEventArgs e)
        {
            if (_pressedKeys.Contains(e.KeyCode))
                _pressedKeys.Remove(e.KeyCode);

            AdditionalActionOnKeyUp?.Invoke(e);
        }

        /// <summary>
        /// Event that triggers the recording to start if the start recording key combination is pressed.
        /// </summary>
        private void OnKeyDownCheckIfStartRecordingCombinationPressed(object sender, KeyEventArgs e)
        {
            OnCombinationPerformAction(_subscribedConfiguration.StartRecordingCombination, () => StartRecording());
        }

        /// <summary>
        /// Event that triggers the recording to stop if the stop recording key combination is pressed.
        /// </summary>
        private void OnKeyDownCheckIfStopRecordingCombinationPressed(object sender, KeyEventArgs e)
        {
            OnCombinationPerformAction(_subscribedConfiguration.StopRecordingCombination, () => StopRecording());
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
        /// Event that adds a new keyboard button press object to the recording when triggered.
        /// </summary>
        private void OnKeyDownAddRecordedAction(object sender, KeyEventArgs e)
        {
            _currentRecording.Actions.Add(new RecordedKeyboardButtonPress()
            {
                TimeRecorded = SystemTime.Now().Ticks,
                Key = e.KeyCode
            });
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
    }
}
