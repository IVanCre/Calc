using System.Text.Json;

namespace calc
{
    class OperatorDetectorsHolder
    {
        static  OperatorDetector[] allKnownDetectors = {new StandartDetector(),
                                                        new RomanDetector()};

        public static string GetAllDetectorsInfo()
        {
            CalcSetInfo[] allSets = new CalcSetInfo[allKnownDetectors.Length];
            for (int i = 0; i < allSets.Length; i++)
                allSets[i] = allKnownDetectors[i].GetDetctorInfo();


            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;

            return  JsonSerializer.Serialize<CalcSetInfo[]>(allSets, options);
        }
    }
}
