
namespace MouseRecorder.CSharp.DataModel.Actions
{
    public interface IRecordedStop : IRecordedAction
    {

    }

    public class RecordedStop : IRecordedStop
    {
        public long TimeRecorded { get; set; }
    }
}
