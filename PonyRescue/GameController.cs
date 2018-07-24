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
        private IMazeMap mazeMap;
        private IMazeMapFactory mazeMapFactory;
        private IPonyChallengeFacade ponyChallangeFacade;
        private IPonyNavigator ponyNavigator;
        private string mazeId = "";

        public event EventHandler MoveMadeEvent;

        public GameController(IPonyChallengeFacade ponyChallangeFacade, IMazeMapFactory mazeMapfactory, IPonyNavigator ponyNavigator)
        {
            this.ponyChallangeFacade = ponyChallangeFacade;
            this.mazeMapFactory = mazeMapfactory;
            this.ponyNavigator = ponyNavigator;
        }

        public async Task<string> StartNewGame(int boardWidth, int boardHeight, string ponyName, int difficulty)
        {
            this.mazeId = await ponyChallangeFacade.StartNewGameAsync(boardWidth, boardHeight, ponyName, difficulty);
            var mazeState = await ponyChallangeFacade.GetMazeStateAsync(mazeId);
            this.mazeMap = this.mazeMapFactory.Create(boardWidth, boardHeight, mazeState.PonyLocation, mazeState.ExitLocation, mazeState.data);
            var mazeSanpshot = await ponyChallangeFacade.PrintMazeStateAsync(this.mazeId);

            return mazeSanpshot;
        }

        public async Task<string> RescuePony(int stepDelay)
        {
            MazeState mazeState;
            do
            {
                mazeState = await ponyChallangeFacade.GetMazeStateAsync(mazeId);
                Direction move = this.ponyNavigator.GetNextMove(mazeMap, mazeState);

                await ponyChallangeFacade.MovePonyAsync(this.mazeId, move);
                var mazeSnapshot = await ponyChallangeFacade.PrintMazeStateAsync(this.mazeId);
                MoveMadeEvent?.Invoke(this, new GameEventArgs() {snapshot = mazeSnapshot});

                await Task.Delay(stepDelay);
            } while (mazeMap.CurrentPathToExit.Count > 0);

            mazeState = await ponyChallangeFacade.GetMazeStateAsync(mazeId);
            return mazeState.gameState.GameResult;
        }
    }
}
