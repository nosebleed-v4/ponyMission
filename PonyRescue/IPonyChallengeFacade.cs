using System.Threading.Tasks;

namespace PonyRescue
{
    internal interface IPonyChallengeFacade
    {
        Task<string> StartNewGameAsync(int width, int height, string name, int difficulty);

        Task<MazeState> GetMazeStateAsync(string mazeId);

        Task MovePonyAsync(string mazeId, Direction direction);

        Task<string> PrintMazeStateAsync(string mazeId);
    }
}
