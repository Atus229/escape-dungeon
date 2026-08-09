using System;
using System.Diagnostics;

namespace EscapeDungeon.Maze
{
    [Serializable]
    public class MazeCell
    {
        public int X;
        public int Y;
        public bool Visited;
        public bool HasLeftWall;
        public bool HasRightWall;
        public bool HasTopWall;
        public bool HasDownWall;

        public MazeCell(int x, int y)
        {
            this.X = x;
            this.Y = y;
            this.Visited = false;
            this.HasLeftWall = true;
            this.HasRightWall = true;
            this.HasTopWall = true;
            this.HasDownWall = true;
        }

        public void SetVisited()
        {
            this.Visited = true;
        }

        public void RemoveWall(Direction direction)
        {
            switch (direction)
            {
                case Direction.Left:
                    this.HasLeftWall = false;
                    break;
                case Direction.Right:
                    this.HasRightWall = false;
                    break;
                case Direction.Top:
                    this.HasTopWall = false;
                    break;
                case Direction.Down:
                    this.HasDownWall = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Không phải hướng hợp lệ được lưu...");
                    break;
            }
        }

        public static bool IsEqual(MazeCell currentCell, MazeCell exitCell)
        {
            if(currentCell.X == exitCell.X && currentCell.Y == exitCell.Y)
            {
                return true;
            }
            return false;
        }
    }
}