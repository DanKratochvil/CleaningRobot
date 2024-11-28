using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningRobot
{
    public class RobotCommands
    {
        private RobotPos robotPos;
        private int battery;
        private Queue<Command> commands;
        private IList<IList<CellStatus>> map;            //first index is Y-coord, second X coord    
        private CleaningResult cleaningResult;
        private List<List<Command>> backOffStrategy;
        private ILog log;

        public RobotCommands(CleaningInfo cleaningInfo, ILog log)
        {
            this.robotPos = cleaningInfo.Start;
            this.battery = cleaningInfo.Battery;
            this.commands = new Queue<Command>(cleaningInfo.Commands);
            this.map = cleaningInfo.Map;
            this.cleaningResult = new CleaningResult();
            this.cleaningResult.Visited.Push(new Cell(robotPos));
            this.backOffStrategy = InitBackOffStrategy();
            this.log = log;
        }

        public CleaningResult CleanRoom()
        {
            log.Info($"Robot position:X:{robotPos.X},Y:{robotPos.Y},Facing:{robotPos.Facing}");

            while (commands.Count > 0)
            {
                Command command = commands.Dequeue();
                RobotPos nextPos = ExecuteCommand(command);
                if (nextPos != null && ObstacleHit(nextPos))     //nextPos !=null only in case of Advance or Back Command when robot changes cell and wall can be hit
                {
                    if (!BackOffStrategy())
                        break;
                }
                else
                {
                    if (nextPos != null)
                    {
                        robotPos = new RobotPos(nextPos);
                        Cell cell = new Cell(robotPos);
                        if (cleaningResult.Visited.All(c => c.X != cell.X || c.Y != cell.Y))   //when cell was visited more times, it appears only once in result 
                            cleaningResult.Visited.Push(cell);
                    }
                    
                    if (command== Command.C)
                        log.Info($"Robot cleaned cell at:X:{robotPos.X},Y:{robotPos.Y},Facing:{robotPos.Facing}");
                    else
                        log.Info($"Robot position:X:{robotPos.X},Y:{robotPos.Y},Facing:{robotPos.Facing}");
                }
            }

            cleaningResult.Final = robotPos;
            cleaningResult.Battery = battery;

            return cleaningResult;
        }

        public RobotPos ExecuteCommand(Command cmd)
        {
            RobotPos nextPos = null; ;
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
                    Clean();
                    break;
            }
            return nextPos;
        }

        public bool BackOffStrategy()
        {
            bool seqStepSucc = true;

            for (int backOffStrategyStepNo = 0; backOffStrategyStepNo < backOffStrategy.Count; backOffStrategyStepNo++)
            {
                var backOffStrategyStep = backOffStrategy[backOffStrategyStepNo];
                seqStepSucc = true;

                for (int i = 0; i < backOffStrategyStep.Count; i++)
                {
                    Command cmd = backOffStrategyStep[i];
                    RobotPos nextPos = ExecuteCommand(cmd);
                    if (nextPos != null)
                    {
                        if (ObstacleHit(nextPos))     //nextPos !=null only in case of Advance or Back Command when wall can be hit
                        {
                            seqStepSucc = false;
                            break;  //next backOffStrategyStep
                        }
                        else
                        {
                            robotPos = new RobotPos(nextPos);
                            Cell cell = new Cell(robotPos);
                            if (cleaningResult.Visited.All(c=>c.X != cell.X || c.Y != cell.Y))  
                                cleaningResult.Visited.Push(cell);
                        }
                    }
                    log.Info($"Robot BackOffStrategy Step{backOffStrategyStepNo} Cmd{i} position:X:{robotPos.X},Y:{robotPos.Y},Facing:{robotPos.Facing}");
                }

                //if one of the stepps e.g. [TR,A,TL] succeeds, the rest stepps are dropped  
                if (seqStepSucc)
                    break;
            }

            return seqStepSucc;  //true if robot succseeds completing one of BackOffStrategy steps without hitting the obstacle
        }
      

        public void TurnLeft()
        {
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
            battery--;
        }

        public void TurnRight()
        {
            if (battery < 1)
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
            battery--;
        }

        public RobotPos Advance()
        {
            RobotPos nextPos = new RobotPos(robotPos);
            
            if (battery < 2)
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

            battery = battery - 2;
            return nextPos;
        }

        public RobotPos Back()
        {
            if (battery < 2)
                return null;

            RobotPos nextPos = new RobotPos(robotPos);
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

            battery = battery - 2;
            return nextPos;
        }

        public bool ObstacleHit(RobotPos nextPos)
        {
            if (nextPos.X < 0 || nextPos.Y < 0 || nextPos.Y >= map.Count() || nextPos.X >= map[0].Count() || map[nextPos.Y][nextPos.X] == CellStatus.C || map[nextPos.Y][nextPos.X] == CellStatus.W)
            {
                log.Info($"Robot hit wall at:X:{robotPos.X},Y:{robotPos.Y},Facing:{robotPos.Facing}");
                return true;
            }
            else
                return false;
        }


        public List<List<Command>> InitBackOffStrategy()
        {            
            return new List<List<Command>>()
            {
                new List<Command>(){Command.TR, Command.A, Command.TL},
                new List<Command>(){Command.TR, Command.A, Command.TR},
                new List<Command>(){Command.TR, Command.A, Command.TR},
                new List<Command>(){Command.TR, Command.B, Command.TR, Command.A },
                new List<Command>(){Command.TL, Command.TL, Command.A}
            };
        }


        public void Clean()
        {
            if (battery >= 5)     //map[robotPos.Y][robotPos.X] == CellStatus.S  -check if cell was cleaned before
            {
                battery = battery - 5;
                map[robotPos.Y][robotPos.X] = CellStatus.D;
                Cell cell = new Cell(robotPos);

                if (cleaningResult.Cleaned.All(c => c.X != cell.X || c.Y != cell.Y))
                    cleaningResult.Cleaned.Push(new Cell(robotPos));
            }
        }
    }
}
