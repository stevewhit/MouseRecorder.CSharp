using System;
using System.Drawing;
using System.Windows.Forms;

namespace MouseRecorder.CSharp.DataModel.Actions
{
    public interface IRecordedMouseButtonAction : IRecordedMouseAction
    {
        /// <summary>
        /// The button for this mouse action.
        /// </summary>
        MouseButtons Button { get; set; }

        /// <summary>
        /// The color of the pixel on the screen.
        /// </summary>
        Color PixelColor { get; set; }
    }

    public abstract class RecordedMouseButtonActionBase : RecordedMouseActionBase, IRecordedMouseButtonAction
    {
        /// <summary>
        /// The button for this mouse action.
        /// </summary>
        public MouseButtons Button { get; set; }

        /// <summary>
        /// The color of the pixel on the screen.
        /// </summary>
        public Color PixelColor { get; set; }  

        protected RecordedMouseButtonActionBase() { }
    }
}
