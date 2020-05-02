using Gma.System.MouseKeyHook;
using MouseRecorder.CSharp.App.ViewModel;
using MouseRecorder.CSharp.Business.Services;
using MouseRecorder.CSharp.DataModel.Configuration;
using System.Windows;
using SWControls=System.Windows.Controls;
using System.Windows.Forms;

namespace MouseRecorder.CSharp.App.Views
{
    /// <summary>
    /// Interaction logic for RecordView.xaml
    /// </summary>
    public partial class RecordView : SWControls.UserControl
    {
        private readonly RecordViewModel _model;
        private readonly IGlobalRecordingService _service;
        private readonly IRecordingConfiguration _config;

        public RecordView()
        {
            InitializeComponent();
            _model = new RecordViewModel();
            DataContext = _model;

            _config = new RecordingConfiguration
            {
                RecordKeyboardInputs = true,
                RecordMouseInputs = true,
                StartRecordingCombination = Combination.TriggeredBy(Keys.A).With(Keys.S),
                StopRecordingCombination = Combination.TriggeredBy(Keys.A).With(Keys.S)
            };
            
            _service = new GlobalRecordingService();
            _service.Subscribe(_config);
            SubscribeToKeyMouseEvents();
        }

        #region Key Mouse Events

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

        /// <summary>
        /// 
        /// </summary>
        private void OnKeyDown(KeyEventArgs e)
        {
            _model.Actions.Add($"Key Down: {e.KeyCode}");
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnKeyUp(KeyEventArgs e)
        {
            _model.Actions.Add($"Key Up: {e.KeyCode}");
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnMouseMove(MouseEventArgs e)
        {
            _model.Actions.Add($"Mouse Move: {e.Location}");
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnMouseDown(MouseEventArgs e)
        {
            _model.Actions.Add($"Mouse Down: {e.Button}");
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnMouseUp(MouseEventArgs e)
        {
            _model.Actions.Add($"Mouse Up: {e.Button}");
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnMouseWheel(MouseEventArgs e)
        {
            _model.Actions.Add($"Mouse Wheel: {e.Delta}");
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnStartRecording()
        {
            _model.Actions.Add($"Recording started..");
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnStopRecording()
        {
            _model.Actions.Add($"Recording stopped..");
        }

        #endregion

        private void BtnView_Clicked(object sender, RoutedEventArgs e)
        {
            // prompt to save..?
            _service.Unsubscribe();
        }

        private void BtnRecord_Clicked(object sender, RoutedEventArgs e)
        {
            _service.StartRecording();
        }

        private void BtnStop_Clicked(object sender, RoutedEventArgs e)
        {
            _service.StopRecording();
            _service.Save("testestes.txt");
        }
    }
}
