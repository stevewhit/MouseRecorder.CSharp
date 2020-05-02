using Gma.System.MouseKeyHook;

namespace MouseRecorder.CSharp.DataModel.Configuration
{
    public interface IRecordingConfiguration
    {
        /// <summary>
        /// The keyboard hot-key combination to start the recording.
        /// </summary>
        Combination StartRecordingCombination { get; set; }

        /// <summary>
        /// The keyboard hot-key combination to stop the recording.
        /// </summary>
        Combination StopRecordingCombination { get; set; }

        /// <summary>
        /// Indicates whether mouse inputs will be captured in the recording.
        /// </summary>
        bool RecordMouseInputs { get; set; }

        /// <summary>
        /// Indicates whether keyboard inputs will be captured in the recording.
        /// </summary>
        bool RecordKeyboardInputs { get; set; }

        /// <summary>
        /// Indicates whether the starting position should be overlayed on screen during the recording.
        /// </summary>
        bool ShowStartingPositionOverlay { get; set; }

        /// <summary>
        /// Indicates whether the recorded actions should be displayed.
        /// </summary>
        bool ShowRecordedActions { get; set; }
    }

    public class RecordingConfiguration : IRecordingConfiguration
    {
        /// <summary>
        /// The keyboard hot-key combination to start the recording.
        /// </summary>
        public Combination StartRecordingCombination { get; set; }

        /// <summary>
        /// The keyboard hot-key combination to stop the recording.
        /// </summary>
        public Combination StopRecordingCombination { get; set; }

        /// <summary>
        /// Indicates whether mouse inputs will be captured in the recording.
        /// </summary>
        public bool RecordMouseInputs { get; set; }

        /// <summary>
        /// Indicates whether keyboard inputs will be captured in the recording.
        /// </summary>
        public bool RecordKeyboardInputs { get; set; }

        /// <summary>
        /// Indicates whether the starting position should be overlayed on screen during the recording.
        /// </summary>
        public bool ShowStartingPositionOverlay { get; set; }

        /// <summary>
        /// Indicates whether the recorded actions should be displayed.
        /// </summary>
        public bool ShowRecordedActions { get; set; }
    }
}
