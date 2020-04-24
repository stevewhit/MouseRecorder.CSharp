using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MouseRecorder.CSharp.DataModel.Actions
{
    public interface IPlaybackMouseButtonPress : IPlaybackMouseAction
    {
        
    }

    public class PlaybackMouseButtonPress : PlaybackMouseActionBase, IPlaybackMouseButtonPress
    {
        
    }
}
