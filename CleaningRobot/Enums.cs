using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CleaningRobot
{
    public enum CellStatus
    {
        S,    //cell should be cleaned
        D,    //done, cell was cleaned 
        C,    //column
        W,    //wall
    }

    public enum Command
    {
        TL,
        TR,
        A,
        B,
        C
    }


    [JsonConverter(typeof(StringEnumConverter))]
    public enum Facing
    {
        N,
        E,
        S,
        W
    }

}
