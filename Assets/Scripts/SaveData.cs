using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Assets.Scripts
{
    public class SaveData
    {
        public static PacmanData currentData = new();
        static string GetSlotPath(int slotNumber)
        {
            string persistentPath = Application.persistentDataPath;
            switch (slotNumber)
            {
                case 1:
                    persistentPath += "/h.dat";
                    break;
                case 2:
                    persistentPath += "/v.dat";
                    break;
                case 3:
                    persistentPath += "/d.dat";
                    break;
                default:
                    return string.Empty;
            }
            return persistentPath;
        }
        public static void SaveGame(int slotNumber, PacmanData data)
        {
            if (slotNumber < 1 || slotNumber > 3)
            {
                Debug.Log("Invalid slot!");
                return;
            }
            BinaryFormatter bf = new BinaryFormatter();
            string path = GetSlotPath(slotNumber);
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                bf.Serialize(fs, data);
            }
            //FileMode.Create: If file exists it will be overwritten.
        }

        public static PacmanData LoadGame(int slotNumber)
        {
            if (slotNumber < 1 || slotNumber > 3)
            {
                Debug.Log("Invalid slot!");
                return null;
            }
            string path = GetSlotPath(slotNumber);
            if (File.Exists(path))
            {
                BinaryFormatter bf = new BinaryFormatter();
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    PacmanData pd = bf.Deserialize(fs) as PacmanData;
                    return pd;
                }
            }
            else
            {
                Debug.Log($"No file exists on slot {slotNumber}!");
                return null;
            }
        }

        public static bool CheckSlot(int slotNumber)
        {
            if (slotNumber < 1 || slotNumber > 3)
            {
                return false;
            }
            return File.Exists(GetSlotPath(slotNumber));
        }

        public static bool CheckAllSlots()
        {
            string slot1 = GetSlotPath(1);
            string slot2 = GetSlotPath(2);
            string slot3 = GetSlotPath(3);
            return File.Exists(slot1) || File.Exists(slot2) || File.Exists(slot3);
        }

        public static void RemoveSlot(int slotNumber)
        {
            string path = GetSlotPath(slotNumber);
            Debug.Log(slotNumber);
            Debug.Log(path);
            if (File.Exists(path))
            {
                File.Delete(path);
                Debug.Log("Removed");
            }
        }
    }
}
