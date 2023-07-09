//
//    `````              Code founded by Alex Mankin                       
// .:///////:-.                               -.    
//./////////////:-`          `....``    ```.-://:-` 
//`/////////////////:.``  .:///////////////////+yy/-
//  `..---:://////////////////////////////////::::.`
//           `-://////////////////////////////`     
//                ```````.//////::::--..-////.      
//                      -//////-       `--``-/`     
//                      -//:.`         .     ./.    
//                        -::.                ``  
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using System.Linq;

public class sv : MonoBehaviour {

    //VARIABLES

    public static SaveData Data;

    private static int currentSlot;
    private static SaveData dataBeingLoaded;
    private static SaveStatus status;

    private static List<SaveData> listOfSaves;

    public enum SaveStatus { Ready, Saving, Loading };

    //CONSTANTS
    private const int NUM_SAVE_SLOTS = 3;
    private static readonly string PATH_EXT = ".sav";
    private static readonly string TMP_EXT = ".tmp";
    private static readonly string BACKUP_EXT = ".backup";
    private static string PATH_BASE;    //Cannot use persistent data path, defined in Awake

    //EVENTS

    //METHODS

    void Awake() {

    }

    public static void Initialize() {
        PATH_BASE = Application.persistentDataPath + "/";

        //dataBeingLoaded = new SaveData ();

        //Data = dataBeingLoaded;


        Data.PersonNameIndices = new List<PersonIndex>();
        Data.VariablePackages = new List<ActionVariablePackage>();
        Data.ArtInspectedList = new List<ArtPiece>();
        Data.catName = new char[NamingScreen.MAX_CHARS];
        listOfSaves = new List<SaveData>();
    }

    public static bool Save(int slot = -1, bool savePlayerPosition = true) {
        if (!canSave)
            return false;

        status = SaveStatus.Saving;

        if (slot < 0)
            slot = currentSlot;

        Debug.Log("Saving data to slot " + (slot + 1));

        GenerateFileList();

        if (Player.PlayableActor != null && savePlayerPosition) {
            Data.Sys_PlayerPositionX = Player.PlayableActor.transform.position.x;
            Data.Sys_PlayerPositionY = Player.PlayableActor.transform.position.y;
        }

        Data.Sys_DateOfSave = DateTime.Now;

        string path = PATH_BASE + slot + PATH_EXT;
        FileStream file;
        BinaryFormatter bf = new BinaryFormatter();

        try {
            file = new FileStream(path + TMP_EXT, FileMode.OpenOrCreate);

            bf.Serialize(file, Data);
            file.Close();

            //Replace existing .SAV file with temporary file
            if (!File.Exists(path))
                File.Move(path + TMP_EXT, path);
            else
                File.Replace(path + TMP_EXT, path, path + BACKUP_EXT);
        }
        catch (System.Exception e) {
            throwSaveException(slot, e);
            status = SaveStatus.Ready;
            FB_Audio.PlaySFX(SFX.Denied);
            return false;
        }

        status = SaveStatus.Ready;
       // FB_Audio.PlaySFX(SFX.ItemGetNoVoice);
        return true;
    }

    public static bool Load(int slot = 0) {
        if (!canLoad)
            return false;

        status = SaveStatus.Loading;
        Debug.Log("Loading data from slot " + (slot + 1));

        GenerateFileList();

        if (listOfSaves.Count <= slot) {
            Debug.LogWarning("No data found in slot " + (slot + 1));
            status = SaveStatus.Ready;
            FB_Audio.PlaySFX(SFX.Denied);
            return false;
        }

        currentSlot = slot;
        Data = listOfSaves[slot];
        Data.Sys_LoadingSaveData = true;

        SceneLoader.Load(Data.currentSceneIndex);

        status = SaveStatus.Ready;
        return true;
    }

    public static void GenerateFileList() {
        listOfSaves = new List<SaveData>();

        DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath);
        FileInfo[] fileInfo = dir.GetFiles("*" + PATH_EXT);
        SaveData? currentFileData;
        FileStream file;
        BinaryFormatter bf = new BinaryFormatter();

        for (int i = 0; i < fileInfo.Length; ++i) {

            try {
                file = new FileStream(fileInfo[i].FullName, FileMode.Open);
                currentFileData = (SaveData)bf.Deserialize(file);

                if (currentFileData != null)
                    listOfSaves.Add(currentFileData.Value);

                file.Close();
            }
            catch (System.Exception e) {
                throwLoadException(0, e);
            }

        }
    }


    private static void throwSaveException(int slot, System.Exception e) {
        Debug.LogWarning("Failed to save file in slot " + slot + "\n" + e.ToString());
        status = SaveStatus.Ready;
    }

    private static void throwLoadException(int slot, System.Exception e) {
        Debug.LogWarning("Failed to load file in slot " + slot + "\n" + e.ToString());
        status = SaveStatus.Ready;
    }

    private static bool canSave {
        get {
           // if (IGA.IsRunning)
             //   return false;

            if (SceneLoader.CurrentScene == 0 || SceneLoader.CurrentScene == 1)
                return false;

            return true;
        }
    }
    private static bool canLoad {
        get {
            if (IGA.IsRunning)
                return false;

            //if (SceneLoader.CurrentScene == 0)
           //     return false;

            return true;
        }
    }

    public static SaveStatus Status {
        get {
            return status;
        }
    }

    public static bool HasData {
        get {
            GenerateFileList();

            return listOfSaves.Count > 0;
        }
    }

    public static string CatName => listOfSaves[0].CatName;
}

