using System.Windows.Forms;

namespace MouseRecorder.CSharp.DataModel.Actions
{
    public interface IRecordedMouseButtonAction : IRecordedMouseAction
    {
        /// <summary>
        /// The button for this mouse action.
        /// </summary>
        MouseButtons Button { get; set; }
    }

    public abstract class RecordedMouseButtonActionBase : RecordedMouseActionBase, IRecordedMouseButtonAction
    {
        /// <summary>
        /// The button for this mouse action.
        /// </summary>
        public MouseButtons Button { get; set; }

        protected RecordedMouseButtonActionBase() { }
    }
}
