using System.Linq;
using NPPES.Loader.Framework;

namespace NPPES.Loader
{
    public class WebRequestScheduler : AsyncWorkerBase<NPIRequest>
    {

        RoundRobinObjectEnumerator<WebRequestHandler> enumerator;

        private static AsyncWorkerBase<NPIRequest> scheduler;
        public static AsyncWorkerBase<NPIRequest> Instance
        {
            get
            {
                if (scheduler == null)
                    scheduler = new WebRequestScheduler();
                return scheduler;
            }
        }

        private WebRequestScheduler()
        {
            enumerator = new RoundRobinObjectEnumerator<WebRequestHandler>();
        }

        public override void Start()
        {
            base.Start();
            foreach (var item in enumerator.All())
            {
                item.Start();
            }
        }

        protected override void Process(NPIRequest info)
        {
            var handler = enumerator.Next();
            handler.Submit(info);
        }

        public override string Pending()
        {
            var all = enumerator.All();
            var individual = string.Join(" | ", all.Select(x => x.Pending()));
            return $"\t[{individual}]";
        }
    }
}
