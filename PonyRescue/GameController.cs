using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PonyRescue
{
    class GameController : IGameController
    {
        private IPathFinder pathFinder;
        private IPonyChallengeFacade ponyChallangeFacade;
        private string mazeId = "";

        public event EventHandler MoveMadeEvent;

        public GameController()
        {
            ponyChallangeFacade = new PonyChallengeFacade();
        }

        public async Task<string> StartNewGame(int boardWidth, int boardHeight, string ponyName, int difficulty)
        {
            this.mazeId = await ponyChallangeFacade.StartNewGameAsync(boardWidth, boardHeight, ponyName, difficulty);
            var mazeState = await ponyChallangeFacade.GetMazeStateAsync(mazeId);

            pathFinder = new PathFinder(boardWidth, boardHeight, mazeState.PonyLocation, mazeState.ExitLocation);
            pathFinder.InitializeChambers(mazeState.data);

            var mazeSanpshot = await ponyChallangeFacade.PrintMazeStateAsync(this.mazeId);

            return mazeSanpshot;
        }

        public async Task RescuePony(int stepDelay)
        {
            List<string> path = await pathFinder.FindShortestPathAsync();
            //PonyNavigator.GetNextMove(path) ...?
            foreach (var move in path)
            {
                if (Enum.TryParse(move, true, out Direction direction))
                {
                    await ponyChallangeFacade.MovePonyAsync(this.mazeId, direction);
                    var mazeSnapshot = await ponyChallangeFacade.PrintMazeStateAsync(this.mazeId);
                    MoveMadeEvent?.Invoke(this, new GameEventArgs() {snapshot =  mazeSnapshot});
                }
                    
                await Task.Delay(stepDelay);
                
            }
        }
    }
}
