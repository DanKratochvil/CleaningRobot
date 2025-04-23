using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CleaningRobot
{
    public class CleaningSerializer
    {
        public static CleaningInfo DeserializeInput(string inputFileName)
        {
            try
            {
                string cleaningInfoContent = File.ReadAllText(inputFileName);
                cleaningInfoContent = cleaningInfoContent.Replace("null", "W");     //wall is W in map
                var cleaningInfo = JsonConvert.DeserializeObject<CleaningInfo>(cleaningInfoContent) ?? throw new Exception("Failed to read CleaningInfo");
                return cleaningInfo;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static  void SerializeOutput(CleaningResult cleaningResult, string outputFileName)
        {
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            try
            {
                string json = JsonConvert.SerializeObject(cleaningResult, Formatting.Indented, serializerSettings);
                string? outputDirName = Path.GetDirectoryName(outputFileName);
                if (outputDirName != null && !Directory.Exists(outputDirName))
                    Directory.CreateDirectory(outputDirName);
                File.WriteAllText(outputFileName, json);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
