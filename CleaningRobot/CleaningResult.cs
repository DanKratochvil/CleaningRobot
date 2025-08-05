namespace CleaningRobot
{
    public class CleaningResult
    {
        public List<Cell> Visited { get; set; }
        public List<Cell> Cleaned { get; set; }
        public RobotPosition Final { get; set; }
        public int Battery { get; set; }

        private HashSet<Cell> visitedPositions;
        private HashSet<Cell> cleanedPositions;

        public CleaningResult()
        {
            Visited = new List<Cell>();
            Cleaned = new List<Cell>();
            Final = new RobotPosition();
            visitedPositions = new HashSet<Cell>();
            cleanedPositions = new HashSet<Cell>();
        }

        public bool AddVisited(Cell cell)
        {
            if (visitedPositions.Add(cell))
            {
                Visited.Add(cell);
                return true;
            }
            return false;
        }

        public bool AddCleaned(Cell cell)
        {
            if (cleanedPositions.Add(cell))
            {
                Cleaned.Add(cell);
                return true;
            }
            return false;
        }
    }

    public class Cell : IEquatable<Cell>
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Cell(RobotPosition robotPos)
        {
            this.X = robotPos.X;
            this.Y = robotPos.Y;
        }

        public bool Equals(Cell? other)
        {
            if (other == null) return false;
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Cell);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
    }
}
