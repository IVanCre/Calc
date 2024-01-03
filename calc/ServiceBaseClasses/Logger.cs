using System;
using System.IO;
using System.Threading.Tasks;

namespace calc
{
    public interface ILogger
    {
        void Save(string text);
        string GetLoggerInfo();
    }

    class Logger: ILogger
    {
        public static string pathToSave = Environment.CurrentDirectory+(@"\Files\Logs");
        object locker_1 = new object();
        object locker_3 = new object();
        static int storagePeriod = 30;
        DateTime lastDateToClean;

        public Logger()
        {
            CheckFolder();
        }
        void CheckFolder()
        {
            if (!Directory.Exists(pathToSave))
                Directory.CreateDirectory(pathToSave);
        }

        public string GetLoggerInfo()
        {
            return $"[storage_Period_InDays={storagePeriod}]\n" +
                   $"[last_Date_To_Clean={lastDateToClean.ToString("G")}]";
        }

        public void Save(string text)
        {
            lock (locker_1)
            {
                if((DateTime.Now - lastDateToClean).Ticks >= TimeSpan.TicksPerDay)
                    ClearLogs(pathToSave);

                string dateNow = DateTime.Now.ToShortDateString();
                string time = DateTime.Now.ToString();

                File.AppendAllText(pathToSave + $"\\{dateNow}.txt", time + " " + text + "\n");

                lastDateToClean = DateTime.Now;
            }
        }

        public  void ClearLogs(string LogFolder)// если запись в файл была более чем Х дней назад, то файл удаляется
        {
            lock (locker_3)
            {
                Task.Factory.StartNew(() =>
                {
                    if (Directory.Exists(LogFolder))
                    {
                        TimeSpan interval;
                        string[] fileNames = Directory.GetFiles(LogFolder);
                        {
                            foreach (string fileName in fileNames)
                            {
                                interval = DateTime.Now - File.GetLastWriteTime(fileName);
                                if (interval.Days >= storagePeriod)
                                    File.Delete(fileName);
                            }
                        }
                    }
                });
            }
        }
    }
}
