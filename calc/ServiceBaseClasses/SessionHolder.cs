using System;
using System.Collections.Generic;
using System.Timers;

namespace calc
{
    public class Session
    {
        public string clientID;
        public OperationParser parserForClient;
        public DateTime timeEndOfSession;
        

        public Session(string ID)
        {
            clientID = ID;
            parserForClient = new OperationParser();
            timeEndOfSession =DateTime.Now + TimeSpan.FromTicks(TimeSpan.TicksPerMinute * SessionHolder.resetTimerInterval);//запоминаем конечную дату
        }
    }

    public class SessionHolder// каждому новому IP выделяет свой экземпляр парсера
    {
        object locker = new object();
        public static int resetTimerInterval = 10;//в минутах
        Timer sessionCleaner;

        List<Session> sessions = new List<Session>();
        ILogger logger;

        public SessionHolder(ILogger log)
        {
            logger = log;
            InicializeCleaner();
        }

        public Session GetSessionResource(string clientID)
        {
            lock (locker)
            {
                Session session = null;
                try
                {
                    int index = sessions.FindIndex(x => x.clientID == clientID);

                    if (index > -1)
                    {
Console.WriteLine("\nupdate session");
                        sessions[index].timeEndOfSession = DateTime.Now + TimeSpan.FromTicks(TimeSpan.TicksPerMinute * resetTimerInterval);
                        session = sessions[index];
                    }
                    else
                    {
Console.WriteLine("\ncreate new session");
                        session = new Session(clientID);
                        sessions.Add(session);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                return session;
            }
        }

        void InicializeCleaner()
        {
            sessionCleaner = new Timer();
            sessionCleaner.AutoReset = true;
            sessionCleaner.Interval = resetTimerInterval* 60_000;
            sessionCleaner.Elapsed+= new ElapsedEventHandler((object sender ,ElapsedEventArgs e)=>
            {
                try
                {
                    sessions.RemoveAll(x => x.timeEndOfSession <= DateTime.Now);
                }
                catch(Exception ex)
                { Console.WriteLine(ex.Message); }
            });

            sessionCleaner.Start();
        }
    }
}
