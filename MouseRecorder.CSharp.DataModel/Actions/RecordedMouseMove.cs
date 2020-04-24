using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MouseRecorder.CSharp.DataModel.Actions
{
    public interface IRecordedMouseMove : IRecordedMouseAction
    {
        /// <summary>
        /// The (X,Y) screen coordinate of this mouse move.
        /// </summary>
        Point ScreenCoordinate { get; set; }
    }

    public class RecordedMouseMove : RecordedMouseActionBase, IRecordedMouseMove
    {
        /// <summary>
        /// The (X,Y) screen coordinate of this mouse move.
        /// </summary>
        public Point ScreenCoordinate { get; set; }
    }
}
