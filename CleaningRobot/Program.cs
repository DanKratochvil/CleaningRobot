using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CleaningRobot
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length!=2)
            {
                Console.WriteLine("Usage>");
                Console.WriteLine("CleaningRobot <source.json> <result.json>");
                return;
            }

            //desewrialise input
            ILog log = LogManager.GetLogger(typeof(Program));
            string jsonString = File.ReadAllText(args[0]);
            jsonString=jsonString.Replace("null", "W");     //wall is W in map
            var cleaningInfo = JsonConvert.DeserializeObject<CleaningInfo>(jsonString);

            //clean
            var commands = new RobotCommands(cleaningInfo, log);
            var cleaningResult = commands.CleanRoom();

            //serialise output
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            string json = JsonConvert.SerializeObject(cleaningResult, Formatting.Indented, serializerSettings);
            File.WriteAllText(args[1], json);
        }
    }
}
