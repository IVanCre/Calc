using System;


namespace calc
{


    class calc
    {
        static void Main()
        {
            ILogger logger = new Logger();

            RegisterAppSettings appSet = new RegisterAppSettings();
            appSet.settings = OperatorDetectorsHolder.GetAllDetectorsInfo();//настройки всех доступных наборов calc
            bool isSetLoading = appSet.LoadSettingsFromFile(logger);//доп.настрйоки текущего сервиса


            if (isSetLoading)
            {
                SessionHolder sessions = new SessionHolder(logger);

                Http_serv listener = new Http_serv(logger, sessions, appSet);
                listener.Listen();
            }


            //OperationParser calcParser = new OperationParser();

            //for (; ; )
            //{
            //    Console.WriteLine("\nВыберите тип системы счета:\n" +
            //                  "0 - стандартная\n" +
            //                  "1 - римская");
            //    NumericSystem type = (NumericSystem)Convert.ToByte(Console.ReadLine());

            //    Console.WriteLine("\nВам нужен подробный отчет о ходе работы? (0/1):");
            //    bool useExplains = Convert.ToBoolean(Convert.ToInt32(Console.ReadLine()));

            //    calcParser.Set(type, useExplains);


            //    Console.WriteLine($"\nДоступные операции в выбранной системе({type}) счета( оператор -> расшифровка):");
            //    foreach (char  key in calcParser.operatorDetector.AllOperations.Keys)
            //    {
            //        Console.WriteLine($"{key} ->  {calcParser.operatorDetector.AllOperations[key].name}");
            //    }




            //    Console.WriteLine("\nВведите выражение:");
            //    string str = Console.ReadLine();
            //    calcParser.Parse(str);
            //}







            //if (useExplains)
            //{

            //    Console.WriteLine("\n1.парсим строку для нахождения индексов скобок.\n" +
            //                      "1.1 Присваиваем какждой скобке приоритет вложенности и сохраняем в массив индекс_приоритет.влож\n" +
            //                      "2.парсим строку, находим индексы, под которыми идут операторы вычислений\n" +
            //                      "2.1 сопоставляем индексы операторов и индексы скобок, чтобы выяснить, с каким приоритетом вложенности будет идти текущий оператор\n" +
            //                      "2.2 для каждого найденного оператора вычисляем операцию:" +
            //                      "    - находим границы этой операции в исходной строке\n" +
            //                      "    - проверяем, затрагивает ли эта операции операции, которые уже были вычислены ранее(с более высоким приоритетом)\n" +
            //                      "    - вычленяем из операции левое и правое значение(если надо -тянем из буфера уже вычисленных операций)\n" +
            //                      "    - добавляем текущую операцию в буфер уже вычисленных операций.\n" +
            //                      "      Все операции, которые участвовали в это операции 'сливаются' c текущей операцией при сохранении в буфер\n" +
            //                      "3.когда все операции вычислены, в буфере вычисленных операций остается только одна выполненная операция, которая поглотила все остальные\n" +
            //                      "  результат этой операции - есть итоговый ответ\n\n");
            //}




            //варианты некорректной входной строки:
            //  строка начинается с оператора
            //  строка заканчивается оператором
            //  строка начинается с закрытой скобки
            //  строка завершается открытой скобкой
            //  два оператора стоят рядом
            //  открытая и закрытая скобки стоят рядом ()
        }

    }
}
