using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoadSystem {
    public static List<GameState> savedStates = new List<GameState>();

    public static void Save()
    {
        savedStates.Add(GameState.current);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/savedGames.gd");
        bf.Serialize(file, SaveLoadSystem.savedStates);
        file.Close();
    }

    public static void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/savedGames.gd"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = 
                File.Open(Application.persistentDataPath + "/savedGames.gd",FileMode.Open);
            SaveLoadSystem.savedStates = (List<GameState>)bf.Deserialize(file);
            file.Close();
        }
    }
}
