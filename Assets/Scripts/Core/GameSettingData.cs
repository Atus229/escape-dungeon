using System;
using System.Collections.Generic;
using System.Text;

namespace EscapeDungeon.Core
{
    [Serializable] // để đánh dấu file này có thể chuyển thành JSON và ngược lại
    public class GameSettingData
    {
        public float MusicVolume;
        public float SfxVolume;
        public bool ShowMinimap;
        public float Sensitivity;
    }
}
