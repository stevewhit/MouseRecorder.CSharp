
namespace MouseRecorder.CSharp.DataModel.Actions
{
    public interface IRecordedMouseButtonPress : IRecordedMouseButtonAction
    {
        /// <summary>
        /// The ARGB color value of the pixel on the screen.
        /// </summary>
        int PixelARGBValue{ get; set; }
    }

    public class RecordedMouseButtonPress : RecordedMouseButtonActionBase, IRecordedMouseButtonPress
    {
        /// <summary>
        /// The ARGB color value of the pixel on the screen.
        /// </summary>
        public int PixelARGBValue { get; set; }
    }
}
