using MouseRecorder.CSharp.DataModel.Actions;
using MouseRecorder.CSharp.DataModel.Configuration.Base;
using System.Collections.Generic;

namespace MouseRecorder.CSharp.DataModel.Configuration
{
    public interface IUnloadedRecording : IRecording
    {
        /// <summary>
        /// The actions that take place during this recording.
        /// </summary>
        IList<IRecordedAction> Actions { get; set; }
    }

    public class UnloadedRecording : RecordingBase, IUnloadedRecording
    {
        /// <summary>
        /// The actions that take place during this recording.
        /// </summary>
        public IList<IRecordedAction> Actions { get; set; }
    }
}
