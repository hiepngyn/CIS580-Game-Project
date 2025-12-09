using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GameProject
{
    [Serializable]

    public class SaveData
    {
        public Vector2 PlayerPosition { get;set; }
        public List<AnimalData> Animals { get; set; } = new List<AnimalData>();
        public string CurrentLevel { get; set; }
        public int LevelNumber { get; set; }
    }

    [Serializable]
    public class AnimalData
    {
        public string Type { get; set; } 
        public Vector2 Position { get; set; }
        public bool IsInPen { get; set; }
    }
}
