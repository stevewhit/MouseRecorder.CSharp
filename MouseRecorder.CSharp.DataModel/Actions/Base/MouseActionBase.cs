using System.Drawing;
using System.Windows.Forms;

namespace MouseRecorder.CSharp.DataModel.Actions
{
    public interface IMouseAction : IAction
    {
        /// <summary>
        /// The button for this mouse action.
        /// </summary>
        MouseButtons Button { get; set; }
    }

    public abstract class MouseActionBase : ActionBase, IMouseAction
    {
        /// <summary>
        /// The button for this mouse action.
        /// </summary>
        public MouseButtons Button { get; set; }
    }
}
