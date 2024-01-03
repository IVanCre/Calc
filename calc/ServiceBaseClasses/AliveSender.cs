using System;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace calc
{
    class AliveSender//отправляет центральному приложению сообщению, что Это приложение начало работать
    {
        int interval = 10_000;//в секундах
        RegisterAppSettings appSet;
        ILogger logger;
        HttpClient client = new HttpClient();

        public AliveSender(RegisterAppSettings set, ILogger log)
        {
            logger = log;
            appSet = set;
        }

        public void StartSendingAmAlive()
        {
            Task.Factory.StartNew(delegate
            {
                /*данный энкодер будет пропускать КАК-ЕСТЬ символы,которые могут быть зарезервированы для html
                https://learn.microsoft.com/en-us/dotnet/api/system.text.encodings.web.javascriptencoder.unsaferelaxedjsonescaping?view=netcore-3.0 */

                JsonSerializerOptions options = new JsonSerializerOptions();
                options.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
                string JsonConvert = JsonSerializer.Serialize(appSet, options);
                
                CancellationTokenSource tokenSource = new CancellationTokenSource();

                for (; ; )
                {
                    try
                    {
                        HttpContent content = new StringContent(JsonConvert, Encoding.UTF8, "text/json");
Console.WriteLine($"\n{DateTime.Now + appSet.CoreAppUrl} send Alive");

                        var T = Task.Run(() => client.PostAsync(appSet.CoreAppUrl + "service", content,tokenSource.Token));
                        T.Wait();
                        var response = T.Result;
                        string rezult = T.Result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
Console.WriteLine($"response from serv ={rezult}\n");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }

                    tokenSource = new CancellationTokenSource();//l
                    Thread.Sleep(interval);
                }
            });
        }
    
    }
}



