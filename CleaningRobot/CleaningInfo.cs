namespace CleaningRobot
{
    public class CleaningInfo
    {
        public RobotPosition Start { get; set; } = new RobotPosition();
        public int Battery { get; set; }
        public List<Command> Commands { get; set; } = new List<Command>();
        public List<List<CellStatus>> Map { get; set; } = new List<List<CellStatus>>();          //first index is Y-coord, second X coord
    }
}
