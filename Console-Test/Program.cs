using Jinher.AMP.SNS.Service.BE.Neighbor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Console_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            NeighborStore.CreateInstance().Init();
        }
    }
}
