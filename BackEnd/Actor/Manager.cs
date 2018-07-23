namespace Backend.Actor
{
    using System;
    using System.Collections.Generic;
    using Akka.Actor;
    using Message;

    internal class Manager : ReceiveActor, ILogReceive
    {
        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private int pnrCounter = 0;

        public Manager()
        {
            this.Receive<List<Office>>(s => this.Process(s));
            this.Receive<OfficePnrList>(l => this.CallBack(l));
            this.Receive<CompletedResponse>(r => this.Completed(r));
        }

        private void Process(List<Office> officeList)
        {
            foreach (var office in officeList)
            {
                this.logger.Info(NLog.LogLevel.Info);
                var name = office.ID + office.Data;
                var supervisor = Context.Child(name);
                if (supervisor == ActorRefs.Nobody)
                {
                    supervisor = Context.ActorOf(Props.Create(() => new Supervisor(name)), name);
                }

                supervisor.Tell(office);
            }
        }

        private void CallBack(OfficePnrList officePnrList)
        {
            Console.WriteLine($"{officePnrList.Counter} WIP {officePnrList.Office.ID} ::: {officePnrList.Office.Data}");
            foreach (var pnr in officePnrList.Pnrs)
            {
                this.logger.Info(NLog.LogLevel.Info);
                this.pnrCounter++;
                var processor = Context.Child(pnr);
                if (processor == ActorRefs.Nobody)
                {
                    processor = Context.ActorOf(Props.Create(() => new Processor(pnr)), pnr);
                }

                processor.Tell(new Pnr(pnr));
            }
        }

        private void Completed(CompletedResponse response)
        {
            this.pnrCounter--;
            Console.WriteLine($"{pnrCounter}");
            if (this.pnrCounter == 0)
            {
                Console.WriteLine($"Completed");
                Program.Tasker.Tell(new Initiate());
            }
        }
    }
}