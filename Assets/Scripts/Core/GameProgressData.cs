using EscapeDungeon.Maze;
using System;
using System.Collections.Generic;
using System.Text;

namespace EscapeDungeon.Core
{
    [Serializable] // để đánh dấu file này có thể chuyển thành JSON và ngược lại
    public class GameProgressData
    {
        public int CurrentRound;
        public int HighestCompletedRound;
        public int CurrentHP;
        public int HintRemaining;
        public float TotalPlayTime;

        public float RemainingTime;

        public float PlayerPosX;
        public float PlayerPosY;
        public float PlayerPosZ;

        public int MazeWidth;
        public int MazeHeight;
        public List<MazeCell> Cells;
    }
}
