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
using System;

public class TextDatabase {

	//VARIABLES
	public static Dictionary<int, Dictionary<int, TextData>> List;

	//CONSTANTS
	
	//EVENTS
	
	//METHODS

	public static void Initialize(){
		//May be called following initial boot up if Locale is changed

		List = new Dictionary<int, Dictionary<int, TextData>> ();

		string groupName, sequenceName;
		Type enumType;

        //Enums are magic
		foreach (int I in Enum.GetValues(typeof(TextGroups))) {
			if (I > 0){
				List [I] = new Dictionary<int, TextData> ();
				groupName = Enum.GetName (typeof(TextGroups), I);
				enumType = Type.GetType (groupName);

               	foreach(int J in Enum.GetValues(enumType)){
					if (J > 0){
						sequenceName = Enum.GetName (enumType, J);

                        List [I] [J] = Resources.Load<TextData> ("Text/" + groupName + "/" + sequenceName);

						if (List [I] [J] == null) {
							Debug.LogWarning ("Could not find TextData for " + groupName + "." + sequenceName);
						}

					}
				}
			}
		}
	}

	/// <summary>
	/// Grabs the text data object for the corresponding enum value. This may include a sequence of messages.
	/// </summary>
	/// <param name="sequenceEnum">An Enum value from one of the Txt enums (ie TxtSystem, TxtMultiChoice...)</param>
	public static TextData Get<TEnum>(TEnum sequenceEnum){
		int textGroup = GetGroupInt (sequenceEnum);

		if (textGroup < 0) {
			Debug.LogWarning ("Could not find text group " + sequenceEnum.GetType () + ".  Run \"Mystery Teens > Import Text\" and try again.");
			return null;
		}

		int textSequence = (int)Enum.Parse(sequenceEnum.GetType(), sequenceEnum.ToString());

		return List [textGroup] [textSequence];
	}

	/// <summary>
	/// Just grab the text from the first message in the data.
	/// </summary>
	/// <returns>Text from the first message in the data, from the locale currently defined by UserPrefs.CurrentLocale</returns>
	/// <param name="sequenceEnum">An Enum value from one of the Txt enums (ie TxtSystem, TxtMultiChoice...)</param>
	public static string GetString<TEnum>(TEnum sequenceEnum){
		if (Get (sequenceEnum) == null)
			return null;

		return Get (sequenceEnum).GetText ();
	}

	public static bool IsInitialized{
		get{
			return List != null;
		}
	}

	public static int GetGroupInt<TEnum>(TEnum value){
		TextGroups ret;
		Enum.TryParse<TextGroups> (value.GetType ().Name, out ret); 

		if (ret == TextGroups._NULL)
			return -1;

		return (int)ret;
	}
}
	
public enum Locale{
	English,
	Japanese
}

