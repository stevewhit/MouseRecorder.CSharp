using System.Drawing;
using System.Windows.Forms;

namespace MouseRecorder.CSharp.DataModel.Actions
{
    public interface IMouseAction : IAction
    {
        
    }

    public abstract class MouseActionBase : ActionBase, IMouseAction
    {
        
    }
}
