using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimator : MonoBehaviour {

	public AnimationCurve tCurveApp, tCurveDis, pCurveApp, pCurveDis;

	private Timer tTimer, bTimer, fTimer, pTimer;
	private float tLerp, bLerp, fLerp;
	private Coroutine tRoutine, bRoutine, fRoutine, pRoutine;

	//private Coroutine tRoutine, bRoutine, fRoutine, pRoutine;

	private Vector2 tBegin, tDestination;
	private bool bApp;

	private RectTransform rt;
	private Vector2 T2;
	private Vector3 T3;

	public enum MoveType{ Direct, AppearCurve, DisappearCurve }

	private bool tActive, bActive, fActive, pActive;

	//CONSTANTS
	private const float DEFAULT_TRANSLATE_DURATION = 1f;



	// Use this for initialization
	void Awake () {
		rt = GetComponent<RectTransform> ();
		if (rt == null) {
			Debug.LogError ("Cannot add UIAnimator to a non-UI object.");
		}

		tTimer = new Timer ();
		bTimer = new Timer ();
		fTimer = new Timer ();
		pTimer = new Timer ();
	}

	public void Translate(bool app, Vector2 dir, float t = DEFAULT_TRANSLATE_DURATION, float sizeRatio = 1f){
	//	if (tRoutine != null)
		//	StopCoroutine (tRoutine);
		
		StartCoroutine (crTranslate (app, dir, t, sizeRatio));
	}

	public void MoveTo(Vector2 destination, float t = DEFAULT_TRANSLATE_DURATION, MoveType M = MoveType.AppearCurve){
		if (tRoutine != null) {
			StopCoroutine (tRoutine);
			setPosition (tDestination);
		}

		tRoutine = StartCoroutine (crMoveTo(destination, t, M));
	}

	public void Blink(bool app, float t = 0f){

		//if (bRoutine != null)
			//StopCoroutine (bRoutine);

		//bRoutine = StartCoroutine (crBlink (app, t));
		if (bRoutine != null) {
			StopCoroutine (bRoutine);
			setScale (bApp ? 1f : 0f);
		}
		
		if (Helpers.IsFloatZero (t)) {
			setScale (app ? 1f : 0f);
			return;
		}

		bRoutine = StartCoroutine (crBlink (app, t));
	}

	public void Fade(bool app, float t){
		//if (fRoutine != null)
		//	StopCoroutine (fRoutine);
		//
		//fRoutine = StartCoroutine (crFade (app, t));
		StartCoroutine (crFade (app, t));
	}

	public void Pop(bool app, float t){

	}

	private IEnumerator crMoveTo(Vector2 destination, float t, MoveType M){

		tBegin = rt.anchoredPosition;
		tDestination = destination;

		tTimer.Begin(t);
		while (tTimer.IsRunning){

			if (M == MoveType.AppearCurve)
				setPosition (tBegin + ((tDestination - tBegin) * tCurveApp.Evaluate (tTimer.Percentage)));
			else if (M == MoveType.DisappearCurve)
				setPosition (tBegin + ((tDestination - tBegin) * tCurveDis.Evaluate (tTimer.Percentage)));
			else
				setPosition (tBegin + ((tDestination - tBegin) * tTimer.Percentage));
			
			yield return null;
		}

		setPosition (tDestination);
	}

	private IEnumerator crTranslate(bool app, Vector2 dir, float t, float sizeRatio){
		yield return new WaitUntil (() => !tActive);
		tActive = true;

		tTimer.Begin (t);
		tBegin = rt.anchoredPosition;
		tDestination = tBegin + (dir.normalized * ((Mathf.Abs(dir.x) > .01f ? rt.rect.width : rt.rect.height)));
		AnimationCurve crv = app ? tCurveApp : tCurveDis;

		while (tTimer.IsRunning) {
			setPosition (tBegin + (crv.Evaluate(tTimer.Percentage) * (tDestination - tBegin)));
			yield return null;
		}

		setPosition (tDestination);
		tActive = false;
	}

	private IEnumerator crBlink(bool app, float t){
		bApp = app;
		bTimer.Begin (t);
		while (bTimer.IsRunning) {
			setScale ((app) ? bTimer.Percentage : bTimer.PercentageInverse);
			yield return null;
		}

		setScale (app ? 1f : 0f);
	}

	private IEnumerator crFade(bool app, float t){
		yield return new WaitUntil (() => !fActive);
		fActive = true;

		fTimer.Begin (t);

		while (fTimer.IsRunning) {

			yield return null;
		}

		fActive = false;
	}

	private IEnumerator evalPop(bool app, float t){
		yield return new WaitUntil (() => !pActive);
		pActive = true;

		pTimer.Begin (t);
		while (pTimer.IsRunning) {

			yield return null;
		}
		pActive = false;
	}

	private void setScale(float f){
		T3 = rt.localScale;
		T3.y = f;
		rt.localScale = T3;
	}

	void setPosition(Vector2 pos){
		rt.anchoredPosition = pos;
	}

	public bool UseUnscaledTime{
		set { 
			tTimer.UsingUnscaledTime = true;
			bTimer.UsingUnscaledTime = true;
			fTimer.UsingUnscaledTime = true;
			pTimer.UsingUnscaledTime = true;
		}
		get{
			return tTimer.UsingUnscaledTime;
		}
	}

	public bool IsAnimating{
		get{
			return tTimer.IsRunning || bTimer.IsRunning || fTimer.IsRunning || pTimer.IsRunning;
		}
	}
}

public enum UITransformType{
	Translate,
	Blink,
	Fade,
	Pop
}