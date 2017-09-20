using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour {

    Map map;

    public void Start()
    {
        map = GameObject.Find("Map").GetComponent<Map>();
    }

    public enum MenuType
    {
        Main,
        Save,
        Load,
        Presets,
        Edit
    }

    public MenuType currentMenu;
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(0, 0, Screen.width / 6, Screen.height / 6));
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();

        if (currentMenu == MenuType.Main)
        {
            if (GUILayout.Button("Save"))
            {
                currentMenu = MenuType.Save;

                //save
            }
            if (GUILayout.Button("Load"))
            {
                currentMenu = MenuType.Load;
                // load
            }
            if (GUILayout.Button("Presets"))
            {
                currentMenu = MenuType.Presets;
            }

            DebugManager.Instance.DB_PlaceWalls = GUILayout.Toggle(DebugManager.Instance.DB_PlaceWalls,"Place Walls");
             
        }
        else if (currentMenu == MenuType.Save)
        {
            GameState.current.Name = GUILayout.TextField(GameState.current.Name, 10);
            if (GUILayout.Button("Enter"))
            {
                map.SaveMap();
                SaveLoadSystem.Save();
                currentMenu = MenuType.Main;
            }
            if (GUILayout.Button("Back"))
                currentMenu = MenuType.Main;
        }
        else if (currentMenu == MenuType.Load)
        {
            GUILayout.Box("Select a Save File");
            GUILayout.Space(10);
            foreach(GameState game in SaveLoadSystem.savedStates)
            {
                if (GUILayout.Button(game.Name))
                {
                    GameState.current = game;
                    GameState.current.LoadVariables();
                    currentMenu = MenuType.Main;
                }
            }
            if (GUILayout.Button("Back"))
                currentMenu = MenuType.Main;
        }
        else if (currentMenu == MenuType.Presets)
        {
            GUILayout.Box("Select a Preset");
            GUILayout.Space(10);

            if (GUILayout.Button("Circle"))
            {
                AgentManager.Instance.CirclePreset();
            }
            if (GUILayout.Button("Side To Side"))
            {
                AgentManager.Instance.SideToSidePreset();
            }
            if (GUILayout.Button("Back"))
                currentMenu = MenuType.Main;
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.EndArea();

    }
}
