using System;

namespace MouseRecorder.CSharp.DataModel.Actions
{
    public interface IRecordedKeyboardAction : IKeyboardAction, IRecordedAction
    {
        
    }

    public abstract class RecordedKeyboardActionBase : KeyboardActionBase, IRecordedKeyboardAction
    {
        /// <summary>
        /// The date that this action was captured.
        /// </summary>
        public DateTime Date { get; set; }
    }
}
