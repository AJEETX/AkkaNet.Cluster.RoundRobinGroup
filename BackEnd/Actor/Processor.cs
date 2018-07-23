namespace Backend.Actor
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using Akka.Actor;
    using Message;

    internal class Processor : ReceiveActor, ILogReceive
    {
        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly string name = string.Empty;

        public Processor(string name)
        {
            this.name = name;
            this.Receive<Pnr>(pnr => this.Process(pnr));
        }

        private void Process(Pnr pnr)
        {
            this.logger.Info(NLog.LogLevel.Info);
            Console.WriteLine($" Processing pnr {pnr.Locator}");
            Thread.Sleep(1000);
            this.Sender.Tell(new CompletedResponse());
        }
    }
}