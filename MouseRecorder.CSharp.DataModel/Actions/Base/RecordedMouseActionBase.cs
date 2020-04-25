using System;
using System.Drawing;

namespace MouseRecorder.CSharp.DataModel.Actions
{
    public interface IRecordedMouseAction : IMouseAction, IRecordedAction
    {
        /// <summary>
        /// The color of the pixel on the screen.
        /// </summary>
        Color PixelColor { get; set; }
    }

    public abstract class RecordedMouseActionBase : MouseActionBase, IRecordedMouseAction
    {
        /// <summary>
        /// The date that this action was captured.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// The color of the pixel on the screen.
        /// </summary>
        public Color PixelColor { get; set; }

        protected RecordedMouseActionBase() { }
    }
}
