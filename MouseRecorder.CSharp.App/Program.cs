using log4net;
using Ninject;
using System.Reflection;
using System;

namespace MouseRecorder.CSharp.App
{
    class Program
    {
        public static void Main(string[] args)
        {
            IKernel kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());

            var log = kernel.Get<ILog>();

            try
            {
                log.Info("Starting..");
            }
            catch (AggregateException e)
            {
                LogRecursive(log, e, "Error occured.");
            }

            log.Info("Finished..");
            kernel.Dispose();
        }

        /// <summary>
        /// Logs the inner exceptions of an AggregateException separately.
        /// </summary>
        private static void LogRecursive(ILog log, AggregateException e, string message)
        {
            foreach (var innerException in e.InnerExceptions)
            {
                if (innerException is AggregateException)
                    LogRecursive(log, innerException as AggregateException, message);
                else
                    log.Error($"{message}", innerException);
            }
        }
    }
}
