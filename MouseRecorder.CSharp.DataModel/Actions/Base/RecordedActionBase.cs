
using System;

namespace MouseRecorder.CSharp.DataModel.Actions
{
    public interface IRecordedAction
    {
        /// <summary>
        /// The date that this action was captured.
        /// </summary>
        DateTime Date { get; set; }
    }
}
