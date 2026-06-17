using EscapeDungeon.Maze;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting.Antlr3.Runtime.Collections;


namespace EscapeDungeon.Algorithms
{
    public class DFSSolver
    {
        public List<MazeCell> FindPath(MazeGrid maze, MazeCell startCell, MazeCell exitCell)
        {
            //biến
            var path = new List<MazeCell>();
            var stack = new Stack<MazeCell>();
            var width = maze.Width;
            var height = maze.Height;
            var visited = new bool[width, height];
            var parent = new MazeCell[width, height];
            bool found = false;

            //guard clause
            ValidiateInputs(maze, startCell, exitCell);

            if (startCell == exitCell)
            {
                path.Add(exitCell);
                return path;
            }


            //code xử lý
            visited[startCell.X, startCell.Y] = true;
            stack.Push(startCell);

            MazeCell currentCell;
            while (stack.Count != 0)
            {
                currentCell = stack.Pop();

                if (currentCell == exitCell)
                {
                    found = true;
                    break;
                }

                //Lấy danh sách neighbor hợp lệ
                var neighbors = GetNeighbors(maze, currentCell, visited);

                foreach (var neighbor in neighbors)
                {
                    visited[neighbor.X, neighbor.Y] = true;
                    parent[neighbor.X, neighbor.Y] = currentCell;
                    stack.Push(neighbor);
                }
            }

            if (found == true)
            {
                path = ReconstructPath(startCell, exitCell, parent);

                return path;
            }
            else
            {
                return new List<MazeCell>();
            }

        }



        private static List<MazeCell> ReconstructPath(MazeCell startCell, MazeCell exitCell, MazeCell[,] parent)
        {
            var path = new List<MazeCell>();
            MazeCell currentCell = exitCell;
            while (currentCell != null)
            {
                path.Add(currentCell);

                if (currentCell == startCell)
                {
                    break;
                }

                currentCell = parent[currentCell.X, currentCell.Y];

            }

            path.Reverse();
            return path;
        }



        private static void ValidiateInputs(MazeGrid maze, MazeCell startCell, MazeCell exitCell)
        {
            if (maze == null)
            {
                throw new ArgumentNullException("Bạn chưa khởi tạo maze");
            }

            if (startCell == null || exitCell == null)
            {
                throw new ArgumentNullException("Bạn chưa khởi tạo startCell hoặc exitCell");
            }

        }

        private List<MazeCell> GetNeighbors(MazeGrid maze, MazeCell currentCell, bool[,] visited)
        {
            var neighbors = new List<MazeCell>();
            var directions = new List<MazeCell>(4);

            directions.Add(maze.GetCell(currentCell.X + 1, currentCell.Y));
            directions.Add(maze.GetCell(currentCell.X - 1, currentCell.Y));
            directions.Add(maze.GetCell(currentCell.X, currentCell.Y - 1));
            directions.Add(maze.GetCell(currentCell.X, currentCell.Y + 1));

            for (int i = 0; i < directions.Count; i++)
            {
                if (directions[i] == null)
                {
                    continue;
                }
                var x = directions[i].X;
                var y = directions[i].Y;
                if (directions[i] != null && visited[x,y] != true && !HasWallBetween(currentCell, directions[i]))
                {
                    neighbors.Add(directions[i]);
                }

            }
                return neighbors;
        }

        private bool HasWallBetween(MazeCell currentCell, MazeCell neighborCell)
        {
            if (currentCell == null) throw new ArgumentNullException(nameof(currentCell));
            if (neighborCell == null) throw new ArgumentNullException(nameof(neighborCell));

            int cX = currentCell.X;
            int cY = currentCell.Y;
            int nX = neighborCell.X;
            int nY = neighborCell.Y;

            if ((nX == cX + 1 && nY == cY) && (currentCell.HasRightWall == true))
            {
                //nextCell nằm bên phải
                return true;
            }
            else if ((nX == cX - 1 && nY == cY) && (currentCell.HasLeftWall == true))
            {
                //nextCell nằm bên trái
                return true;
            }
            else if ((nX == cX && cY + 1 == nY) && (currentCell.HasDownWall == true))
            {
                //nextCell nằm bên dưới
                return true;
            }
            else if ((nX == cX && cY - 1 == nY) && (currentCell.HasTopWall == true))
            {
                //nextCell nằm bên trên
                return true;
            }

            return false;
        }
    }
}
