using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiChoice : SummonableActor {

    //VARIABLES
    [SerializeField] private MultiOption[] options;

    public static int SelectedIndex { get; private set; }

    public static int HighlightedIndex {
        get { return _highlightedIndex; }
        private set {
            _highlightedIndex = value;
            _highlightedIndex = Mathf.Clamp(_highlightedIndex, 0, numberOfActiveOptions - 1);
        }
    }
    private static int _highlightedIndex;

    private static float bottomY, offsetY;
    private static int numberOfActiveOptions;
    
    private static MultiChoice instance;

    private static Vector2 t2;
    //CONSTANTS
    private const float SUMMON_STAGGER_TIME = 0.1f, HOLD_SELECTION_DURATION = 0.5f;
	//EVENTS
	
	//METHODS
	
	void Awake(){
        base.Awake();
        instance = this;

        bottomY = options[0].LocalPosition.y;
        offsetY = options[1].LocalPosition.y - bottomY;
        SelectedIndex = 0;
    }

    private void Update() {
        base.Update();

        if (SelectedIndex >= 0)
            return;

        if (FB_Input.ButtonDown(FB_Button.Up)) {
            HighlightedIndex--;
        }

        if (FB_Input.ButtonDown(FB_Button.Down)) {
            HighlightedIndex++;
        }

        if (!AreOptionsReady)
            return;

        if (FB_Input.ButtonDown(FB_Button.Confirm)) {
            FB_Audio.PlaySFX(SFX.Select);
            SelectedIndex = HighlightedIndex;
        }
    }

    public static IEnumerator Prompt(TextMessage option1, TextMessage option2, TextMessage option3 = null, TextMessage option4 = null) {
        IsVisible = true;
        HighlightedIndex = -1;
        numberOfActiveOptions = option4 == null ? option3 == null ? 2 : 3 : 4;
        List<TextMessage> optionsAsList = new List<TextMessage>();
        optionsAsList.Add(option1);
        optionsAsList.Add(option2);
        if (option3 != null) optionsAsList.Add(option3);
        if (option4 != null) optionsAsList.Add(option4);
        
        for (int i = 0; i < numberOfActiveOptions; ++i) {
            t2 = instance.options[i].LocalPosition;
            t2.y = bottomY + ((numberOfActiveOptions - i - 1) * offsetY);
            instance.options[i].LocalPosition = t2;

            instance.options[i].LoadMessage(optionsAsList[i], i);
            instance.options[i].Summon();
            yield return new WaitForSeconds(SUMMON_STAGGER_TIME);
        }

        yield return new WaitUntil(() => instance.options[numberOfActiveOptions - 1].IsReady);

        SelectedIndex = -1;
        HighlightedIndex = 0;
        yield return new WaitUntil(() => SelectedIndex >= 0);

        for (int i = 0; i < numberOfActiveOptions; ++i) {
            if (i != SelectedIndex)
                instance.options[i].Close();
        }

        yield return new WaitForSeconds(HOLD_SELECTION_DURATION);
        instance.options[SelectedIndex].Close();
        IsVisible = false;
    }

    //Overrides


    //PROPERTIES
    public static bool IsVisible { get; private set; }
    private static bool AreOptionsReady { get {
            for (int i = 0; i < numberOfActiveOptions; ++i) {
                if (instance.options[i].IsHighlighting)
                    return false;
            }
            return true;
        }
    }
   
}
