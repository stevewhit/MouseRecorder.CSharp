using System;
using System.Drawing;

namespace MouseRecorder.CSharp.DataModel.Actions
{
    public interface IRecordedMouseAction : IMouseAction, IRecordedAction
    {

    }

    public abstract class RecordedMouseActionBase : MouseActionBase, IRecordedMouseAction
    {
        /// <summary>
        /// The time (in ticks) that this action was captured.
        /// </summary>
        public long TimeRecorded { get; set; }

        protected RecordedMouseActionBase() { }
    }
}
