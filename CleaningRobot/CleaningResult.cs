namespace CleaningRobot
{
    public class CleaningResult
    {
        public List<Cell> Visited { get; set; }
        public List<Cell> Cleaned { get; set; }
        public RobotPosition Final { get; set; }
        public int Battery { get; set; }

        private HashSet<string> visitedPositions;
        private HashSet<string> cleanedPositions;

        public CleaningResult()
        {
            Visited = new List<Cell>();
            Cleaned = new List<Cell>();
            Final = new RobotPosition();
            visitedPositions = new HashSet<string>();
            cleanedPositions = new HashSet<string>();
        }

        public bool AddVisited(Cell cell)
        {
            string key = $"{cell.X},{cell.Y}";
            if (visitedPositions.Add(key))
            {
                Visited.Add(cell);
                return true;
            }
            return false;
        }

        public bool AddCleaned(Cell cell)
        {
            string key = $"{cell.X},{cell.Y}";
            if (cleanedPositions.Add(key))
            {
                Cleaned.Add(cell);
                return true;
            }
            return false;
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
