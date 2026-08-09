using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EscapeDungeon.Maze
{
    class MazeGenerator : MonoBehaviour
    {
        private System.Random _random = new System.Random();

        public void MazeGenerate(MazeGrid maze)
        {
            //Khởi tạo biến
            var stack = new Stack<MazeCell>();
            var neighbors = new List<MazeCell>();
            MazeCell startCell = maze.startCell;
            MazeCell currentCell;
            MazeCell nextCell;

            //Các ngoại lệ có thể có
            if (maze == null)
            {
                throw new ArgumentNullException("Bạn chưa khởi tạo maze");
            }

            if(startCell.X < 0 || startCell.X >= maze.Width || startCell.Y <0 || startCell.Y >= maze.Height)
            {
                throw new ArgumentOutOfRangeException("Điểm bắt đầu không thuộc trong maze");
            }

            if(startCell == null)
            {
                throw new InvalidOperationException("startCell không được null");
            }

            //Code xử lý
            startCell.SetVisited();
            stack.Push(startCell);

            while(stack.Count > 0)
            {
                currentCell = stack.Peek();
                neighbors = GetUnvisitedNeighbors(maze, currentCell);

                
                if (neighbors.Count > 0)
                {
                    nextCell = ChooseRandomNeighbor(neighbors);
                    RemoveWallsBetween(stack, currentCell, nextCell);
                    //Đẩy nextCell vào push
                    nextCell.SetVisited();
                    stack.Push(nextCell);
                }
                else
                {
                    stack.Pop();
                }
            }

        }

        private static void RemoveWallsBetween(Stack<MazeCell> stack, MazeCell currentCell, MazeCell nextCell)
        {
            if (currentCell == null) throw new ArgumentNullException(nameof(currentCell));
            if (nextCell == null) throw new ArgumentNullException(nameof(nextCell));

            int cX = currentCell.X;
            int cY = currentCell.Y;
            int nX = nextCell.X;
            int nY = nextCell.Y;

            if (nX == cX + 1 && nY == cY)
            {
                //nextCell nằm bên phải
                currentCell.RemoveWall(Direction.Right);
                nextCell.RemoveWall(Direction.Left);
            }
            else if (nX == cX - 1 && nY == cY)
            {
                //nextCell nằm bên trái
                currentCell.RemoveWall(Direction.Left);
                nextCell.RemoveWall(Direction.Right);
            }
            else if (nX == cX && cY + 1 == nY)
            {
                //nextCell nằm bên trên -> theo trục unity(z) thì y+1 là đi lên
                currentCell.RemoveWall(Direction.Top);
                nextCell.RemoveWall(Direction.Down);
            }
            else if (nX == cX && cY - 1 == nY)
            {
                //nextCell nằm bên dưới
                currentCell.RemoveWall(Direction.Down);
                nextCell.RemoveWall(Direction.Top);
            }
            else
            {
                throw new InvalidOperationException("next cell không nằm cạnh currentcell nên không thể đục tường");
            }

        }

        private MazeCell ChooseRandomNeighbor(List<MazeCell> neighbors)
        {
            int index = _random.Next(0, neighbors.Count);
            return neighbors[index];
        }

        private static List<MazeCell> GetUnvisitedNeighbors(MazeGrid maze, MazeCell currentCell)
        {
            if (maze == null) throw new ArgumentNullException(nameof(maze));
            if (currentCell == null) throw new ArgumentNullException(nameof(currentCell));

            var neighbors = new List<MazeCell>(4);
            var directions = new List<MazeCell>(4);

            directions.Add(maze.GetCell(currentCell.X + 1, currentCell.Y));
            directions.Add(maze.GetCell(currentCell.X - 1, currentCell.Y));
            directions.Add(maze.GetCell(currentCell.X, currentCell.Y - 1));
            directions.Add(maze.GetCell(currentCell.X, currentCell.Y + 1));

            for (int i = 0; i < directions.Count; i++)
            {
                if (directions[i] != null && directions[i].Visited != true)
                {
                    neighbors.Add(directions[i]);
                }
            }

            return neighbors;
        }
    }
}
