using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EscapeDungeon.Core
{
    [CreateAssetMenu(fileName ="LevelConfig", menuName ="EscapeDungeon/LevelConfig", order = 0)]
    public class LevelConfig : ScriptableObject
    {
        public int roundNumber;
        public int mazeWidth;
        public int mazeHeight;
        public int timeLimits;
        public int maxHints;

    }
}
