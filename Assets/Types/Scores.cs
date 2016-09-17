using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Types
{
    public static class Scores
    {
        //positive
        public static int HitShip = 10;
        public static int DestroyShip = 20;
        public static int DestroyRocket = 50; 
        //negative 
        public static int HitAnotherPlayer = -25;
        public static int DestroyAnotherPlayer = -500;
        public static int HitBuilding = -50;
        public static int DestroyBuilding = -1000; 
    }
}
