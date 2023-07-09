using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageBox : SummonableActor {

    //VARIABLES
    [Header("Message Box")]
    [SerializeField] private Actor textActor;
    [SerializeField] private Actor bgActor;
    [SerializeField] private Actor bgBorderActor;
    [SerializeField] private Actor nametagActor;
    [SerializeField] private FB_Sprite portraitSprite;
    public static TextData ActiveTextData { get; private set; }
    public static TextMessage ActiveMessage { get; private set; }
    private static Timer holdTimer;
    private static Vector3 deployedPosition, closedPosition;

    private static AllPersons prevMessagePerson;
    private static MessageBox instance;

    private static Vector3 t3;

    private static SuperTextMesh DialogueTextMesh => instance.textActor.textMesh;
    //CONSTANTS
    private const float HOLD_DURATION = 0.1f, HORIZONTAL_PAN_DISTANCE = 442f, COLOR_SWITCH_DURATION = 0.1f;
	//EVENTS
	
	//METHODS
	
	void Awake(){
        base.Awake();

        if (instance != null) {
            Destroy(gameObject);
            return;
        }

        instance = this;
        deployedPosition = LocalPosition;
        t3 = deployedPosition;
        t3.x -= HORIZONTAL_PAN_DISTANCE;
        closedPosition = t3;
        holdTimer = new Timer();
        
    }

    private void Start() {
        base.Start();
        CloseLerp(1f); 
    }

    public static void LoadData(TextData newData) {
        ActiveTextData = newData;
    }

    public static IEnumerator Display(int index = 0) {

        if (ActiveTextData == null) {
            Debug.LogWarning("No text data loaded!");
            yield break;
        }

        if (index < 0 || ActiveTextData.Messages.Count <= index) {
            Debug.LogWarning("Loaded text data does not contain a message at index " + index);
            yield break;
        }

        ActiveMessage = ActiveTextData.Messages[index];
        yield return new WaitUntil(() => instance.IsReady);

        instance.portraitSprite.SetAnimation(ActiveMessage.Portrait);

        string finalText = ActiveMessage.Text;
        finalText = ReplaceCatName(finalText);

        DialogueTextMesh.text = finalText;

        if (ActiveMessage.person != prevMessagePerson) {
            instance.nametagActor.textMesh.text = ActiveMessage.Nametag;
            instance.SetColor(ActiveMessage.personData, COLOR_SWITCH_DURATION);
            prevMessagePerson = ActiveMessage.person;
        }


        while (DialogueTextMesh.reading) {
            if (InputAdvance) {
                DialogueTextMesh.SkipToEnd();
            }

            yield return null;
        }           

        if (!ActiveMessage.autoSkip) 
            yield return new WaitUntil(() => FB_Input.ButtonDown(FB_Button.Confirm));

        if (!ActiveMessage.autoSkip)
            FB_Audio.PlaySFX(SFX.Book_PageTurn1);

        ActiveMessage = null;
        holdTimer.Run(HOLD_DURATION);
    }

    private void SetColor(PersonData person, float duration = 0f) {
        if (person == null) {
            bgActor.color.AdjustColor(Color.white, duration);
            bgBorderActor.color.AdjustColor(Color.black, duration);
            nametagActor.TextColorAdjuster.AdjustColor(Color.gray, duration);
            textActor.TextColorAdjuster.AdjustColor(Color.black, duration);
            return;
        }

        bgActor.color.AdjustColor(person.colorLight, duration);
        bgBorderActor.color.AdjustColor(person.colorDark, duration);
        nametagActor.TextColorAdjuster.AdjustColor(person.colorMid, duration);
        textActor.TextColorAdjuster.AdjustColor(person.colorDark, duration);
    }

    static string ReplaceCatName(string text) {
        int openIndex = text.IndexOf("<cat");
        int closeIndex = text.IndexOf('>');

        if (openIndex < 0 || closeIndex < 0)
            return text;

        return text.Substring(0, openIndex) + sv.Data.CatName + text.Substring(closeIndex + 1);
    }

    //SUMMON OVERRIDES
    protected override void OnSummon() {
        instance.portraitSprite.SetAnimation(null);
        instance.SetColor(ActiveMessage.personData);
        instance.nametagActor.textMesh.text = "";
        FB_Audio.PlaySFX(SFX.Book_PageFlip1);
    }

    protected override void OnClose() {
        base.OnClose();
        prevMessagePerson = AllPersons.NULL;
    }
    protected override void SummonLerp(float t) {
        LocalPosition = closedPosition + (deployedPosition - closedPosition) * t;
        instance.textActor.TextColorAdjuster.Alpha = t;
        instance.color.Alpha = t;
    }

    //PROPERTIES
    protected override bool ShouldBeVisible => ActiveMessage != null || holdTimer.IsRunning || MultiChoice.IsVisible;
    public static bool CanSkip => !ActiveMessage.noText;
    public static bool InputAdvance => FB_Input.ButtonDown(FB_Button.Confirm) || FB_Input.GetButton(FB_Button.Cancel);
    public static bool ReadyToAdvance => ActiveMessage != null && !DialogueTextMesh.reading && instance.IsReady;
}
