using System.Windows.Forms;

namespace MouseRecorder.CSharp.DataModel.Actions
{
    public interface IKeyboardAction : IAction
    {
        /// <summary>
        /// The Key for the keyboard action.
        /// </summary>
        Keys Key { get; set; }
    }

    public abstract class KeyboardActionBase : ActionBase, IKeyboardAction
    {
        /// <summary>
        /// The Key for the keyboard action.
        /// </summary>
        public Keys Key { get; set; }
    }
}
