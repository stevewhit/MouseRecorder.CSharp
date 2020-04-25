using MouseRecorder.CSharp.DataModel.Actions;
using MouseRecorder.CSharp.DataModel.Configuration.Base;
using System.Collections.Generic;

namespace MouseRecorder.CSharp.DataModel.Configuration
{
    public interface ILoadedRecording : IRecording
    {
        /// <summary>
        /// The actions that take place during this recording.
        /// </summary>
        IList<IPlaybackAction> Actions { get; set; }
    }

    public class LoadedRecording : RecordingBase, ILoadedRecording
    {
        /// <summary>
        /// The loaded actions that will take place during this recording.
        /// </summary>
        public IList<IPlaybackAction> Actions { get; set; }
    }
}
