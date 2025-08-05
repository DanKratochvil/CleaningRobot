namespace CleaningRobot
{
    public class RobotCommand
    {
        private const int BATTERY_COST_TURN = 1;
        private const int BATTERY_COST_MOVE = 2;
        private const int BATTERY_COST_CLEAN = 5;

        private RobotPosition robotPos;
        private int battery;
        private Command cmd;
        private CleaningResult cleaningResult;
        private List<List<CellStatus>> map;

        public RobotCommand(RobotPosition robotPos, Command cmd, CleaningResult cleaningResult, List<List<CellStatus>> map, int battery)
        {
            this.robotPos = robotPos;
            this.cmd = cmd;
            this.battery = battery;  
            this.cleaningResult = cleaningResult;
            this.map = map;
        }

        public (RobotPosition?, int) ExecuteCommand()
        {
            RobotPosition? nextPos = null; 
            switch (cmd)
            {
                case Command.A:
                    nextPos = Advance();
                    break;

                case Command.B:
                    nextPos = Back();
                    break;

                case Command.TL:
                    TurnLeft();
                    break;

                case Command.TR:
                    TurnRight();
                    break;

                case Command.C:
                    CleanCell();
                    break;
            }
            return (nextPos, battery);
        }

        private void TurnLeft()
        {
            if (battery < BATTERY_COST_TURN)
                return;

            switch (robotPos.Facing)
            {
                case Facing.N:
                    robotPos.Facing = Facing.W;
                    break;
                case Facing.W:
                    robotPos.Facing = Facing.S;
                    break;
                case Facing.S:
                    robotPos.Facing = Facing.E;
                    break;
                case Facing.E:
                    robotPos.Facing = Facing.N;
                    break;
            }
            battery -= BATTERY_COST_TURN;
        }

        private void TurnRight()
        {
            if (battery < BATTERY_COST_TURN)
                return;

            switch (robotPos.Facing)
            {
                case Facing.N:
                    robotPos.Facing = Facing.E;
                    break;
                case Facing.E:
                    robotPos.Facing = Facing.S;
                    break;
                case Facing.S:
                    robotPos.Facing = Facing.W;
                    break;
                case Facing.W:
                    robotPos.Facing = Facing.N;
                    break;
            }
            battery -= BATTERY_COST_TURN;
        }

        private RobotPosition? Advance()
        {
            RobotPosition nextPos = new RobotPosition(robotPos);
            
            if (battery < BATTERY_COST_MOVE)
                return null;

            switch (robotPos.Facing)
            {
                case Facing.N:
                    nextPos.Y = robotPos.Y - 1;
                    break;
                case Facing.W:
                    nextPos.X = robotPos.X - 1;
                    break;
                case Facing.S:
                    nextPos.Y = robotPos.Y + 1;
                    break;
                case Facing.E:
                    nextPos.X = robotPos.X + 1;
                    break;
            }

            battery -= BATTERY_COST_MOVE;
            return nextPos;
        }

        private RobotPosition? Back()
        {
            if (battery < BATTERY_COST_MOVE)
                return null;

            RobotPosition nextPos = new RobotPosition(robotPos);
            switch (robotPos.Facing)
            {
                case Facing.N:
                    nextPos.Y = robotPos.Y + 1;
                    break;
                case Facing.W:
                    nextPos.X = robotPos.X + 1;
                    break;
                case Facing.S:
                    nextPos.Y = robotPos.Y - 1;
                    break;
                case Facing.E:
                    nextPos.X = robotPos.X - 1;
                    break;
            }

            battery -= BATTERY_COST_MOVE;
            return nextPos;
        }

        private void CleanCell()
        {
            if (battery >= BATTERY_COST_CLEAN)     //map[robotPos.Y][robotPos.X] == CellStatus.S  -check if cell was cleaned before
            {
                battery -= BATTERY_COST_CLEAN;
                map[robotPos.Y][robotPos.X] = CellStatus.D;
                Cell cell = new Cell(robotPos);
                cleaningResult.AddCleaned(cell);
            }
        }
    }
}
