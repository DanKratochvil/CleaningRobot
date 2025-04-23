namespace CleaningRobot
{
    public class CleaningResult
    {
        public Stack<Cell> Visited { get; set; }
        public Stack<Cell> Cleaned { get; set; }
        public RobotPosition Final { get; set; }
        public int Battery { get; set; }

        public CleaningResult()
        {
            Visited = new Stack<Cell>();
            Cleaned = new Stack<Cell>();
            Final = new RobotPosition();
        }
    }

    public class Cell
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Cell(RobotPosition robotPos)
        {
            this.X = robotPos.X;
            this.Y = robotPos.Y;
        }
    }
}
