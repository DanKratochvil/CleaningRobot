using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningRobot
{
    public class CleaningResult
    {
        public Stack<Cell> Visited { get; set; }
        public Stack<Cell> Cleaned { get; set; }
        public RobotPos Final { get; set; }
        public int Battery { get; set; }

        public CleaningResult()
        {
            Visited = new Stack<Cell>();
            Cleaned = new Stack<Cell>();
        }
    }

    public class Cell
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Cell (RobotPos robotPos)
        {
            this.X = robotPos.X;
            this.Y = robotPos.Y;
        }
    }
}
