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
                if (!File.Exists(inputFileName))
                    throw new FileNotFoundException($"Input file not found: {inputFileName}");

                string cleaningInfoContent = File.ReadAllText(inputFileName);
                cleaningInfoContent = cleaningInfoContent.Replace("null", "W");     //wall is W in map
                var cleaningInfo = JsonConvert.DeserializeObject<CleaningInfo>(cleaningInfoContent) ?? throw new InvalidOperationException("Failed to deserialize CleaningInfo - file content may be invalid");
                
                // Validate input data
                ValidateCleaningInfo(cleaningInfo);
                
                return cleaningInfo;
            }
            catch (JsonException ex)
            {
                throw new ArgumentException($"Invalid JSON format in input file: {ex.Message}", ex);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void ValidateCleaningInfo(CleaningInfo cleaningInfo)
        {
            if (cleaningInfo.Map == null || cleaningInfo.Map.Count == 0)
                throw new ArgumentException("Map cannot be empty");

            if (cleaningInfo.Start.X < 0 || cleaningInfo.Start.Y < 0)
                throw new ArgumentException("Start position cannot have negative coordinates");

            if (cleaningInfo.Start.Y >= cleaningInfo.Map.Count || 
                cleaningInfo.Start.X >= cleaningInfo.Map[0].Count)
                throw new ArgumentException("Start position is outside the map bounds");

            if (cleaningInfo.Battery < 0)
                throw new ArgumentException("Battery cannot be negative");

            // Check if all map rows have the same length
            int expectedWidth = cleaningInfo.Map[0].Count;
            foreach (var row in cleaningInfo.Map)
            {
                if (row.Count != expectedWidth)
                    throw new ArgumentException("All map rows must have the same length");
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
