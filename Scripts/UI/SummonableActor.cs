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

/// <summary>
/// Automatically summons or closes based on ShouldBeVisible condition. Specifics for what that entails can also be customized.
/// </summary>
public abstract class SummonableActor : Actor {
    [Header("Summonable")]
    public float summonDuration = 0.15f;
    public float closeDuration = 0.15f;
    public bool defaultActive;

    protected void Summon() { if (CanSummon) StartCoroutine(CrSummon()); }
    protected void Close() { if (CanClose) StartCoroutine(CrClose()); }
    protected IEnumerator CrSummon() {
        IsVisible = true;
        IsSummoning = true;
        OnSummon();

        Timer summonTimer = new Timer(summonDuration);
        while (summonTimer.IsRunning) {
            SummonLerp(summonTimer.Percentage);
            yield return null;
        }

        SummonLerp(1f);
        IsSummoning = false;
    }
    protected IEnumerator CrClose() {
        IsClosing = true;
        OnClose();

        Timer closeTimer = new Timer(closeDuration);
        while (closeTimer.IsRunning) {
            CloseLerp(closeTimer.Percentage);
            yield return null;
        }

        CloseLerp(1f);

        IsVisible = false;
        IsClosing = false;
    }

    protected virtual void OnSummon() { }
    protected virtual void OnClose() { }

    protected virtual void SummonLerp(float t) {
        sprite.Alpha = t;
    }
    protected virtual void CloseLerp(float t) {
        SummonLerp(1f - t);
    }

    protected void Start() {
        base.Start();
        if (defaultActive) {
            IsVisible = true;
            IsActive = true;
            SummonLerp(1f);
        }
        else {
            CloseLerp(1f);
        }
    }

    protected void Update() {
        base.Update();
        if (ShouldBeVisible)
            Summon();
        else
            Close();
    }

    protected virtual bool ShouldBeVisible { get; set; }
    public bool IsVisible { get; protected set; }
    public bool IsReady => IsVisible && !IsSummoning && !IsClosing;
    public bool IsSummoning { get; protected set; }
    public bool IsClosing { get; protected set; }
    protected virtual bool CanSummon => !IsVisible;
    protected virtual bool CanClose => IsReady;
}