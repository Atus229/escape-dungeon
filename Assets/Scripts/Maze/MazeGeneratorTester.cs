using System.Text;
using UnityEngine;

namespace EscapeDungeon.Maze
{
    public class MazeGeneratorTester : MonoBehaviour
    {
        public MazeGrid currentMaze { get; private set; }
        private void Update()
        {
            int i = 20;
            if (Input.GetKeyDown(KeyCode.M))
            {
                i++;
                Debug.Log($"Ma tran co kich thuoc {i}");
                MazeGrid grid = new MazeGrid(i, i);

                MazeGenerator generator = new MazeGenerator();
                generator.MazeGenerate(grid, 0, 0);

                this.currentMaze = grid;

                Debug.Log(BuildMazeAscii(grid));


            }
        }

        private string BuildMazeAscii(MazeGrid grid)
        {
            StringBuilder sb = new StringBuilder();

            int width = grid.Width;
            int height = grid.Height;

            // Hàng mái trên cùng (y = 0)
            for (int x = 0; x < width; x++)
            {
                MazeCell cell = grid.Cells[x, 0];
                sb.Append(cell.HasTopWall ? "+---" : "+   ");
            }
            sb.AppendLine("+");

            // Mỗi hàng cell
            for (int y = 0; y < height; y++)
            {
                StringBuilder middleLine = new StringBuilder();

                // Tường trái của cell (0, y)
                MazeCell firstCell = grid.Cells[0, y];
                middleLine.Append(firstCell.HasLeftWall ? "|" : " ");

                // Duyệt từng cell trong hàng
                for (int x = 0; x < width; x++)
                {
                    MazeCell cell = grid.Cells[x, y];

                    // Thân ô: hiện dấu chấm để dễ thấy lối đi
                    middleLine.Append(" . ");

                    // Tường phải
                    middleLine.Append(cell.HasRightWall ? "|" : " ");
                }

                sb.AppendLine(middleLine.ToString());

                // Hàng đáy giữa hàng y và y+1 (hoặc đáy cuối nếu y là hàng cuối)
                StringBuilder bottomLine = new StringBuilder();
                for (int x = 0; x < width; x++)
                {
                    MazeCell cell = grid.Cells[x, y];
                    bottomLine.Append(cell.HasDownWall ? "+---" : "+   ");
                }
                bottomLine.Append("+");
                sb.AppendLine(bottomLine.ToString());
            }

            return sb.ToString();
        }
    }
}