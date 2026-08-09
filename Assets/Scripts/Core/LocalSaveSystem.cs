using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace EscapeDungeon.Core
{
    public static class LocalSaveSystem
    {
        //Application.persistent là unity tự cung cấp đường dẫn -> để lưu lâu dài trên máy tùy thuộc vào thiết bị yêu 
        private static string SavePath => Path.Combine(Application.persistentDataPath, "save.json"); 
        
        public static void Save(PlayerData data)
        {
            //chuyển dữ liệu object thành json 
            string json = JsonUtility.ToJson(data, prettyPrint: true); //prettyPrint -> in json đẹp, dễ nhìn

            //Ghi json ra file
            File.WriteAllText(SavePath, json);

            Debug.Log("[LocalSaveSystem] saved to " + SavePath);
        }


        public static PlayerData Load()
        {
            if (!File.Exists(SavePath))
            {
                Debug.Log("[LocalSaveSystem] No save file found");
                return null;
            }

            string json = File.ReadAllText(SavePath);
            //Chuyển json thành dữ liệu kiểu PlayerData
            var data = JsonUtility.FromJson<PlayerData>(json);

            Debug.Log("[LocalSaveSystem] Loaded from " + SavePath);
            return data;
        }

        public static void ResetProgressToNewGame()
        {
            PlayerProfileData profile = new PlayerProfileData
            {
                PlayerName = "Player",
                CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                LastPlayedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            GameProgressData progress = new GameProgressData
            {
                // New Game luôn bắt đầu từ round 1
                CurrentRound = 1,

                // Chưa hoàn thành round nào
                HighestCompletedRound = 0,

                // Giá trị mặc định ban đầu
                CurrentHP = 100,
                HintRemaining = 2,
                TotalPlayTime = 0f
            };

            GameSettingData setting = new GameSettingData();

            PlayerData newGameData = new PlayerData
            {
                Profile = profile,
                Progress = progress,
                Setting = setting
            };

            Save(newGameData);

            Debug.Log(
                "[LocalSaveSystem] New Game confirmed. " +
                "Progress reset to round 1."
            );
        }
    }
}
