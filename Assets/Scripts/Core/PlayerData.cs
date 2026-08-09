using System;
using System.Collections.Generic;
using System.Text;

namespace EscapeDungeon.Core
{
    [Serializable] // để đánh dấu file này có thể chuyển thành JSON và ngược lại
    public class PlayerData
    {
        public PlayerProfileData Profile;
        public GameProgressData Progress;
        public GameSettingData Setting;
    }
}
