using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningRobot
{
    public class CleaningInfo
    {
        public RobotPos CurrentPos { get; set; }
        public RobotPos Start { get; set; }
        public int Battery { get; set; }
        public IList<Command> Commands { get; set; }
        public IList<IList<CellStatus>> Map { get; set; }      //first index is Y-coord, second X coord
    }

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

    public class RobotPos
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Facing Facing { get; set; }

        public RobotPos()
        { }

        public RobotPos(RobotPos pos)
        {
            if (pos != null)
            {
                this.X = pos.X;
                this.Y = pos.Y;
                this.Facing = pos.Facing;
            }
        }
    }
}
