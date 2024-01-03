using System;
using System.Xml;

namespace calc
{
    [Serializable]
    public struct RegisterAppSettings//базовые настройки для коннекта базы и этого сервиса
    {
        public string CoreAppUrl { get; set; }
        public string serviceName { get; set; }
        public string serviceListenUrl { get; set; }
        public string settings { get; set; }

        public bool LoadSettingsFromFile(ILogger logger)
        {
            try
            {
                using (XmlReader xReader = XmlReader.Create(Environment.CurrentDirectory + "\\Files\\AppSettings.xml"))
                {
                    while (xReader.Read())
                    {
                        if (xReader.NodeType == XmlNodeType.Element && xReader.Name == "SettingsDoc")
                        {
                            xReader.ReadToFollowing("ListenIP");
                             serviceListenUrl= $"http://{xReader.GetAttribute("host")}:{xReader.GetAttribute("port")}/";

                            xReader.ReadToFollowing("CoreAppIP");
                            CoreAppUrl = $"http://{xReader.GetAttribute("host")}:{xReader.GetAttribute("port")}/";

                            xReader.ReadToFollowing("ThisAppName");
                            serviceName = xReader.GetAttribute("name");
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                logger.Save($"Ошибка загрузки файла настроек http-слушателя!\n {e.Message}");
                return false;
            }
        }
    }
}
