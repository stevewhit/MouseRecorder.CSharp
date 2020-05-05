
namespace MouseRecorder.CSharp.DataModel.Actions
{
    public interface IRecordedMouseMove : IRecordedMouseAction
    {
        /// <summary>
        /// The X screen coordinate of this mouse move.
        /// </summary>
        int XCoordinate { get; set; }

        /// <summary>
        /// The Y screen coordinate of this mouse move.
        /// </summary>
        int YCoordinate { get; set; }
    }

    public class RecordedMouseMove : RecordedMouseActionBase, IRecordedMouseMove
    {
        /// <summary>
        /// The X screen coordinate of this mouse move.
        /// </summary>
        public int XCoordinate { get; set; }

        /// <summary>
        /// The Y screen coordinate of this mouse move.
        /// </summary>
        public int YCoordinate { get; set; }
    }
}
