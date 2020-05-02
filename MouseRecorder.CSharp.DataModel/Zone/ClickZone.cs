using System.Drawing;

namespace MouseRecorder.CSharp.DataModel.Zone
{
    public interface IClickZone
    {
        /// <summary>
        /// Represents the location and dimensions of this click-zone.
        /// </summary>
        Rectangle Shape { get; set; }
    }

    public class ClickZone : IClickZone
    {
        /// <summary>
        /// Represents the location and dimensions of this click-zone.
        /// </summary>
        public Rectangle Shape { get; set; }
    }
}
