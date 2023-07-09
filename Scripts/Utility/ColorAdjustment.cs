using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ColorAdjustment {

	private SpriteRenderer targetSprite;
	private Image targetImage;
	private RawImage targetRawImage;
	private SuperTextMesh targetText;
	private MonoBehaviour sourceBehaviour;
	private Timer colorTimer, alphaTimer;
	private Coroutine colorRoutine, alphaRoutine;

	private Color C, startColor;
	private float A, startAlpha;

	public ColorAdjustment(SpriteRenderer T, MonoBehaviour B){
		targetSprite = T;
		sourceBehaviour = B;

		colorTimer = new Timer ();
		alphaTimer = new Timer ();
	}
	public ColorAdjustment(Image I, MonoBehaviour B){
		targetImage = I;
		sourceBehaviour = B;

		colorTimer = new Timer ();
		alphaTimer = new Timer ();
	}
	public ColorAdjustment(RawImage I, MonoBehaviour B){
		targetRawImage = I;
		sourceBehaviour = B;

		colorTimer = new Timer ();
		alphaTimer = new Timer ();
	}
	public ColorAdjustment(SuperTextMesh STM, MonoBehaviour B){
		targetText = STM;
		sourceBehaviour = B;

		colorTimer = new Timer ();
		alphaTimer = new Timer ();
	}

	public void AdjustColor(Color newColor, float t = 0f){
		if (colorRoutine != null)
			sourceBehaviour.StopCoroutine (colorRoutine);

		if (Helpers.IsFloatZero (t)) {
			setColor (newColor);
			return;
		}
			
		colorRoutine = sourceBehaviour.StartCoroutine (crColor (newColor, t));
	}

	public void AdjustAlpha(float newAlpha, float t = 0f){
		if (alphaRoutine != null) 
			sourceBehaviour.StopCoroutine (alphaRoutine);

		if (Helpers.IsFloatZero (t)) {
			setAlpha (newAlpha); 
			return;
		}
		
		alphaRoutine = sourceBehaviour.StartCoroutine (crAlpha (newAlpha, t));
	}

	private IEnumerator crColor(Color newColor, float t){
		startColor = TargetColor;
		colorTimer.Clear ();
		colorTimer.Begin (t);

		while (colorTimer.IsRunning) {
			setColor (Color.Lerp (startColor, newColor, colorTimer.Percentage));
			yield return null;
		}

		setColor (newColor);
	}

	private IEnumerator crAlpha(float newAlpha, float t){
		startAlpha = TargetColor.a;
		alphaTimer.Clear ();
		alphaTimer.Begin (t);

		while (alphaTimer.IsRunning) {
			setAlpha (Helpers.Lerp (startAlpha, newAlpha, alphaTimer.Percentage));
			yield return null;
		}

		setAlpha (newAlpha);
	}

	private void setColor(Color newColor){

		C = TargetColor;
		A = TargetColor.a;
		C = newColor;
		C.a = A;
		TargetColor = C;
	}

	private void setAlpha(float newAlpha){
		C = TargetColor;
		C.a = newAlpha;
		TargetColor = C;
	}

	public Color TargetColor{
		get{
			if (targetSprite == null && targetImage == null && targetRawImage == null && targetText == null) {
				return Color.black;
			}

			if (targetSprite != null)
				return targetSprite.color;

			if (targetImage != null)
				return targetImage.color;

			if (targetRawImage != null)
				return targetRawImage.color;

			if (targetText != null)
				return targetText.color;

			return Color.black;
		}
		set{
			if (targetSprite == null && targetImage == null && targetRawImage == null&& targetText == null) {
				return;
			}

			if (targetSprite != null)
				targetSprite.color = value;

			if (targetImage != null)
				targetImage.color = value;

			if (targetRawImage != null)
				targetRawImage.color = value;

			if (targetText != null) {
				targetText.color = value;
				targetText.Rebuild ();
			}
		}
	}

	public bool IsFading{
		get{
			return (alphaTimer.IsRunning || colorTimer.IsRunning);
		}
	}

    public float Alpha {
        get { return TargetColor.a; }
        set { AdjustAlpha(value); }
    }
}
