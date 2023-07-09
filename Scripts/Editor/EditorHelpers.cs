/* 
	Copyright (c) 2019 Homestuck Inc 
	Author(s) of Code: Alex Mankin
	Do Not Distribute
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public static class EditorHelpers {

    //VARIABLES

    //CONSTANTS
    public const string PATH_SCENE = "Assets/_Scenes/";
	//EVENTS
	
	//METHODS
	
    public static string SceneFolderFromIndex(int index) {
        return SceneFolderFromIndex(index.ToString("NNN"));
    }
	public static string SceneFolderFromIndex(string index) {

        DirectoryInfo dir;
        DirectoryInfo[] subDirs, sceneDirs;

        dir = new DirectoryInfo(PATH_SCENE);
        subDirs = dir.GetDirectories();

        for (int i = 0; i < subDirs.Length; ++i) {
            sceneDirs = subDirs[i].GetDirectories();
            for (int j = 0; j < sceneDirs.Length; ++j) {
                if (sceneDirs[j].Name.Substring(1, 3).Equals(index)) {
                    return PATH_SCENE + subDirs[i].Name + '/' + sceneDirs[j].Name + '/';
                }
            }
        }

        return "";
    }

    public static string SceneFolderFromSceneCode(string code) {
        //RR.AAA.z  R = Region, A = Area, z = zone
        //01.001.b = Woodworld (01), Hometown (001), Scoffy's Home (b)
        string[] codeSplit = code.Split('.');

        if (codeSplit.Length <= 0) {
            Debug.LogWarning("Scene Code [" + code + "] invalid. Please use format RR.AAA.z");
            return null;
        }

        string returnPath = PATH_SCENE;
        returnPath += GetFolderMatchingPrefix(returnPath, codeSplit[0]);
        if (codeSplit.Length > 1) returnPath += GetFolderMatchingPrefix(returnPath, codeSplit[1]);
        if (codeSplit.Length > 2) returnPath += GetFolderMatchingPrefix(returnPath, codeSplit[2]);

        return returnPath;
    }

    private static string GetFolderMatchingPrefix(string path, string matchPrefix) {
        DirectoryInfo dir = new DirectoryInfo(path);
        DirectoryInfo[] subDirs = dir.GetDirectories();
        string currentSubDirName;
        for (int i = 0; i < subDirs.Length; ++i) {
            currentSubDirName = Path.GetFileNameWithoutExtension(subDirs[i].Name);
            if (currentSubDirName.Equals(matchPrefix)) {
                return Path.GetFileName(subDirs[i].FullName) + "/";
            }
        }

        return "";
    }
   
	
	
	//PROPERTIES
}
