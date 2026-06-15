using System;
using System.Diagnostics;

namespace EscapeDungeon.Maze
{
    public class MazeCell
    {
        public int X { get; private set;}
        public int Y { get; private set; }
        public bool Visited { get; private set; }
        public bool HasLeftWall { get; private set; }
        public bool HasRightWall { get; private set; }
        public bool HasTopWall { get; private set; }
        public bool HasDownWall { get; private set; }

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
    }
}