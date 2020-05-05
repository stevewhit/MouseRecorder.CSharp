
namespace MouseRecorder.CSharp.DataModel.Actions
{
    public interface IPlaybackKeyboardAction : IPlaybackAction, IKeyboardAction
    {
        
    }

    public abstract class PlaybackKeyboardActionBase : KeyboardActionBase, IPlaybackKeyboardAction
    {
        protected PlaybackKeyboardActionBase() { }
    }
}
