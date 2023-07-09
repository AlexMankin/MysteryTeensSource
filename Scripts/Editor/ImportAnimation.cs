using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System;

public class ImportAnimation : AssetPostprocessor{

	//CONSTANTS
	private const string JSON_PATH = "Assets/Import/Animation/",
	ANIMATION_PATH = "Assets/Objects/Animations/",
	RAW_IMAGE_BASE_PATH = "Assets/Import/Animation/",
	EXT_ASSET = ".asset",
	ANIMATION_ENUM_NAME = "AllAnimations",
	SCENE_PATH = "Assets/_Scenes/";


	//VARIABLES
	static DirectoryInfo dir;
	static FileInfo[] fileInfo;
	static string[] enumNames, sheetNames;
	static string fileName;
	static SpriteMetaData[] sprites; 
	static string imgPath;
	static string jsonString;
	static int numAnimations;

	static AseData aseData;

	[MenuItem ("Mystery Teens/Import Animations")]
	static void Go(){
		numAnimations = 0;

		dir = new DirectoryInfo (JSON_PATH);
		fileInfo = dir.GetFiles ("*.json");
		numAnimations += fileInfo.Length;

		sheetNames = new string[numAnimations];
		int sheetIndex = 0;

		foreach (FileInfo file in fileInfo) {
			jsonString = File.ReadAllText (JSON_PATH + file.Name);
			aseData = JsonUtility.FromJson<AseData> (jsonString);
			imgPath = aseData.meta.image;
			fileName = Path.GetFileNameWithoutExtension (file.Name);

			sheetNames [sheetIndex] = fileName;
			sheetIndex++;

			//CreateEnums ();
			CreateSpriteAssets (RAW_IMAGE_BASE_PATH );
			GenerateAnimations ();
		}
		
		//EnumBuilder.Build (ANIMATION_ENUM_NAME, sheetNames, true);

		AssetDatabase.Refresh ();
	}

	static void CreateEnums(){
		enumNames = new string[aseData.meta.frameTags.Length];
		for (int i = 0; i < aseData.meta.frameTags.Length; ++i) {
			enumNames [i] = aseData.meta.frameTags [i].name;
		}

		//EnumBuilder.Build (fileName, enumNames, true);
	}

	static void CreateSpriteAssets(string path){
		if (File.Exists (path + Path.GetFileName(imgPath))) {
			TextureImporter TI = TextureImporter.GetAtPath (path + Path.GetFileName(imgPath)) as TextureImporter;
			TI.spriteImportMode = SpriteImportMode.Multiple;
			TI.compressionQuality = 0;
			TI.textureCompression = TextureImporterCompression.Uncompressed;
			TI.spritePixelsPerUnit = GlobalMiscData.PPU;

			bool isCentered = false;
			for (int i = 0; i < aseData.meta.layers.Length; ++i) {
				if (aseData.meta.layers [i].data != null) {
					if (aseData.meta.layers [i].data.ToUpper () == "CENTER")
						isCentered = true;
				}
			}

			TI.spritePivot = (!isCentered) ? new Vector2 (0.5f, 0f) : new Vector2(0.5f, 0.5f);
			TI.filterMode = FilterMode.Point;
			//TI.filterMode = FilterMode.Point;

			TI.name = fileName;

			sprites = new SpriteMetaData[aseData.frames.Length];
			for (int i = 0; i < aseData.frames.Length; ++i) {
				AseFrameSizeData currentFrameSize = aseData.frames [i].frame;
				sprites [i].rect = new Rect (currentFrameSize.x, aseData.meta.size.h - (((currentFrameSize.y / currentFrameSize.h) + 1) * currentFrameSize.h), currentFrameSize.w, currentFrameSize.h);
				sprites [i].name = i.ToString("D3") + aseNameByIndex (i);
				sprites [i].alignment = (!isCentered) ? (int)SpriteAlignment.BottomCenter : (int)SpriteAlignment.Center;
			}

			TI.spritesheet = sprites;

			AssetDatabase.ImportAsset (path + Path.GetFileName(imgPath), ImportAssetOptions.ForceUpdate);

			Debug.Log ("Generated " + aseData.frames.Length + " sprites for sheet " + fileName);
		} else {
			Debug.LogError ("Image not found: " + (path + Path.GetFileName(imgPath)) + "\nAnimation: + " + fileName);
		}
	}

	private static void GenerateAnimations(){ 
		//GenerateAnimations is called within a loop, file data is currently in static variables
		int animStartFrame, animEndFrame;
		string destinationDirectory;
		string sceneName = fileName.Split ('_') [0]; 
		if (Directory.Exists (SCENE_PATH + sceneName)) {
			destinationDirectory = SCENE_PATH + sceneName + "/Animations";
			Directory.CreateDirectory (SCENE_PATH + sceneName + "/Animations/");
		} else {
			destinationDirectory = ANIMATION_PATH + fileName;
			Directory.CreateDirectory (ANIMATION_PATH + fileName);
		}

		string assetPathAndName;

		UnityEngine.Object[] spriteObjects = AssetDatabase.LoadAllAssetRepresentationsAtPath (RAW_IMAGE_BASE_PATH + fileName + ".png");

		for (int i = 0; i < aseData.meta.frameTags.Length; ++i){
			//Each loop is a unique animation 

			assetPathAndName = destinationDirectory + "/" + aseData.meta.frameTags [i].name + EXT_ASSET;

			AnimData A;
			if (File.Exists (assetPathAndName)) {
				A = AssetDatabase.LoadAssetAtPath<AnimData> (assetPathAndName);
			} else {
				A = ScriptableObject.CreateInstance<AnimData> ();
			}

			animStartFrame = aseData.meta.frameTags [i].from;
			animEndFrame = aseData.meta.frameTags [i].to;

			A.Name = aseData.meta.frameTags [i].name;
			A.SheetName = fileName;
			A.FrameList = new AnimFrame[animEndFrame - animStartFrame + 1];
			A.Loop = true;

			for (int j = 0; j < A.FrameList.Length; ++j) {
				A.FrameList [j] = new AnimFrame ();
				A.FrameList [j].Sprite = spriteObjects [animStartFrame + j] as Sprite;
				A.FrameList [j].Duration = .001f * aseData.frames[animStartFrame + j].duration;

				if (isLoopFrame (animStartFrame + j)) {
					if (j == 0)
						A.Loop = false;
					else {
						A.FrameList [j].LoopFromHere = true;
					}
				}

				if (!string.IsNullOrEmpty(getSFX(animStartFrame + j))){
					Enum.TryParse<SFX> (getSFX (animStartFrame + j), out A.FrameList [j].SoundEffect);
					if (A.FrameList [j].SoundEffect == SFX._NULL)
						Debug.LogWarning ("Could not find SFX (" + getSFX (animStartFrame + j) + ") for animation " + fileName + "/" + aseData.meta.frameTags [i].name);
				}
			}

			if (!File.Exists (assetPathAndName)) {
				AssetDatabase.CreateAsset (A, assetPathAndName);
			}

			EditorUtility.SetDirty (A);
		}

		AssetDatabase.Refresh ();
		AssetDatabase.SaveAssets ();

	}
		
	public static bool isLoopFrame(int index){
		for (int i = 0; i < aseData.meta.layers.Length; ++i) {
			if (aseData.meta.layers [i].cels != null) {
				for (int j = 0; j < aseData.meta.layers [i].cels.Length; ++j) {
					if (aseData.meta.layers [i].cels [j].frame == index && aseData.meta.layers [i].cels [j].color != null) {
						return true;
					}
				}
			}
		}
		return false;
	}

	static string aseNameByIndex(int index){
		//Get the name of the animation from the sprite index
		for (int i = 0; i < aseData.meta.frameTags.Length; ++i){
			if (aseData.meta.frameTags [i].to >= index && aseData.meta.frameTags[i].from <= index) {
				return aseData.meta.frameTags [i].name + "_" + (index - aseData.meta.frameTags [i].from).ToString("D2");
			}
		}
		return "ERROR_" + index;
	}

	static string getSFX(int index){
		for (int i = 0; i < aseData.meta.layers.Length; ++i){
			if (aseData.meta.layers [i].cels != null) {
				for (int j = 0; j < aseData.meta.layers [i].cels.Length; ++j) {
					if (aseData.meta.layers [i].cels [j].data != null && aseData.meta.layers [i].cels [j].frame == index) {
						return aseData.meta.layers [i].cels [j].data;
					}
				}
			}
		}

		return null;
	}
}
