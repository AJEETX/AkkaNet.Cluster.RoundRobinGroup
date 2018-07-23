namespace Backend.Actor
{
    using System;
    using System.Threading;
    using Akka.Actor;
    using Message;

    internal class Supervisor : ReceiveActor, ILogReceive
    {
        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly string name = string.Empty;

        public Supervisor(string name)
        {
            this.name = name;
            this.Receive<Office>(o => Do(o));
        }

        private void Do(Office office)
        {
            Console.WriteLine($"{Context.Self.Path} - {office.ID} : {office.Data}");
            this.logger.Info(NLog.LogLevel.Info);
            var data = new OfficePnrList(office, Data.Pnrs);
            Thread.Sleep(2000);
            this.Sender.Tell(data);
        }
    }
}