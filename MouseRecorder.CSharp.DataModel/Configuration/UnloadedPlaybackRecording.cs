using MouseRecorder.CSharp.DataModel.Actions;
using MouseRecorder.CSharp.DataModel.Configuration.Base;
using System.Collections.Generic;

namespace MouseRecorder.CSharp.DataModel.Configuration
{
    public interface IUnloadedPlaybackRecording : IPlaybackRecording
    {
        /// <summary>
        /// The unloaded actions that will take place during this recording.
        /// </summary>
        IList<IRecordedAction> Actions { get; set; }
    }

    public class UnloadedPlaybackRecording : PlaybackRecordingBase, IUnloadedPlaybackRecording
    {
        /// <summary>
        /// The unloaded actions that will take place during this recording.
        /// </summary>
        public IList<IRecordedAction> Actions { get; set; }
    }
}
