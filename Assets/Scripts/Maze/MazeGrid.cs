using System;
using System.Collections.Generic;
using System.Text;

namespace EscapeDungeon.Maze
{
    [Serializable]
    public class MazeGrid
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public MazeCell[,] Cells { get; private set; }
        public MazeCell startCell { get; private set; }
        public MazeCell exitCell { get; private set; }

        public MazeGrid(int width, int height, MazeCell startCell, MazeCell exitCell)
        {
            if(width<=0 || height <= 0)
            {
                throw new ArgumentException("width & height phải là số nguyên > 0");
            }

            this.Width = width;
            this.Height = height;

            this.Cells = new MazeCell[width, height];
            for(int x = 0; x < width; x++)
            {
                for(int y=0;y< height; y++)
                {
                    Cells[x, y] = new MazeCell(x, y);
                }
            }

            this.startCell = GetCell(startCell.X, startCell.Y);
            this.exitCell = GetCell(exitCell.X, exitCell.Y);
        }

        public bool IsInside(int x, int y)
        { 
            if((x >= 0 && x < this.Width) && (y >= 0 && y < this.Height)) { 
                return true;
            }
            return false;
        }

        public MazeCell GetCell(int x, int y)
        {
            if (IsInside(x, y))
            {
                return Cells[x,y];
            }

            return null;
        }

        // Trong class MazeGrid
        public void SetCell(int x, int y, MazeCell cell)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                throw new ArgumentOutOfRangeException(
                    $"SetCell out of range: ({x},{y})"
                );
            }

            Cells[x, y] = cell;
        }

    }
}
