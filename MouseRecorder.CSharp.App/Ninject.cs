using log4net;
using Ninject.Modules;
using MouseRecorder.CSharp.DataModel;
using Framework.Generic.EntityFramework;

namespace MouseRecorder.CSharp.App
{
    public class NinjectBindings : NinjectModule
    {
        public override void Load()
        {
            var context = new EfContext(new MRContext());

            Bind<ILog>().ToMethod(_ => LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType));
        }
    }
}
