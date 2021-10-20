using System.Linq;
using NPPES.Loader.Framework;

namespace NPPES.Loader
{
    public class WebRequestScheduler : AsyncWorkerBase<NPIRequest>
    {

        RoundRobinObjectEnumerator<WebRequestHandler> enumerator;

        public WebRequestScheduler()
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

        internal override string Pending()
        {
            var all = enumerator.All();
            var individual = string.Join(" | ", all.Select(x => x.Pending()));
            return $"\t[{individual}]";
        }
    }
}
