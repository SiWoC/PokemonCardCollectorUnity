using System;
using UnityEditor;
using UnityEngine;

namespace Factories.Config
{
    [Serializable]
    public class PossibleCard
    {
        public string id;
        public string name;
        public int nationalPokedexNumber;
        public string rarity;
        public string setCode;
        public string imageUrlSmall;
        public string imageUrlLarge;
        public int generation;
        public int numberOwned;
    }
}