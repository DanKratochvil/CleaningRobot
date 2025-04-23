namespace CleaningRobot
{
    public class RobotPosition
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Facing Facing { get; set; }

        public RobotPosition()
        { }

        public RobotPosition(RobotPosition pos)
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
