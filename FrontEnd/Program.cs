using Akka.Actor;
using Akka.Configuration;
using Akka.Routing;
using FrontEnd.Actor;
using Message;
using System;
using System.Diagnostics;

namespace Frontend
{
    internal class Program
    {
        public static ActorSystem ClusterSystem;
        private static IActorRef StartActor;

        private static void Main(string[] args)
        {
            Console.Title = "FrontEnd";
            var config = ConfigurationFactory.ParseString(@"
akka
    {
    loglevel = INFO
    loggers=[""Akka.Logger.NLog.NLogLogger, Akka.Logger.NLog""]
    actor {
		provider=cluster
        deployment {
        /tasker {
          router = round-robin-group # routing strategy
            routees.paths = [""/user/starter""]
            nr-of-instances = 5 # max number of total routees
          cluster {
             enabled = on
             allow-local-routees = off
             use-role = tasker
          }
        }
      }
        debug
            {
              receive = on      # log any received message
              autoreceive = on  # log automatically received messages, e.g. PoisonPill
              lifecycle = on    # log actor lifecycle changes
              event-stream = on # log subscription changes for Akka.NET event stream
              unhandled = on    # log unhandled messages sent to actors
            }
        }
    remote
        {
        dot-netty.tcp {
        port = 8081
        hostname = ""localhost""
        }
    }

    cluster {
        seed-nodes = [""akka.tcp://ClusterSystem@localhost:8081""]
        roles=[""tasker""]
        }
}
");
            ClusterSystem = ActorSystem.Create("ClusterSystem", config);
            var routerActor = ClusterSystem.ActorOf(Props.Empty.WithRouter(FromConfig.Instance), "tasker");

            StartActor = ClusterSystem.ActorOf(Props.Create(() => new StartActor(routerActor)), "startactor");
            StartActor.Tell(new Initiate());
            ClusterSystem.WhenTerminated.Wait();
        }
    }
}