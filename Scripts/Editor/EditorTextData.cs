using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ActionBaseClass), true)]
public class EditorTextData : Editor {


	public override void OnInspectorGUI(){
		base.OnInspectorGUI ();

		ActionBaseClass abc = (ActionBaseClass)target;
		TextData _textData = abc.Txt;
		//New GUI code

		if (_textData != null) {
			EditorGUILayout.TextArea (FormatText(_textData, "Txt"), GUILayout.MaxWidth(235f), GUILayout.MaxHeight(15f));
		}

		_textData = abc.Txt2;
		if (_textData != null) {
			EditorGUILayout.TextArea (FormatText(_textData, "Txt2"), GUILayout.MaxWidth(235f), GUILayout.MaxHeight(15f));
		}

		_textData = abc.Txt3;
		if (_textData != null) {
			EditorGUILayout.TextArea (FormatText(_textData, "Txt3"), GUILayout.MaxWidth(235f), GUILayout.MaxHeight(15f));
		}

        if (abc.AdditionalText != null) {
            for (int i = 0; i < abc.AdditionalText.Length; ++i) {
                _textData = abc.AdditionalText[i];
                if (_textData != null) {
                    EditorGUILayout.TextArea(FormatText(_textData, "AdditionalText[" + i + "]"), GUILayout.MaxWidth(235f), GUILayout.MaxHeight(15f));
                } 
            }
        }
    }

    string FormatText(TextData td, string textDataString) {
        return "//COPY: " + td.SequenceName + "\npublic IEnumerator Seq" + td.SequenceName + "(){" + "\nMessageBox.LoadData(" + textDataString + ");\n" + td.CodeHelper;
    }

}