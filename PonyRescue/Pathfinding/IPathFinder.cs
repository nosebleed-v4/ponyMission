using System.Collections.Generic;
using System.Threading.Tasks;

namespace PonyRescue
{
    internal interface IPathFinder
    {
        List<string> FindShortestPath();

        Task<List<string>> FindShortestPathAsync();
        void InitializeChambers(List<List<string>> data);
    }
}