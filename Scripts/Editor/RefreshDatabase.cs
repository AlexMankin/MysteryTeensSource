/* 
	Copyright (c) 2019 Homestuck Inc 
	Author(s) of Code: Alex Mankin
	Do Not Distribute
*/

using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Collections.Generic;

public class RefreshDatabase{
    private static List<string> nameList;
    private static string[] names;

    private const string OBJECTS_PATH = "Assets/Objects/",
    RESOURCES_PATH = "Assets/Resources/",
    SCENE_PATH = "Assets/_Scenes/",
    SCENE_DATA_PATH = OBJECTS_PATH + "SceneData/",
    ANIM_PATH = "Assets/Objects/Animations/",
    ENUM_NAME_PFX = "All",
    ANIM_IDLE = "Idle",
    ANIM_WALK = "Walk",
    ANIM_SUFFIX_BACK = "B",
        ANIM_HURT = "Hurt",
        ANIM_KO = "KO",
        ANIM_ACTIVE = "Active";

	[MenuItem("Mystery Teens/Refresh Asset Database")]
	public static void RefreshAll(){
		refresh ("Persons", OBJECTS_PATH, GlobalMiscData.FILE_EXT_ASSET);
		refresh ("BGM", RESOURCES_PATH, FB_Audio.AUDIO_EXT, true, "");
		refresh ("SFX", RESOURCES_PATH, FB_Audio.AUDIO_EXT, false, "");

		refreshScenes ();	//Scenes are special enough to get their own method
		setActorData();

		AssetDatabase.SaveAssets ();
	}

	private static void refresh(string dataType, string path, string fileExtension, bool serializable = false, string prefix = ENUM_NAME_PFX, bool includeDataTypeFolder = true){
		string fullPath = includeDataTypeFolder ? path + dataType : path;
		DirectoryInfo dir = new DirectoryInfo (fullPath);
		FileInfo[] fileInfo = dir.GetFiles ("*" + fileExtension);
		names = new string[fileInfo.Length];

		for (int i = 0; i < fileInfo.Length; ++i) {
			names [i] = Path.GetFileNameWithoutExtension(fileInfo [i].Name);
		}

		EnumBuilder.Build (prefix + dataType, names, true, serializable);
	}

	private static void refreshScenes(){
        //TODO: All scenes need to translate directly to a SceneData object
        //000.00.a.SquirrelHome.asset
        //Get index from SceneManager, passing in the file name

        string basePath = SCENE_PATH;
        DirectoryInfo dir = new DirectoryInfo(basePath);
        DirectoryInfo[] subDirectories = dir.GetDirectories();

        FileInfo[] imagesInFolder;
        for (int i = 0; i < subDirectories.Length; ++i) {
            imagesInFolder = subDirectories[i].GetFiles("*.png");
            for (int j = 0; j < imagesInFolder.Length; ++j) {
                formatImage(SCENE_PATH + subDirectories[i].Name + "/" + imagesInFolder[j].Name, SpriteAlignment.BottomLeft);
            }
        }
    }

    static void PrepFolderAsSceneFolder(string path) {
        DirectoryInfo dir = new DirectoryInfo(path);

        if (dir == null)
            return;

        //Format the images
        FileInfo[] imgFileInfo = dir.GetFiles("*.png");

        for (int i = 0; i < imgFileInfo.Length; ++i) {
            formatImage(path + Path.GetFileName(imgFileInfo[i].Name), SpriteAlignment.BottomLeft); 
        }
    }

	private static void setActorData(){
		DirectoryInfo dir = new DirectoryInfo (OBJECTS_PATH + "Persons");
		FileInfo[] fileInfo = dir.GetFiles ("*" + GlobalMiscData.FILE_EXT_ASSET);
		for (int i = 0; i < fileInfo.Length; ++i) {
			PersonData A = AssetDatabase.LoadAssetAtPath<PersonData> (OBJECTS_PATH + "Persons/" + fileInfo [i].Name);
			Enum.TryParse<AllPersons> (Path.GetFileNameWithoutExtension(fileInfo [i].Name), out A.personName);

			string animPath = ANIM_PATH + Path.GetFileNameWithoutExtension("Anim" + fileInfo [i].Name);

			if (Directory.Exists (animPath)) {
                A.animations.idleDown = AssetDatabase.LoadAssetAtPath<AnimData>(animPath + "/" + ANIM_IDLE + GlobalMiscData.FILE_EXT_ASSET);
                A.animations.idleUp = AssetDatabase.LoadAssetAtPath<AnimData>(animPath + "/" + ANIM_IDLE + ANIM_SUFFIX_BACK + GlobalMiscData.FILE_EXT_ASSET);
                A.animations.walkDown = AssetDatabase.LoadAssetAtPath<AnimData>(animPath + "/" + ANIM_WALK + GlobalMiscData.FILE_EXT_ASSET);
                A.animations.walkUp = AssetDatabase.LoadAssetAtPath<AnimData>(animPath + "/" + ANIM_WALK + ANIM_SUFFIX_BACK + GlobalMiscData.FILE_EXT_ASSET);
 

            }

			AssetDatabase.Refresh ();
			EditorUtility.SetDirty (A);
		}
			
	}


	static void formatImage(string path, SpriteAlignment alignment = SpriteAlignment.BottomCenter){
		TextureImporter TI = new TextureImporter ();
		TextureImporterSettings TIS = new TextureImporterSettings ();
		TextureImporterPlatformSettings TIPS = new TextureImporterPlatformSettings ();

        Debug.Log(path);
		TI = TextureImporter.GetAtPath(path) as TextureImporter;
		TI.ReadTextureSettings (TIS);

        if (TIS.spritePixelsPerUnit == GlobalMiscData.PPU) {
            //We already formatted this guy, let's not mess with him anymore
            return;
        }

		TIS.spritePixelsPerUnit = GlobalMiscData.PPU;
		TIS.spriteAlignment = (int)alignment;
		TIS.filterMode = FilterMode.Point;

		TIPS.maxTextureSize = 4096;
		TIPS.textureCompression = TextureImporterCompression.Uncompressed;
		TI.SetTextureSettings (TIS);
		TI.SetPlatformTextureSettings (TIPS);

		AssetDatabase.ImportAsset (path, ImportAssetOptions.ForceUpdate);
	}
}