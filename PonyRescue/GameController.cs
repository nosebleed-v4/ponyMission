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
        private IPonyChallengeFacade ponyChallangeFacade;
        private IPonyNavigator ponyNavigator;
        private string mazeId = "";

        public event EventHandler MoveMadeEvent;

        public GameController()
        {
            ponyChallangeFacade = new PonyChallengeFacade();
            mazeMap = new MazeMap();
            ponyNavigator = new PonyNavigator();
        }

        public async Task<string> StartNewGame(int boardWidth, int boardHeight, string ponyName, int difficulty)
        {
            this.mazeId = await ponyChallangeFacade.StartNewGameAsync(boardWidth, boardHeight, ponyName, difficulty);
            var mazeState = await ponyChallangeFacade.GetMazeStateAsync(mazeId);
            mazeMap.Initialize(boardWidth, boardHeight, mazeState.PonyLocation, mazeState.ExitLocation, mazeState.data);

            var mazeSanpshot = await ponyChallangeFacade.PrintMazeStateAsync(this.mazeId);

            return mazeSanpshot;
        }

        public async Task RescuePony(int stepDelay)
        {
            var mazeState = await ponyChallangeFacade.GetMazeStateAsync(mazeId);
            Direction move = this.ponyNavigator.GetNextMove(mazeMap, mazeState);

            while (move != Direction.None)
            {
                await ponyChallangeFacade.MovePonyAsync(this.mazeId, move);
                var mazeSnapshot = await ponyChallangeFacade.PrintMazeStateAsync(this.mazeId);
                MoveMadeEvent?.Invoke(this, new GameEventArgs() {snapshot = mazeSnapshot});

                await Task.Delay(stepDelay);

                mazeState = await ponyChallangeFacade.GetMazeStateAsync(mazeId);
                move = this.ponyNavigator.GetNextMove(mazeMap, mazeState);

            } 
        }
    }
}
