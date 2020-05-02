using System;

namespace MouseRecorder.CSharp.DataModel.Actions
{
    public interface IRecordedKeyboardAction : IKeyboardAction, IRecordedAction
    {
        
    }

    public abstract class RecordedKeyboardActionBase : KeyboardActionBase, IRecordedKeyboardAction
    {
        /// <summary>
        /// The time (in ticks) that this action was captured.
        /// </summary>
        public long TimeRecorded { get; set; }

        protected RecordedKeyboardActionBase() { }
    }
}
