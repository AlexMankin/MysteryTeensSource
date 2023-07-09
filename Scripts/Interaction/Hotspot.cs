using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[AddComponentMenu("Mystery Teens/Hotspot", 3)]
public class Hotspot : MonoBehaviour {

	public HotspotType Type;
	public bool FirstRowIsLabel;
	private BoxCollider2D box;

	private IGameAction action;
	private static List<Hotspot> hotspotsInScene;

	void Awake(){
		hotspotsInScene = new List<Hotspot> ();
		box = GetComponent<BoxCollider2D> ();
		action = GetComponent<IGameAction> ();

		SceneLoader.E_ReadyToLoadAfterFade += clearList;
		SceneLoader.E_LoadedBeforeFade += register;
	}

	void OnDestroy(){
		SceneLoader.E_ReadyToLoadAfterFade -= clearList;
		SceneLoader.E_LoadedBeforeFade -= register;
	}

	static void clearList(){
		hotspotsInScene.Clear ();
	}

	void register(){
		hotspotsInScene.Add (this);
	}

	void onSelectHotspot(Hotspot H){
		if (H == this) {
			if (action != null) {
                action.OnInspect();
			}
		}
	}

	public static List<Hotspot> HotspotsInScene{
		get{ return hotspotsInScene; }
	}

	public Rect BoundsInScene{
		get{

			if (Type != HotspotType.UI && box != null) {
				return new Rect (box.bounds.min, box.bounds.size);
			}  

			return new Rect();
		}
	}

	public bool IsActive{
		get{
			return true;
		}
	}

	public static bool AreUIHotspotsActive{
		get{
			return false;
		}
	}

	public string Label{
		get{
			if (!FirstRowIsLabel || action == null)
				return null;


			if (GetComponent<ActionBaseClass> ().Txt == null)
				return null;

			return GetComponent<ActionBaseClass> ().Txt.GetText ();
		}
	}













	/*




	//==========================================================================

	public Vector2 Size;

	public bool UseUnscaledTime;

	//public static EventClickable E_CursorEnter, E_CursorExit, E_CursorEnterButton, E_CursorExitButton;
	public static Helpers.EventVoid E_DeactivateButtons;

	private bool manualActiveState = true;

	//public Txt_HotspotLabels DefaultLabel;

	private bool touchingCursor, touchingCursorPrev;

	private IGameAction act;
	private UIAnimator animUI;
	private RectTransform rectTransform;
	private static Vector2 currentMousePosition;
	private SceneExit exit;

	private Vector2 T2;
	private Vector3 T3;

	void OldAwake(){
		act = GetComponent<IGameAction> ();
		exit = GetComponent<SceneExit> ();

		rectTransform = GetComponent<RectTransform> ();

		touchingCursor = false;
		touchingCursorPrev = false;

		E_DeactivateButtons += handleDeactivateButtons;
	}

	void OldStart(){

		box = GetComponent<BoxCollider2D> ();
		if (box == null) {
			box = gameObject.AddComponent<BoxCollider2D> ();
			if (IsUIElement) {
				box.size = GetComponent<RectTransform> ().sizeDelta;
				T2.x = 0.5f - rectTransform.pivot.x;
				T2.y = 0.5f - rectTransform.pivot.y;
				T2.x *= rectTransform.rect.width;
				T2.y *= rectTransform.rect.height;
				box.offset = T2;
			} else {
				box.size = Size;

			}

			box.isTrigger = true;
		}

		Rigidbody2D rigidBody = gameObject.AddComponent<Rigidbody2D> ();
		rigidBody.isKinematic = true;

		Substart ();
	//	MouseCursor.OnClick += handleClick;
		FB_Input.E_ButtonDown += testButtonDown;

	}

	protected virtual void Substart(){}

	void testButtonDown(FB_Button btn){
		if (btn == FB_Button.Confirm){
			//Debug.Log(gameObject.name + ": " + 
		}
	}

	void OldUpdate(){
		//if (currentMousePosition == GlobalMiscData.NullVector)
		//	currentMousePosition = MouseCursor.ScreenPosition;
		
		touchingCursorPrev = touchingCursor;
		touchingCursor = isTouchingCursor;

		if (touchingCursor && !touchingCursorPrev) {
		//	if (E_CursorEnter != null)
		//		E_CursorEnter (this);
		} else if (!touchingCursor && touchingCursorPrev) {
		//	if (E_CursorExit != null)
			//	E_CursorExit (this);
		}

		currentMousePosition = GlobalMiscData.NullVector;
	}

	void OldOnDestroy(){
		E_DeactivateButtons -= handleDeactivateButtons;
	}

	void handleClick(Hotspot H){
		if (H == this && IsClickable) {
			if (act != null) {
				IGA.Run (act.SelectAction ());
			} else if (exit != null) {
				IGA.Run (exit.LoadSceneAction());
			}
		}
	}

	void handleDeactivateButtons(){
		SetActive (false);
	}

	public void SetActive(bool active){
		manualActiveState = active;
	}

	public static void DeactivateVisibleButtons(){
		if (E_DeactivateButtons != null)
			E_DeactivateButtons ();
	}

	public AnimCursor GetHighlightAnimation(){
		if (IsUIElement)
			return AnimCursor.UISelect;
		
		if (Type == HotspotType.Actor)
			return AnimCursor.Speech;
		else
			return AnimCursor.Inspect;
	}

	public bool IsUIElement{
		get {
			return GetComponent<RectTransform> () != null;
		}
	}

	private bool isTouchingCursor{
		get {
			return ((currentMousePosition.x >= LeftBorder && currentMousePosition.x <= RightBorder) 
				&& (currentMousePosition.y <= TopBorder && currentMousePosition.y >= BottomBorder)
				&& IsClickable);
		}
	}

	public RectTransform RT{
		get {
			return rectTransform;
		}
	}

	public bool IsClickable{
		get {
			bool ret = true;

			if (!isActiveAndEnabled)
				ret = false;

			if (IsAnimating)
				ret = false;

			if (!gameObject.activeSelf)
				ret = false;

			if (SceneLoader.IsLoading)
				ret = false;

			if (!manualActiveState)
				ret = false;

			return ret;
		}
	}

	public bool IsAnimating{
		get{
			if (!IsUIElement)
				return false;

			return animUI.IsAnimating;
		}
	}

	public static bool ButtonsAreActive{
		get { return false; }
	}

	public float UnitWidth{
		get{
			return box.size.x;
		}
	}

	public float UnitHeight{
		get{
			return box.size.y;
		}
	}

	public Vector2 BoxOffset{
		get{
			return box.offset;
		}
	}

	public float LeftBorder{
		get{
			T3.x = box.bounds.min.x;
			if (IsUIElement) {
				return ZoomCamera.Cam.WorldToScreenPoint (T3).x;
			} else {
				//return T3.x / ScreenAdjuster.ScreenToWorldRatio / GlobalMiscData.PPU;
				return SceneCamera.Cam.WorldToScreenPoint(T3).x  * GlobalMiscData.AspectRatio;
			}

			// (IsUIElement ? ZoomCamera.Cam : SceneCamera.Cam).WorldToScreenPoint (T3).x;
		}
	}

	public float RightBorder{
		get{
			T3.x = box.bounds.max.x;
			if (IsUIElement)
				return ZoomCamera.Cam.WorldToScreenPoint (T3).x;
			else {
				return SceneCamera.Cam.WorldToScreenPoint (T3).x * GlobalMiscData.AspectRatio;
			}
			//return (IsUIElement ? ZoomCamera.Cam : SceneCamera.Cam).WorldToScreenPoint (T3).x;

		}
	}

	public float TopBorder{
		get{
			T3.y = box.bounds.max.y;
			if (IsUIElement)
				return ZoomCamera.Cam.WorldToScreenPoint (T3).y;
			else {
				return SceneCamera.Cam.WorldToScreenPoint (T3).y * GlobalMiscData.AspectRatio;
			}
			//return (IsUIElement ? ZoomCamera.Cam : SceneCamera.Cam).WorldToScreenPoint (T3).y;

		}
	}

	public float BottomBorder{
		get{
			T3.y = box.bounds.min.y;
			if (IsUIElement)
				return ZoomCamera.Cam.WorldToScreenPoint(T3).y;
			else{
				return SceneCamera.Cam.WorldToScreenPoint (T3).y * GlobalMiscData.AspectRatio;
			}
			//return (IsUIElement ? ZoomCamera.Cam : SceneCamera.Cam).WorldToScreenPoint (T3).y;

		}
	}	

	public string Label{
		get{
			/*
			Txt_HotspotLabels ID = Txt_HotspotLabels._NULL;
			if (AttachedSequence != null)
				ID = AttachedSequence.Label;

			if (ID == Txt_HotspotLabels._NULL)
				ID = DefaultLabel;

			if (ID != Txt_HotspotLabels._NULL) {
				return FB_Text.GetText (ID).Text;
			}


			return null;
		}
	}

	public ActionBaseClass AttachedAction{
		get{
			return GetComponent<ActionBaseClass>();
		}
	}
	*/
}

public enum HotspotType{
	Environment,
	Actor,
	Deduction,
	UI
}