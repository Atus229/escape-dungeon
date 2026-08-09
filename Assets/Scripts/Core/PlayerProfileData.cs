using System;
using System.Collections.Generic;
using System.Text;

namespace EscapeDungeon.Core
{
    [Serializable] // để đánh dấu file này có thể chuyển thành JSON và ngược lại
    public class PlayerProfileData
    {
        public string PlayerName;
        public string CreatedAt;
        public string LastPlayedAt;
    }
}
