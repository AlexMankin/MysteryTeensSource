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

[AddComponentMenu("Actor", 0)]
public class Actor : MonoBehaviour {

    //VARIABLES
    [Header("Actor")]
    public PersonData person;
    [HideInInspector] public FB_Sprite sprite;
    [HideInInspector] public Mover mover {
        get {
            if (GetComponent<Mover>() == null)
                gameObject.AddComponent<Mover>();

            return GetComponent<Mover>();
        }
    }
    [HideInInspector] public ActionBaseClass action;
    [HideInInspector] public Collider2D collider;
    [HideInInspector] public Rigidbody2D rigidBody;
    [HideInInspector] public Hotspot hotspot;
    [HideInInspector] public SuperTextMesh textMesh => GetComponent<SuperTextMesh>();
    [HideInInspector] public Vector2 InitialPosition { get; private set; }

    public float speed = 120f;
    public bool makePlayer;
    public bool defaultIgnoreCollision;
    public bool collideToInspect;
    [SerializeField] private CardinalDirection initialForward;
    [HideInInspector] public CardinalDirection forward;

    //CONSTANTS
    private const float BOX_SIZE_X = 18f, BOX_SIZE_Y = 12f, BOX_VERT_OFFSET = BOX_SIZE_Y / 2;
    private const float INSPECTION_HEIGHT = 0.25f;
    //EVENTS

    //METHODS

    protected void Awake() {

        InitialPosition = Position;

        if (sprite == null) {
            sprite = GetComponent<FB_Sprite>();
            if (sprite == null)
                sprite = gameObject.AddComponent<FB_Sprite>();
        }

        if (action == null)
            action = GetComponent<ActionBaseClass>();

        if (hotspot == null) {
            hotspot = GetComponent<Hotspot>();
            if (hotspot == null)
                hotspot = gameObject.AddComponent<Hotspot>();
        }

        if (collider == null) {
            collider = GetComponent<BoxCollider2D>();
            if (collider == null && person != null) {
                collider = gameObject.AddComponent<BoxCollider2D>();
                if (collider is BoxCollider2D) {
                    BoxCollider2D _Box = collider as BoxCollider2D;
                    _Box.offset = new Vector2(0f, BOX_VERT_OFFSET);
                    _Box.size = new Vector2(BOX_SIZE_X, BOX_SIZE_Y);
                }
            }
        }

        if (rigidBody == null) {
            rigidBody = GetComponent<Rigidbody2D>();
            if (rigidBody == null) {
                rigidBody = (collider != null ? collider.gameObject : gameObject).AddComponent<Rigidbody2D>();
            }
        }

        rigidBody.simulated = true;
        rigidBody.gravityScale = 0f;
        rigidBody.freezeRotation = true;
        rigidBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rigidBody.bodyType = RigidbodyType2D.Kinematic;

        if (collider != null ? !collider.isTrigger : false)
            IgnoreCollision = defaultIgnoreCollision;

        if (person != null && sprite != null) {
            gameObject.AddComponent<PersonAnimator>();
        }

        ActorRegistrar.E_CallForRegistration += onRegister;
    }

    protected void Start() {
        if (makePlayer)
            IsPlayer = true;

        ForwardVector = initialForward != CardinalDirection._NULL ? Helpers.CardinalToVector(initialForward) : Vector2.down;
    }

    void OnDestroy() {
        ActorRegistrar.E_CallForRegistration -= onRegister;
    }

    void OnValidate() {
        if (Application.isPlaying)
            return;

        if (person == null)
            return;

        if (person.animations.idleDown == null)
            return;

        if (sprite != null) {
            return; //An existing FB_Sprite will render itself
        }

        SpriteRenderer spr = GetComponent<SpriteRenderer>();
        if (spr == null)
            spr = gameObject.AddComponent<SpriteRenderer>();

        spr.hideFlags = HideFlags.HideInInspector;
        spr.sortingLayerName = SpriteLayer.Sprite.ToString();

        spr.sprite = person.animations.idleDown.FrameList[0].Sprite;

    }

    protected void Update() {
        if (makePlayer && !IsPlayer) {
            if (Player.PlayableActor != null)
                Player.PlayableActor.makePlayer = false;
            IsPlayer = true;
        }

        if (mover.IsMoving && !LockDirection && mover.Forward != Vector2.zero) {
            forward = mover.Forward.ToCardinal();
        }

    }

    public static Actor InstantiateActor(string name, Vector2 position) {
        Actor newActor = Instantiate(Global.Misc.ActorPrefab).GetComponent<Actor>();
        newActor.name = name;
        newActor.transform.position = new Vector3(position.x, position.y, 10f);

        return newActor;
    }
    public static Actor InstantiateActor(string name) {
        return InstantiateActor(name, Vector2.zero);
    }

    void onRegister() {
        ActorRegistrar.Register(this, person);
    }

    public void SetAnimation(AnimData newAnim, float speedMultiplier = 1f, Sprite newSprite = null) {
        if (sprite == null) {
            Debug.LogWarning("Cannot set animation for Actor " + gameObject.name + ", no FB_Sprite found.");
            return;
        }

        LockAnimation = true;

        if (newAnim != null)
            sprite.SetAnimation(newAnim, speedMultiplier);
        else
            sprite.SetAnimation(newSprite);
    }
    public void SetAnimation(Sprite newSprite) {
        SetAnimation(null, 1f, newSprite);
    }

    public void SelectListAnimation(int index) {
        sprite.SelectListAnimation(index);
        LockAnimation = true;
    }
    /// <summary>
    /// Activates this actor.
    /// </summary>
    /// <param name="attemptToSave">If true, updates Flag Zero based on functionality of attached Action.</param>
    public void On(bool attemptToSave = false) {
        IsActive = true;
        if (attemptToSave && action != null) {
            if (action.FirstFlagActivation) {
                action.SetFlag(true);
            }

            if (action.FlagZeroFunction == FlagZeroBehavior.OnSwitch) {
                action.SetFlag(true);
            }

            if (action.FlagZeroFunction == FlagZeroBehavior.OffSwitch) {
                action.SetFlag(false);
            }
        }
    }
    /// <summary>
    /// Deactivates this actor.
    /// </summary>
    /// <param name="attemptToSave">If true, updates Flag Zero based on functionality of attached Action.</param>
    public void Off(bool attemptToSave = false) {
        IsActive = false;
        if (attemptToSave && action != null) {
            if (action.FirstFlagActivation) {
                action.SetFlag(false);
            }

            if (action.FlagZeroFunction == FlagZeroBehavior.OnSwitch) {
                action.SetFlag(false);
            }

            if (action.FlagZeroFunction == FlagZeroBehavior.OffSwitch) {
                action.SetFlag(true);
            }


        }
    }

    public Ghost GetGhost(int index = 0) {
        if (person == null)
            return Ghost.Get(Global.Misc.defaultGhost, index);

        return Ghost.Get(person.personName, index);
    }

    public void WalkToGhost(AllPersons P, int index = 0, float t = 0f) {
        if (t <= 0f) {
            mover.MoveWithVelocityToPosition(Ghost.GetPosition(P, index), speed);
            //oldMover.MoveUntilCrossing(Ghost.GetPosition(P, index), speed);
        }
        else {
            mover.MoveToPositionInSeconds(Ghost.GetPosition(P, index), t);
            // oldMover.MoveToPosition(Ghost.GetPosition(P, index), t);
        }
    }
    public void WalkToGhost(int index = 0, float t = 0f) {
        WalkToGhost(person != null ? person.personName : Global.Misc.defaultGhost, index, t);
    }

    public void CrossGhost(AllPersons P, Vector2 direction, int index = 0, float speed = 140f) {
        mover.MoveWithVelocityUntilCrossing(direction, Ghost.GetPosition(P, index), speed);
    }

    public void CrossGhost(Vector2 direction, int index = 0, float speed = 140f) {
        CrossGhost(person != null ? person.personName : Global.Misc.defaultGhost, direction, index, speed);
    }

    public void WarpToGhost(AllPersons P, int index = 0) {
        Position = Ghost.GetPosition(P, index);
        forward = Ghost.Get(P, index).forward;
    }
    public void WarpToGhost(int index = 0) {
        WarpToGhost(person != null ? person.personName : Global.Misc.defaultGhost, index);
    }

    public Vector2 GhostPosition(int index = 0) {
        if (person == null)
            return Ghost.GetPosition(Global.Misc.defaultGhost, index);

        return Ghost.GetPosition(person.personName, index);
    }

    public void Face(Actor a) {
        ForwardVector = a.Position - Position;
    }
    public void Face(Vector2 pos) {
        ForwardVector = pos - Position;
    }
    public void Face(CardinalDirection direction) {
        ForwardVector = Helpers.CardinalToVector(direction);
    }

    public IEnumerator WalkCardinalToGhost(AllPersons P, Vector2 initialDirection, int index = 0, float speed = Global.DEFAULT_WALKING_SPEED) {
        CrossGhost(P, initialDirection, index, speed);
        yield return new WaitUntil(() => DoneMoving);
        CrossGhost(P, (Ghost.GetPosition(P, index) - Position).NearestCardinal(), index, speed);
        yield return new WaitUntil(() => DoneMoving);
    }

    public IEnumerator WalkCardinalToGhost(Vector2 initialDirection, int index = 0, float speed = Global.DEFAULT_WALKING_SPEED) {
        yield return WalkCardinalToGhost(person != null ? person.personName : AllPersons.NULL, initialDirection, index, speed);
    }
    public IEnumerator WalkCardinalToGhost(int index = 0, float speed = Global.DEFAULT_WALKING_SPEED) {
        yield return WalkCardinalToGhost((GetGhost(index).Position - Position).NearestCardinal(), index, speed);
    }
    public IEnumerator WalkCardinalToGhost(AllPersons P, int index = 0, float speed = Global.DEFAULT_WALKING_SPEED) {
        yield return WalkCardinalToGhost((Ghost.GetPosition(P, index) - Position).NearestCardinal(), index, speed);
    }

    public void ReleaseAnimation() {
        LockAnimation = false;
        LockDirection = false;
    }

    //PROPERTIES
    public bool IsPlayer {
        get {
            return player != null;
        }
        set {
            if (value) {
                if (player == null) {
                    gameObject.AddComponent<Player>();
                }
                rigidBody.bodyType = RigidbodyType2D.Dynamic;
            }
            else {
                if (player != null) {
                    GameObject.Destroy(player);
                }
                rigidBody.bodyType = RigidbodyType2D.Kinematic;
            }
        }
    }

    public void Hop(float h = 15f, float t = 0.25f) {
        mover.HopSprite(h, t);
        FB_Audio.PlaySFX(SFX.Squish);
    }

    public Player player { get { return GetComponent<Player>(); } }

    public Vector2 BubblePosition {
        get {
            Vector2 ret = transform.position;
            ret.x += 1.5f;
            ret.y += 2.2f;
            return ret;
        }
    }
    public Vector2 Position {
        get { return transform.position; }
        set { transform.position = value; }
    }

    public Vector3 LocalPosition {
        get { return transform.localPosition; }
        set { transform.localPosition = value; }
    }

    public Vector2 ForwardVector {
        get {
            return Helpers.CardinalToVector(forward);
        }
        set {
            forward = value.ToCardinal();
            mover.Forward = value;
        }
    }

    public bool IgnoreCollision { get { return collider != null ? collider.isTrigger : false; } set { if (collider != null) collider.isTrigger = value; } }
    public ColorAdjustment color  => sprite.Adjuster;
    public ColorAdjustment TextColorAdjuster {
        get {
            if (textMesh == null)
                return null;

            if (textAdjuster == null)
                textAdjuster = new ColorAdjustment(textMesh, this);

            return textAdjuster;
        }
    }
    private ColorAdjustment textAdjuster;

   
    private PersonAnimator personAnimator {
        get {
            PersonAnimator ret = GetComponent<PersonAnimator>();
            if (ret == null && person != null)
                return gameObject.AddComponent<PersonAnimator>();

            return ret;
        }
    }

    public bool DoneMoving => !mover.IsMoving;

    public bool LockAnimation {
        get { return personAnimator != null ? personAnimator.LockAnimation : false; }
        set { if (personAnimator != null) personAnimator.LockAnimation = value; }
    }

    public bool LockDirection {
        get { return personAnimator != null ? personAnimator.LockDirection : false; }
        set { if (personAnimator != null) personAnimator.LockDirection = value; }
    }

    public bool IsActive {
        get { return gameObject.activeSelf; }
        set { gameObject.SetActive(value); }
    }

}
