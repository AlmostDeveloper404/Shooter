using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Main
{
    public static class SaveLoadProgress
    {
        public static void SaveData<T>(T playerData, int identifier)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            string path = Application.persistentDataPath + "/playerProgress.bn" + $"{identifier}";
            FileStream fileStream = new FileStream(path, FileMode.Create);


            binaryFormatter.Serialize(fileStream, playerData);
            fileStream.Close();
        }

        public static T LoadData<T>(int identifier)
        {
            string path = Application.persistentDataPath + "/playerProgress.bn" + $"{identifier}";
            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);

                
                T playerProgress = (T)formatter.Deserialize(stream);
                stream.Close();

                return playerProgress;
            }
            else
            {
                Debug.Log($"File doesn't found! Path: {path} ");
                return default(T);
            }
        }

        public static void DeleteData(int identifier)
        {
            string path = Application.persistentDataPath + "/playerProgress.bn" + $"{identifier}";
            File.Delete(path);
        }
    }
}


