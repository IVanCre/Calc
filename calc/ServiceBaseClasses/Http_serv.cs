using System;
using System.Net;
using System.IO;
using System.Text;

using System.Collections.Generic;
using System.Text.Json;


namespace calc
{
    public enum TypeOfMessager:byte
    {
        unknown,
        Scada,
        Browser,
        Form
    }
    public interface IHttpListener
    {
        string[] GetAllListenAddress();
        void CloseApp();
        void Listen();
    }


    public class Http_serv:IHttpListener
    {
        AliveSender sender;
        

        bool tryListen = true;//флаг отключения прослушки
        SessionHolder sessionHolder;
        ILogger logger;
        HttpListener listener;
        List<string> listenIpAddress = new List<string>();
        RegisterAppSettings appSettings;

        public Http_serv(ILogger log, SessionHolder sessions, RegisterAppSettings appSet)
        {
            logger = log;
            sessionHolder = sessions;
            appSettings = appSet;

            listenIpAddress.Add(appSettings.serviceListenUrl);
        }

        public string[] GetAllListenAddress()
        {
            return listenIpAddress.ToArray();
        }

        public void CloseApp()
        {
            tryListen = false;

            if (listener != null)
            {
                listener.Abort();
            }
        }

        public void Listen()//асинхронная прослушка порта на наличие http-запросов
        {
            try
            {
                listener = new HttpListener();
                foreach (string url in listenIpAddress)//прослушиваем сетевые адреса, которые принадлежат текущей машине(были указаны в настройках)
                {
                    if (!listener.Prefixes.Contains(url))
                    {
                        listener.Prefixes.Add(url);
                    }
                }
                listener.Start();

                sender = new AliveSender(appSettings, logger);
                sender.StartSendingAmAlive();

                while (tryListen)
                {
                    IAsyncResult result = listener.BeginGetContext(new AsyncCallback(ListenerCallback), listener);
                    result.AsyncWaitHandle.WaitOne();//обязательно ждем, чтобы было подключение, иначе срывается в переполнение!
                }
            }
            catch (Exception e)
            {
                Console.WriteLine( e.Message);
                Environment.Exit(1);
            }
        }



        byte[] ParseToResponse(Session session, string inputData, Encoding encoding)
        {
            string responseString = "";
            RequestForParse requestSettings = JsonSerializer.Deserialize<RequestForParse>(inputData);

            if (requestSettings.type == "command")
            {
 Console.WriteLine("get command getCalcSets");
                if (requestSettings.dataForParse == "getCalcSets")
                {
                    responseString = OperatorDetectorsHolder.GetAllDetectorsInfo();
Console.WriteLine(responseString);
                }
            }
            else
            {
                responseString = session.parserForClient.Parse(requestSettings, logger);
            }
            return  encoding.GetBytes(responseString);
        }



        public void  ListenerCallback(IAsyncResult result)
        {
            try
            {
                HttpListener listener = (HttpListener)result.AsyncState;
                HttpListenerContext context = listener.EndGetContext(result);
                HttpListenerRequest request = context.Request;

                //расшифровка потока данных из запроса
                Stream body = request.InputStream;
                string dataRequest = "";
                using (StreamReader reader = new StreamReader(body, request.ContentEncoding))
                {
                    dataRequest = reader.ReadToEnd();
                    body.Close();
                }

                string baseID = "";
                string[] val = request.Headers.GetValues("client_ID");
                if (val.Length > 0)
                    baseID = val[0];


                Session session = sessionHolder.GetSessionResource(baseID);
                Response(context, dataRequest, session);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        void Response(HttpListenerContext context,string dataRequest, Session session)
        {
            byte[] buffer = null;
            HttpListenerResponse response = context.Response;

            switch (context.Request.HttpMethod)
            {
                case "POST"://запрос от страницы 
                    {
                        if (dataRequest != "{}" || dataRequest != "")
                            buffer = ParseToResponse(session, dataRequest, context.Request.ContentEncoding);
                        else
                            buffer = Encoding.ASCII.GetBytes("Request not corrected! Body is empty");
                        break;
                    }
                default:
                    {
                        string resp = $"This http-method not supported!";
                        buffer = Encoding.ASCII.GetBytes(resp);
                        break;
                    }
            }

            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();
        }
    }
}
