
namespace MouseRecorder.CSharp.DataModel.Actions
{
    public interface IRecordedStart : IRecordedAction
    {

    }

    public class RecordedStart : IRecordedStart
    {
        public long TimeRecorded { get; set; }
    }
}
