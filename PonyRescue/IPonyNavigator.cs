﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PonyRescue
{
    interface IPonyNavigator
    {
        Direction GetNextMove(IMazeMap mazeMap, MazeState mazeState);
    }
}
