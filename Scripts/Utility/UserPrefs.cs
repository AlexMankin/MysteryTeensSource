using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserPrefs : MonoBehaviour {

	//VARIABLES
	public static Locale CurrentLocale;

	//CONSTANTS
	public static Locale DEFAULT_LOCALE = Locale.English;

	//EVENTS
	
	//METHODS
	
	public static void Initialize(){
		//TODO: load data from UserPrefs file
		CurrentLocale = DEFAULT_LOCALE;
	}

}
