/* 
	Copyright (c) 2019 Homestuck Inc 
	Author(s) of Code: Alex Mankin
	Do Not Distribute
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MenuItems : MonoBehaviour {

    //VARIABLES

    //CONSTANTS
    const string PATH_PERSONS = "Assets/Objects/Persons/",
        PATH_SCENE_PREFAB = "Assets/Prefabs/Map Construction/NewScene.prefab",
        PATH_EXIT_PREFIX = "Assets/Prefabs/Map Construction/EntryExit/",
        PATH_EXIT_EAST = "EastExit.prefab",
        PATH_EXIT_NORTH = "NorthExit.prefab",
        PATH_EXIT_WEST = "WestExit.prefab",
        PATH_EXIT_SOUTH = "SouthExit.prefab";

    //EVENTS

    //METHODS

    void Awake(){
		
	}
	
	[MenuItem("GameObject/Mystery Teens/Create Actor", false, 2)] 
	static void createActor(){
		GameObject O = new GameObject ();
		O.name = "New Actor";
        O.transform.SetParent(Selection.activeTransform);
        O.AddComponent<Actor> ();
		O.AddComponent<FB_Sprite> ();
		Selection.activeObject = O;
	}

    [MenuItem("GameObject/Mystery Teens/Create Ghost", false, 3)]
    static void createReferencePosition() {
        GameObject O = new GameObject();
        O.name = "GhNew";
        O.transform.SetParent(Selection.activeTransform);
        O.AddComponent<Ghost>();
        O.GetComponent<Ghost>().person = AssetDatabase.LoadAssetAtPath<PersonData>(PATH_PERSONS + "Margaret.asset");
        O.GetComponent<Ghost>().OnValidate();
        Selection.activeObject = O;
        
    }

    [MenuItem("GameObject/Mystery Teens/NEW SCENE TEMPLATE", false,1)]
    static void createScenePrefab() {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PATH_SCENE_PREFAB);
        GameObject O = Instantiate(prefab, null, true);
        int numChildren = O.transform.childCount;
        for (int i = 0; i < numChildren; ++i) {
            O.transform.GetChild(0).SetParent(null, true);
        }
       DestroyImmediate(O);
    }

    [MenuItem("GameObject/Mystery Teens/Scene Exit/East", false)]
    static void createEastExit() {
        createExit(PATH_EXIT_EAST);
    }

    [MenuItem("GameObject/Mystery Teens/Scene Exit/North", false)]
    static void createNorthExit() {
        createExit(PATH_EXIT_NORTH);
    }

    [MenuItem("GameObject/Mystery Teens/Scene Exit/West", false)]
    static void createWestExit() {
        createExit(PATH_EXIT_WEST);
    }

    [MenuItem("GameObject/Mystery Teens/Scene Exit/South", false)]
    static void createSouthExit() {
        createExit(PATH_EXIT_SOUTH);
    }

    static void createExit(string fileName) {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PATH_EXIT_PREFIX + fileName);
        GameObject O = Instantiate(prefab, Selection.activeTransform, true);
        O.name = "NewExit";
        SceneEntrance entrance = O.GetComponentInChildren<SceneEntrance>();
        entrance.GetComponent<SpriteRenderer>().hideFlags = HideFlags.HideInInspector;
        Selection.activeGameObject = O;
    }
    //PROPERTIES
}
