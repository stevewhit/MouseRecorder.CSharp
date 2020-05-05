using System.Drawing;

namespace MouseRecorder.CSharp.DataModel.Actions
{
    public interface IRecordedMouseButtonPress : IRecordedMouseButtonAction
    {
        /// <summary>
        /// The color of the pixel on the screen.
        /// </summary>
        Color PixelColor { get; set; }
    }

    public class RecordedMouseButtonPress : RecordedMouseButtonActionBase, IRecordedMouseButtonPress
    {
        /// <summary>
        /// The color of the pixel on the screen.
        /// </summary>
        public Color PixelColor { get; set; }
    }
}
