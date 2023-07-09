using UnityEditor;
using UnityEngine;
using System.IO;
using System;

public class EnumBuilder {
	//Create enums dynamically

	private static readonly string BASE_PATH = "Assets/Enums/";

	private static int currentIndex, highestRecordedValue;
	private static Type enumType;

	public static void Build(string name, string[] values, bool includeNullValue = true, bool makeSerializable = false){

		currentIndex = 1;
		highestRecordedValue = 1;
		enumType = null;
		for (int i = 0; i < System.AppDomain.CurrentDomain.GetAssemblies().Length && enumType == null; ++i) {
			enumType = System.AppDomain.CurrentDomain.GetAssemblies () [i].GetType (name);
		}

		if (enumType != null) {
			highestRecordedValue = findHighestValue ();
		}

		if (values.Length > 0) {
			using (StreamWriter streamWriter = new StreamWriter (BASE_PATH + "ENM_" +  name + ".cs")) {
				if (makeSerializable)
					streamWriter.WriteLine ("[System.Serializable]");
				streamWriter.WriteLine ("public enum " + name);
				streamWriter.WriteLine ("{");

				if (includeNullValue)
					streamWriter.WriteLine ("\t" + GlobalMiscData.ENUM_NULL_STRING + " = 0,");

				for (int i = 0; i < values.Length; ++i) {
					if (values [i] != null) {

						if (enumType != null) {
							if (Enum.IsDefined (enumType, values [i])) {
								currentIndex = (int)Enum.Parse (enumType, values [i]);
							} else
								currentIndex = highestRecordedValue + 1;
						}

						if (values [i].Contains (".") || values [i].Contains (" ")) {
							Debug.LogWarning ("Imported asset contains invalid characters: " + values [i] + "\nPlease remove them and reimport.");
						} else
							streamWriter.WriteLine ("\t" + values [i] + " = " + currentIndex + ",");

						if (currentIndex > highestRecordedValue)
							highestRecordedValue = currentIndex;

						currentIndex = highestRecordedValue + 1;
					}
				}

				streamWriter.WriteLine ("}");

				Debug.Log ("Enums successfully generated for " + name + "!");
			}
		} else {
			Debug.LogWarning ("Cannot generate enums for empty list: " + name);
		}
	}

	private static int findHighestValue(){
		int val, highest = 1; 	//Because 0 is reserved for _NULL

		foreach (var enumValue in Enum.GetValues(enumType)) {
			val = (int)enumValue;
			if (val > highest)
				highest = val;
		}

		return highest;
	}
}