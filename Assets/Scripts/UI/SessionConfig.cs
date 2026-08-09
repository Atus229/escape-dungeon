using System;
using System.Collections.Generic;
using System.Text;

namespace EscapeDungeon.Ui
{
     public static class SessionConfig
    {
        //luu trang thai phien choi khi chuyen scene
        public static bool StartNewGame = true;

        //round tiếp theo chơi là round mấy
        public static int OverrideRound = 0;

        // thời gian hoàn thành được hiển thị lên scene
        public static float LastRoundTime = 0f;

    }
}
