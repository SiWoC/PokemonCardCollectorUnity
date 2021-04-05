
using Factories.Config;
using System;
using System.Collections.Generic;

namespace Globals.PlayerStatsSegments
{
    [Serializable]
    public class Generation
    {
        public int generation;
        public int numberOfPacks;
        public Dictionary<int,Dictionary<string,PossibleCard>> cards = new Dictionary<int,Dictionary<string,PossibleCard>>();

        public Generation(int generationIn)
        {
            this.generation = generationIn;
        }

    }
}