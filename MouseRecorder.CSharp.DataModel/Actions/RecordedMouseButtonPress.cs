using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MouseRecorder.CSharp.DataModel.Actions
{
    public interface IRecordedMouseButtonPress : IRecordedMouseButtonAction
    {

    }

    public class RecordedMouseButtonPress : RecordedMouseButtonActionBase, IRecordedMouseButtonPress
    {
        
    }
}
