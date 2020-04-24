using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MouseRecorder.CSharp.DataModel.Actions
{
    public interface IRecordedKeyboardButtonPress : IRecordedKeyboardAction
    {

    }

    public class RecordedKeyboardButtonPress : RecordedKeyboardActionBase, IRecordedKeyboardButtonPress
    {
        
    }
}
