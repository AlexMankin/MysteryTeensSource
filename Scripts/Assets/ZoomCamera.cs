using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ZoomCamera : MonoBehaviour {

	public AnimationCurve ZoomCurve;
	private static Camera cam;

	private static Timer zoomTimer;

	private static ZoomCamera instance;

	private static CanvasScaler canvasScaler;

	private static float baseOrthoSize, zoomMultiplier, startingZoom, targetZoom;

    //CONSTANTS
    public const float STANDARD_ORTHO_SIZE = 135f;
	private static readonly float ORTHO_SIZE_THRESHOLD = 12f;
	private static readonly string UI_CANVAS_TAG = "UICanvas", CURSOR_CANVAS_TAG = "CursorCanvas";


	void Awake(){
		if (instance != null && instance != this) {
			Destroy (gameObject);
			return;
		}

		instance = this;
		GameObject.DontDestroyOnLoad (gameObject);

		cam = GetComponent<Camera> ();
		zoomTimer = new Timer ();

		//AdjustToResolution ();
		//SetZoom (1f);

		//StartCoroutine (crSetCanvasScaler ());

		//ScreenAdjuster.OnResolutionChanged += AdjustToResolution;

	}

	void OnDestroy(){
		//ScreenAdjuster.OnResolutionChanged -= AdjustToResolution;
	}

	// Update is called once per frame
	void Update () {
				
		//Because Unity curves HATE evaluating to nice numbers
		//float curveEvaluation = (zoomTimer.Percentage > .99999f) ? 1f : ZoomCurve.Evaluate (zoomTimer.Percentage);
		
		//zoomMultiplier = startingZoom + curveEvaluation * (targetZoom - startingZoom);

		//cam.orthographicSize = baseOrthoSize * zoomMultiplier;

	}

	public static void SetZoom(float z, float t){
		if (z <= 0f) {
			Debug.LogError ("Camera zoom must be a positive value.");
			return;
		}

		zoomTimer.Begin (t);
		startingZoom = zoomMultiplier;
		targetZoom = 1/z;
	}
	public static void SetZoom(float z){
		SetZoom (z, 0f);
	}

    public static void UpdateLetterbox() {

        float newOrthoSize = STANDARD_ORTHO_SIZE;

        if (Screen.fullScreen) {
            newOrthoSize = cam.orthographicSize;
            float height = 2f * cam.orthographicSize;
            float width = height * cam.aspect;
            Debug.Log(width);


            float diff = ((480f - (width)) / 2f);
            Debug.Log("Difference: " + diff);

            newOrthoSize += (diff / cam.aspect);
            Debug.Log("new size: " + newOrthoSize);
        }
        
        cam.orthographicSize = newOrthoSize > 0f ? newOrthoSize : STANDARD_ORTHO_SIZE;
    }

	private IEnumerator crSetCanvasScaler(){
		GameObject O;
		while (canvasScaler == null){
			O = GameObject.FindGameObjectWithTag (UI_CANVAS_TAG);
			if (O != null) {
				canvasScaler = O.GetComponent<CanvasScaler> ();
				O.GetComponent<Canvas> ().worldCamera = ZoomCamera.Cam;
			}

			O = GameObject.FindGameObjectWithTag (CURSOR_CANVAS_TAG);
			if (O != null) {
				O.GetComponent<Canvas> ().worldCamera = ZoomCamera.Cam;
			}

            

            yield return null;
		}
	}

	public static float HeightInUnits{ get { return SceneCamera.Cam.orthographicSize * 2f; } }
	public static float WidthInUnits{ get { return (SceneCamera.Cam.orthographicSize * Cam.aspect) * 2f;} }
	public static float LeftBorderInScene{get{return SceneCamera.Cam.transform.position.x - (WidthInUnits / 2f);}}
	public static float BottomBorderInScene{get{return SceneCamera.Cam.transform.position.y - (HeightInUnits / 2f);}}
	public static float RightBorderInScene{ get { return LeftBorderInScene + WidthInUnits; } }
	public static float TopBorderInScene{get{ return BottomBorderInScene + HeightInUnits;}}






	public static float UIPixelsPerUnit{
		get {
			if (canvasScaler == null)
				return 0f;

			return canvasScaler.referencePixelsPerUnit * (Screen.width / canvasScaler.referenceResolution.x);
		}
	}

	public static float ResolutionRatio{
		get{
			if (canvasScaler == null)
				return 1;
			
			return (Screen.height / canvasScaler.referenceResolution.y);
		}
	}

	public static ZoomCamera Instance{
		get { return instance; }
	}

	public static Camera Cam{
        get {
            if (cam != null)
                return cam;

            return GameObject.FindGameObjectWithTag("ZoomCamera")?.GetComponent<Camera>();
        }
    }

}
