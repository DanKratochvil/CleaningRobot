using Microsoft.Extensions.Logging;

namespace CleaningRobot
{
    public class CleaningService
    {
        private RobotPosition robotPos;
        private int battery;
        private Queue<Command> commands;
        private List<List<CellStatus>> map;            //first index is Y-coord, second X coord    
        private CleaningResult cleaningResult;
        private List<List<Command>> backOffStrategy;
        private ILogger logger;

        public CleaningService(CleaningInfo cleaningInfo, ILogger logger)
        {
            this.robotPos = cleaningInfo.Start;
            this.battery = cleaningInfo.Battery;
            this.commands = new Queue<Command>(cleaningInfo.Commands);
            this.map = cleaningInfo.Map;
            this.cleaningResult = new CleaningResult();
            this.cleaningResult.AddVisited(new Cell(robotPos));
            this.backOffStrategy = InitBackOffStrategy();
            this.logger = logger;
        }

        /// <summary>
        /// cleans room, if obstacle hit then BackOffStrategy
        /// </summary>
        /// <returns>CleaningResult</returns>
        public CleaningResult CleanRoom()
        {            
            logger.LogInformation($"Starting cleaning at position X:{robotPos.X}, Y:{robotPos.Y}, Facing:{robotPos.Facing} with {battery} battery");

            while (commands.Count > 0)
            {
                Command command = commands.Dequeue();
                logger.LogDebug($"Executing command: {command}, Battery: {battery}, Commands remaining: {commands.Count}");
                
                var robotCommand = new RobotCommand(robotPos, command, cleaningResult, map, battery);
                (RobotPosition? nextPos, battery) = robotCommand.ExecuteCommand();
                if (nextPos != null && ObstacleHit(nextPos))     //nextPos !=null only in case of Advance or Back Command when robot changes cell and wall can be hit
                {
                    if (!BackOffStrategy())
                    {
                        logger.LogWarning("Back-off strategy failed. Stopping execution.");
                        break;
                    }
                }
                else
                {
                    if (nextPos != null)
                    {
                        robotPos = new RobotPosition(nextPos);
                        Cell cell = new Cell(robotPos);
                        cleaningResult.AddVisited(cell);
                    }

                    if (command == Command.C)
                        logger.LogInformation($"Robot cleaned cell at X:{robotPos.X}, Y:{robotPos.Y}, Facing:{robotPos.Facing}, Battery remaining: {battery}");
                    else
                        logger.LogInformation($"Robot position X:{robotPos.X}, Y:{robotPos.Y}, Facing:{robotPos.Facing}, Battery remaining: {battery}");
                }
            }

            cleaningResult.Final = robotPos;
            cleaningResult.Battery = battery;

            logger.LogInformation($"Cleaning completed. Final position X:{robotPos.X}, Y:{robotPos.Y}, Facing:{robotPos.Facing}");
            logger.LogInformation($"Visited {cleaningResult.Visited.Count} cells, cleaned {cleaningResult.Cleaned.Count} cells, battery remaining: {battery}");

            return cleaningResult;
        }

        private bool BackOffStrategy()
        {
            bool seqStepSucc = true;

            for (int backOffStrategyStepNo = 0; backOffStrategyStepNo < backOffStrategy.Count; backOffStrategyStepNo++)
            {
                var backOffStrategyStep = backOffStrategy[backOffStrategyStepNo];
                seqStepSucc = true;

                for (int i = 0; i < backOffStrategyStep.Count; i++)
                {
                    Command command = backOffStrategyStep[i];
                    var robotCommand = new RobotCommand(robotPos, command, cleaningResult, map, battery);
                    (RobotPosition? nextPos, battery) = robotCommand.ExecuteCommand();

                    if (nextPos != null)
                    {
                        if (ObstacleHit(nextPos))     //nextPos !=null only in case of Advance or Back Command when wall can be hit
                        {
                            seqStepSucc = false;                           
                            break;  //next backOffStrategyStep
                        }
                        else
                        {
                            robotPos = new RobotPosition(nextPos);
                            Cell cell = new Cell(robotPos);
                            cleaningResult.AddVisited(cell);
                        }
                    }
                    logger.LogInformation($"Robot BackOffStrategy Step{backOffStrategyStepNo}, Command{i}: {command}, Position X:{robotPos.X}, Y:{robotPos.Y}, Facing:{robotPos.Facing}, Battery: {battery}");
                }

                //if one of the stepps e.g. [TR,A,TL] succeeds, the rest stepps are dropped  
                if (seqStepSucc)
                    break;
            }

            return seqStepSucc;  //true if robot succseeds completing one of BackOffStrategy steps without hitting the obstacle
        }

        private bool ObstacleHit(RobotPosition nextPos)
        {
            if (nextPos.X < 0 || nextPos.Y < 0 || nextPos.Y >= map.Count() || nextPos.X >= map[0].Count() || map[nextPos.Y][nextPos.X] == CellStatus.C || map[nextPos.Y][nextPos.X] == CellStatus.W)
            {
                logger.LogInformation($"Robot hit obstacle at target X:{nextPos.X}, Y:{nextPos.Y} while at X:{robotPos.X}, Y:{robotPos.Y}, Facing:{robotPos.Facing}");
                return true;
            }
            else
                return false;
        }

        private List<List<Command>> InitBackOffStrategy()
        {
            return new List<List<Command>>()
            {
                new List<Command>(){Command.TR, Command.A, Command.TL},
                new List<Command>(){Command.TR, Command.A, Command.TR},
                new List<Command>(){Command.TL, Command.A, Command.TL},
                new List<Command>(){Command.TR, Command.B, Command.TR, Command.A },
                new List<Command>(){Command.TL, Command.TL, Command.A}
            };
        }
    }
}
