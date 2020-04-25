using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MouseRecorder.CSharp.DataModel.Actions
{
    public interface IAction
    {
        /// <summary>
        /// The Id of the action.
        /// </summary>
        int Id { get; set; }
    }

    public abstract class ActionBase : IAction
    {
        /// <summary>
        /// The Id of the action.
        /// </summary>
        public int Id { get; set; }

        protected ActionBase() { }
    }
}
