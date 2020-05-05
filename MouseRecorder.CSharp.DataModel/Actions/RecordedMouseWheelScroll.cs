
namespace MouseRecorder.CSharp.DataModel.Actions
{
    public interface IRecordedMouseWheelScroll : IRecordedMouseAction
    {
        /// <summary>
        /// The amount of wheel scroll.
        /// </summary>
        int Delta { get; set; }
    }

    public class RecordedMouseWheelScroll : RecordedMouseActionBase, IRecordedMouseWheelScroll
    {
        /// <summary>
        /// The amount of wheel scroll.
        /// </summary>
        public int Delta { get; set; }
    }
}
