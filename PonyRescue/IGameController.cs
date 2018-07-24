using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PonyRescue
{
    interface IGameController
    {
        event EventHandler MoveMadeEvent;

        Task<string> StartNewGame(int boardWidth, int boardHeight, string ponyName, int difficulty);

        Task<string> RescuePony(int stepDelay);
    }
}
