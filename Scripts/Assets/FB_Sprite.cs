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
using UnityEngine.UI;

public class FB_Sprite : MonoBehaviour {
    //Attaches an image to the object and allows controls it by assigning AnimDatas
    public AnimData DefaultAnimation;
    public Sprite DefaultSprite;
    public AnimData SecondaryAnimation;
    public AnimData[] additionalAnimations;
        
    public float AnimationSpeedMultiplier = 1f;
    public bool KillAfterNextLoop;
    public bool UseUnscaledTime;

    public bool flipX, flipY;
    [SerializeField] private bool setOrderManually;
    public bool SetOrderManually {
        get {
            return setOrderManually;
        }
        set {
            setOrderManually = value;
            sorter.SortMe = !value;

            if (value)
                SortingOrder = orderInLayer;

        }
    }
    public int orderInLayer;
    public int sortOffset;
    public int SortingOrder {
        get => render.sortingOrder;
        set {
            if (setOrderManually) {
                if (render != null) render.sortingOrder = value;
            }
        }
    }

    

    public Actor actor => GetComponent<Actor>();
    [SerializeField] private SpriteLayer layer = SpriteLayer.Sprite;

    public Color DefaultTint = Color.white;
    [Range(0, 1)] public float DefaultAlpha = 1f;

    private ColorAdjustment colorAdjust;
    private Image img;

    private RectTransform rectTransform => GetComponent<RectTransform>();
    private SpriteSorter sorter;
    public delegate void EventHandlerSprite(FB_Sprite S);
    public static event EventHandlerSprite OnLoop;
    public static event EventHandlerSprite OnDisable;
    public static event EventHandlerSprite OnComplete;

    GameObject rendererObject;

    private bool hasAwoken;
    private AnimData anim, prevAnim;
    private Sprite sprite, prevSprite;
    private AnimFrame frame;
    private SpriteRenderer render;
    private int index, loopIndex;

    private PixelStabilizer stabilizer;

    private Coroutine cr;

    private bool isPaused;

    private Color C;

    //Constants
    public const float DEFAULT_TIME_BETWEEN_SPRITES = 1f;

    void Awake() {

        if (!IsCanvasObject) {
            //If we have a renderer for visual reference, get rid of it
            render = GetComponent<SpriteRenderer>();
            if (render != null) {
                GameObject.Destroy(render);
            }

            //Create the rendere we're actually using
            render = createRenderer();

            render.sortingLayerName = layer.ToString();

            if (setOrderManually)
                render.sortingOrder = orderInLayer;

            render.flipX = flipX;
            render.flipY = flipY;
            FlipX = flipX;
            FlipY = flipY;

            colorAdjust = new ColorAdjustment(render, this);

            stabilizer = Helpers.AddOrGetComponent<PixelStabilizer>(gameObject);
            stabilizer.useOwnTransform = true;

            
        }
        else {
            img = GetComponent<Image>();
            colorAdjust = new ColorAdjustment(img, this);
            rendererObject = gameObject;
        }

        Vector3 t3 = transform.localScale;
        transform.localScale = Vector3.one;
        rendererObject.transform.localScale = t3;

        Tint = DefaultTint;
        Alpha = DefaultAlpha;

        if (DefaultAnimation != null && anim == null)
            SetAnimation(DefaultAnimation);
        else if (DefaultSprite != null && anim == null)
            SetAnimation(DefaultSprite);

    }

    void OnEnable() {
        if (anim != null) {
            AnimData A = anim;
            Disable();
            SetAnimation(A);
        }
    }

    public void OnValidate() {
        if (DefaultAnimation == null && DefaultSprite == null && actor?.person == null)
            return;

        if (Application.isPlaying)
            return;

        if (GetComponent<RectTransform>() == null) {
            //Scene object
            SpriteRenderer editorRenderer = GetComponent<SpriteRenderer>();
            if (editorRenderer == null)
                editorRenderer = gameObject.AddComponent<SpriteRenderer>();

            editorRenderer.hideFlags = HideFlags.HideInInspector;

            if (DefaultAnimation != null) {
                editorRenderer.sprite = DefaultAnimation.FrameList[0].Sprite;
            }
            else if (DefaultSprite != null) {
                editorRenderer.sprite = DefaultSprite;
            }
            else if (actor.person != null) {
                editorRenderer.sprite = actor.person.animations.idleDown.FrameList[0].Sprite;
            }

            if (setOrderManually)
                editorRenderer.sortingOrder = orderInLayer;

            C = DefaultTint;
            C.a = DefaultAlpha;
            editorRenderer.color = C;
            editorRenderer.sortingLayerName = Layer.ToString();

            editorRenderer.flipX = flipX;
            editorRenderer.flipY = flipY;
            
        }
        else {
            //Canvas object
            Image tempImage = GetComponent<Image>();
            if (tempImage == null)
                tempImage = gameObject.AddComponent<Image>();

            if (DefaultAnimation != null)
                tempImage.sprite = DefaultAnimation.FrameList[0].Sprite;
            else if (DefaultSprite != null)
                tempImage.sprite = DefaultSprite;

            C = DefaultTint;
            C.a = DefaultAlpha;
            tempImage.color = C;

        }

    }

    public void SetAnimation(AnimData A, float speedMultiplier = 1f) {
        if (A == null) {
            Disable();
            return;
        }

        if (anim != null) {
            if (anim.Equals(A)) {
                return;
            }
        }

        KillAfterNextLoop = false;
        anim = A;
        sprite = null;
        AnimationSpeedMultiplier = speedMultiplier;

        if (cr != null) StopCoroutine(cr);
        cr = StartCoroutine(crRunAnimation());

    }
    public void SetAnimation(Sprite S) {
        if (S == null) {
            Disable();
            return;
        }

        if (sprite != null) {
            if (S.Equals(sprite))
                return;
        }

        KillAfterNextLoop = false;
        anim = null;
        sprite = S;

        if (cr != null) StopCoroutine(cr);

        IsVisible = true;

        if (!IsCanvasObject) render.sprite = sprite;
        else img.overrideSprite = sprite;

    }

    public void SetDefaultAnimation() {
        SetAnimation(DefaultAnimation);
    }

    public void SetSecondaryAnimation() {
        SetAnimation(SecondaryAnimation);
    }

    public void SelectListAnimation(int index) {
        if (index < 0 || index >= additionalAnimations.Length) {
            Debug.LogWarning("Animation index (" + index + ") out of range!", gameObject);
            return;
        }

        SetAnimation(additionalAnimations[index]);
    }

    private IEnumerator crRunAnimation() {
        index = 0;
        loopIndex = 0;
        IsVisible = true;

        while (anim != null) {
            frame = anim.FrameList[index];

            if (!IsCanvasObject)
                render.sprite = frame.Sprite;
            else {
                img.overrideSprite = frame.Sprite;
            }

            if (frame.SoundEffect != SFX._NULL)
                FB_Audio.PlaySFX(frame.SoundEffect);

            if (frame.LoopFromHere)
                loopIndex = index;

            if (UseUnscaledTime)
                yield return new WaitForSecondsRealtime((DEFAULT_TIME_BETWEEN_SPRITES * frame.Duration) / AnimationSpeedMultiplier);
            else
                yield return new WaitForSeconds((DEFAULT_TIME_BETWEEN_SPRITES * frame.Duration) / AnimationSpeedMultiplier);

            yield return new WaitUntil(() => !isPaused);

            if (anim != null) {

                index++;

                if (index >= anim.FrameList.Length) {
                    if (!anim.Loop || KillAfterNextLoop) {
                        if (OnComplete != null)
                            OnComplete(this);
                        Disable();
                    }
                    else {
                        Debug.Assert(loopIndex < anim.FrameList.Length, gameObject.name);
                        index = loopIndex;
                        if (OnLoop != null)
                            OnLoop(this);
                    }
                }
            }
        }
    }

    private SpriteRenderer createRenderer() {
        //Add a sprite renderer
        rendererObject = new GameObject(gameObject.name + "_Sprite");
        rendererObject.transform.SetParent(transform);
        rendererObject.transform.localPosition = Vector3.zero;

        if (!setOrderManually) {
            sorter = rendererObject.AddComponent<SpriteSorter>();
            sorter.sortYOffset = sortOffset;
        }

        return rendererObject.AddComponent<SpriteRenderer>();
    }

    public SpriteLayer Layer {
        get { return layer; }
        set {
            if (!IsCanvasObject) {
                layer = value;
                if (render != null)
                    render.sortingLayerName = value.ToString();
            }
        }
    }

    public void Disable() {
        IsVisible = false;
        if (OnDisable != null)
            OnDisable(this);

        //timer.Clear ();
        prevAnim = anim;
        anim = null;
        sprite = null;
        SetPause(false);
        Alpha = 1f;
        KillAfterNextLoop = false;

    }


    public void Enable() {
        if (!IsVisible || anim == null) {
            IsVisible = true;
            SetAnimation(prevAnim);
        }
    }

    public void SetPause(bool val) {
        isPaused = val;
    }

    public bool IsVisible {
        get { return (!IsCanvasObject) ? render.enabled : img.enabled; }
        set {
            if (!IsCanvasObject)
                render.enabled = value;
            else
                img.enabled = value;
        }
    }

    public bool FlipX {
        get { return !IsCanvasObject ? render.flipX : false; }
        set {
            if (!IsCanvasObject)
                render.flipX = value;
        }
    }

    public bool FlipY {
        get { return !IsCanvasObject ? render.flipY : false; }
        set {
            if (!IsCanvasObject)
                render.flipY = value;
        }
    }

    public AnimData CurrentAnimation {
        get { return anim; }
    }

    public bool IsPlaying {
        get { return (anim != null || sprite != null) && !isPaused; }
    }

    //public bool IsCanvasObject { get { return transform.parent?.GetComponent<RectTransform>() != null; } }
    public bool IsCanvasObject => false;

    public Vector2 Size {
        get {
            return (!IsCanvasObject) ? render.size : new Vector2(img.rectTransform.rect.width, img.rectTransform.rect.height);
        }
    }

    public AnimData GetDefaultAnimation {
        get {
            if (DefaultAnimation == null)
                return null;

            return DefaultAnimation;
        }
    }

    public Color Tint {
        get {
            return (!IsCanvasObject) ? render.color : img.color;
        }
        set {
            if (colorAdjust != null) {
                colorAdjust.AdjustColor(value);
            }
        }
    }

    public float Alpha {
        get {
            return (!IsCanvasObject) ? render.color.a : img.color.a;
        }
        set {
            if (colorAdjust != null) {
                colorAdjust.AdjustAlpha(value);
            }
        }
    }

    public ColorAdjustment Adjuster { get { return colorAdjust; } }
    public SpriteRenderer renderer { get { return render; } }
    public Image Img { get { return img; } }
    public GameObject RenderObject { get { return rendererObject; } }
}

public enum SpriteLayer
{
	UnderBG = 10,
	BG = 20,
	BelowSprite = 30,
	Sprite = 40,
	AboveSprite = 50,
	FG = 60,
    UI_AttackOption = 65,
	UI = 70,
	Cursor = 75,
    ScreenWipe = 78,
	Default = 80
}