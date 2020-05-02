using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MouseRecorder.CSharp.DataModel.Actions
{
    public interface IRecordedMouseButtonRelease : IRecordedMouseButtonAction
    {

    }

    public class RecordedMouseButtonRelease : RecordedMouseButtonActionBase, IRecordedMouseButtonRelease
    {
        
    }
}
