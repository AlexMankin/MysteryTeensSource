using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiOption : SummonableActor {

    //VARIABLES
    [SerializeField] private Actor textActor;

    private int currentAssociatedIndex;
    private float activeX, highlightedX;
    private float preClosingX;

    private bool isHighlighted;
    private static Vector2 t2;
    //CONSTANTS
    private const float PAN_DISTANCE_X = 190f, DURATION_HIGHLIGHT = 0.1f;

    //EVENTS

    //METHODS
    private void Awake() {
        base.Awake();
        activeX = LocalPosition.x;
        highlightedX = 0f;
    }

    private void Update() {
        base.Update();

        if (!IsReady)
            return;

        if (ShouldBeHighlighted && !isHighlighted) {
            StartCoroutine(crHighlight());
        }

        if (!ShouldBeHighlighted && isHighlighted) {
            StartCoroutine(crHighlight(true));
        }
    }
    public void LoadMessage(TextMessage msg, int associatedIndex) {
        currentAssociatedIndex = associatedIndex;
        textActor.textMesh.text = msg.Text;
    }

    public void Summon() {
        isHighlighted = false;
        ShouldBeVisible = true;
    }

    public void Close() {
        ShouldBeVisible = false;
    }

    IEnumerator crHighlight(bool isClosing = false) {
        isHighlighted = !isClosing;
        IsHighlighting = true;
        Timer highlightTimer = new Timer();
        while (highlightTimer.Run(DURATION_HIGHLIGHT)) {
            t2 = LocalPosition;
            t2.x = Mathf.Lerp(activeX, highlightedX, isClosing ? highlightTimer.PercentageInverse : highlightTimer.Percentage);
            LocalPosition = t2;
            yield return null;
        }

        t2 = LocalPosition;
        t2.x = isClosing ? activeX : highlightedX;
        LocalPosition = t2;
        IsHighlighting = false;
    }

    //Overrides
    protected override void SummonLerp(float t) {
        t2 = LocalPosition;
        t2.x = Mathf.Lerp(activeX + PAN_DISTANCE_X, activeX, t);
        LocalPosition = t2;
    }

    protected override void OnClose() {
        base.OnClose();
        preClosingX = LocalPosition.x;
    }

    protected override void CloseLerp(float t) {
        t2 = LocalPosition;
        t2.x = Mathf.Lerp(preClosingX, activeX + PAN_DISTANCE_X, t);
        LocalPosition = t2;
    }

    //PROPERTIES
    private bool ShouldBeHighlighted => IsVisible && MultiChoice.HighlightedIndex == currentAssociatedIndex;
    public bool IsHighlighting { get; private set; }
    
}
