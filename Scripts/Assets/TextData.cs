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

[System.Serializable]
public class TextData : ScriptableObject {
	public string GroupName, SequenceName;
	[TextArea(0, 20)]public string CodeHelper;
	public List<TextMessage> Messages;

	private const string TXT_INDEX_OUT_OF_RANGE = "INDEX {0} OUT OF RANGE FOR TEXT {1}",
	TXT_UNIQUE_ID_NOT_FOUND = "COULD NOT FIND UNIQUE ID {0} FOR TEXT {1}";


	public TextData(){
		Messages = new List<TextMessage> ();
	}

	public string GetText(int index = 0){
		if (index < 0 || index >= Messages.Count) {
			return string.Format (TXT_INDEX_OUT_OF_RANGE, index, GroupName + "." + name);
		}

		return Messages [index].Text;
	}

	public bool Equals(TextData other){
		if (other == null)
			return false;
		
		return other.GroupName.Equals (GroupName) && other.SequenceName.Equals (SequenceName);
	}
}

[System.Serializable]
public class TextMessage{
    public bool iHaveData;
	public string[] textLocales;
	public TextData textData;
	public AnimData animation;
	public AudioClip voiceClip;
	public AllPersons person;
    public PersonData personData;
	public int portraitNo;
	public bool autoSkip;
    public bool noText;
    public bool forceLeft, forceRight;
    public bool bumpExisting;
    public bool innerThoughts;

	private const string TXT_NO_LOCALE = "NO TEXT DEFINED FOR {0} IN LOCALE {1}";

	public TextMessage(){
		textLocales = new string[Enum.GetValues (typeof(Locale)).Length];
		portraitNo = 0;
	}

	public string Text{
		get{
			if (string.IsNullOrEmpty(textLocales[(int)UserPrefs.CurrentLocale])) {
				return string.Format (TXT_NO_LOCALE, textData.GroupName + "." + textData.SequenceName, UserPrefs.CurrentLocale);
			}

			return textLocales [(int)UserPrefs.CurrentLocale];
		}
	}

    public string Nametag => personData != null ? personData.Nametag : "";

    public AnimData Portrait => personData?.GetPortrait(portraitNo);
        
}
