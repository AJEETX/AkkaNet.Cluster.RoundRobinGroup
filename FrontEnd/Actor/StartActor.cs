using Akka.Actor;
using Akka.Routing;
using Message;
using System.Linq;

namespace FrontEnd.Actor
{
    internal class StartActor : ReceiveActor, ILogReceive
    {
        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private IActorRef router, self;

        public StartActor(IActorRef router)
        {
            self = Self;
            this.router = router;

            Receive<Initiate>(i =>
            {
                var routee = router.Ask<Routees>(new GetRoutees()).ContinueWith(tr =>
                {
                    if (tr.Result.Members.Count() > 0)
                    {
                        Start(i);
                    }
                    else
                    {
                        self.Tell(i);
                    }
                });
            });
        }

        private void Start(Initiate initiate)
        {
            this.logger.Info(NLog.LogLevel.Info);

            router.Tell(initiate);
        }
    }
}