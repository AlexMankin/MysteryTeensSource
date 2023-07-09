using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FB_Input : MonoBehaviour {

	//VARIABLES
	private static List<InputMapping> inputMap;
	private static Dictionary<FB_Button, bool> prevInput, currentInput;

	//CONSTANTS
	public const string INPUT_NAME_HORIZONTAL = "Horizontal",
	INPUT_NAME_VERTICAL = "Vertical",
	INPUT_NAME_MOUSEX = "MouseX",
	INPUT_NAME_MOUSEY = "MouseY";

	//EVENTS
	public delegate void EventHandlerButton(FB_Button btn);

	public static EventHandlerButton E_ButtonDown;
	public static EventHandlerButton E_ButtonUp;
	public static EventHandlerButton E_ButtonHolding;
	public static EventHandlerButton E_AnalogChange;

	//METHODS

	public static void Initialize(){
		inputMap = new List<InputMapping> ();
		registerMappingFromUserPrefs ();

		prevInput = new Dictionary<FB_Button, bool> ();
		currentInput = new Dictionary<FB_Button, bool> ();

		foreach (FB_Button B in Enum.GetValues(typeof(FB_Button))) {
			prevInput.Add (B, false);
			currentInput.Add (B, false);
		}

		registerMappingFromDefaults ();
		registerMappingFromUserPrefs ();
	}

	void Update(){
		updateInputLists ();

		foreach (FB_Button B in Enum.GetValues(typeof(FB_Button))) {
			if (GetButton (B)) {
				if (E_ButtonHolding != null) E_ButtonHolding (B);
			}

			if (ButtonDown (B)) {
				if (E_ButtonDown != null) E_ButtonDown (B);
			}

			if (ButtonUp (B)) {
				if (E_ButtonUp != null) E_ButtonUp (B);
			}
		}
	}

	public static bool GetButton(FB_Button btn){
		return currentInput [btn];
	}

	public static bool ButtonDown(FB_Button btn){
		return (!prevInput [btn] && currentInput [btn]);
	}

	public static bool ButtonUp(FB_Button btn){
		return (prevInput [btn] && !currentInput [btn]);
	}

	private void updateInputLists(){
		//Since buttons might be mapped to multiple keys, resetting and settings need to be two separate steps.
		foreach (FB_Button B in Enum.GetValues(typeof(FB_Button))) {
			prevInput [B] = currentInput [B];
			currentInput [B] = false;
		}

		foreach (InputMapping M in inputMap) {
			if (Input.GetKey (M.Key)) {
				currentInput [M.Button] = true;
			}
		}
	}

	private static void registerMappingFromUserPrefs(){
		
	}

	private static void registerMappingFromDefaults(){
		registerMapping (FB_Button.Up, KeyCode.UpArrow);
		registerMapping (FB_Button.Left, KeyCode.LeftArrow);
		registerMapping (FB_Button.Right, KeyCode.RightArrow);
		registerMapping (FB_Button.Down, KeyCode.DownArrow);

		registerMapping (FB_Button.Sprint, KeyCode.LeftShift);

		registerMapping (FB_Button.Quit, KeyCode.Escape);

		registerMapping (FB_Button.Confirm, KeyCode.Z);
		registerMapping (FB_Button.Cancel, KeyCode.X);
        registerMapping(FB_Button.Meow, KeyCode.C);

        registerMapping (FB_Button.FullScreen, KeyCode.F);
        registerMapping(FB_Button.FullScreen, KeyCode.F1);
    }

	private static void registerMapping(FB_Button btn, KeyCode key){
		foreach (InputMapping M in inputMap) {
			if (M.Button == btn && M.Key == key) {
				Debug.LogWarning ("Keycode " + key + " is already mapped to button " + btn);
				return;
			}
		}

		InputMapping newMapping;
		newMapping.Button = btn;
		newMapping.Key = key;
		inputMap.Add (newMapping);
	}

	private static void removeMapping(FB_Button btn, KeyCode key){
		InputMapping mapToRemove = new InputMapping();
		mapToRemove.Button = FB_Button._NULL;

		foreach (InputMapping M in inputMap) {
			if (M.Button == btn && M.Key == key) {
				mapToRemove = M;
				break;
			}
		}
			
		if (mapToRemove.Button != FB_Button._NULL)
			inputMap.Remove (mapToRemove);
	}
		
}

public enum FB_Button{
	_NULL = 0,
	Up = 1,
	Down = 2,
	Left = 3, 
	Right = 4,
	Confirm = 5,
	Cancel = 6,
	Meow = 7,
	FullScreen = 8,
	Sprint = 9,
	AnalogX = 11,
	AnalogY = 12,
	Quit = 15
}

public struct InputMapping{
	public FB_Button Button;
	public KeyCode Key;
}