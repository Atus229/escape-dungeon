using System.Text;
using UnityEngine;

namespace EscapeDungeon.Maze
{
    public class MazeGeneratorTester : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            MazeGrid grid = new MazeGrid(4, 3);

            MazeGenerator generator = new MazeGenerator();
            generator.MazeGenerate(grid, 0, 0);

            Debug.Log(BuildMazeAscii(grid));
        }

        private string BuildMazeAscii(MazeGrid grid)
        {
            StringBuilder sb = new StringBuilder();

            int width = grid.Width;
            int height = grid.Height;

            // Hàng top đầu tiên: dựa trên hasTopWall của hàng y = 0
            for (int x = 0; x < width; x++)
            {
                MazeCell cell = grid.Cells[x, 0];
                sb.Append(cell.HasTopWall ? "+---" : "+   ");
            }
            sb.AppendLine("+");

            // Mỗi hàng của maze
            for (int y = 0; y < height; y++)
            {
                StringBuilder middleLine = new StringBuilder();

                // Bắt đầu mỗi hàng: tường trái của cell (0,y)
                MazeCell firstCell = grid.Cells[0, y];
                middleLine.Append(firstCell.HasLeftWall ? "|" : " ");

                // Duyệt từng cell trong hàng
                for (int x = 0; x < width; x++)
                {
                    MazeCell cell = grid.Cells[x, y];

                    // Thân ô (3 khoảng trống, sau này nếu có ký tự nội dung thì để đây)
                    middleLine.Append("   ");

                    // Tường phải của cell hiện tại
                    if (cell.HasRightWall)
                        middleLine.Append("|");
                    else
                        middleLine.Append(" ");
                }

                sb.AppendLine(middleLine.ToString());

                // Sau khi vẽ xong middleLine cho hàng y, vẽ hàng đáy giữa y và y+1:
                // nếu y là hàng cuối, dùng hasDownWall; nếu không, có thể vẫn dùng hasDownWall để nhìn đáy từng cell
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