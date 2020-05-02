using System;

namespace MouseRecorder.CSharp.DataModel.Actions
{
    public interface IRecordedAction
    {
        /// <summary>
        /// The time (in ticks) that this action was captured.
        /// </summary>
        long TimeRecorded { get; set; }
    }
}
