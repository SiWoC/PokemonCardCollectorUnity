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
        public string imageUrl;
        public string supertype;
        public int number;
        public string rarity;
        public string setCode;
        public string imageUrlHiRes;
        public int nationalPokedexNumber;
    }
}