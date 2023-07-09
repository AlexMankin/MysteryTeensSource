using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
#if UNITY_EDITOR 
	using UnityEditor;
#endif

public class Helpers : MonoBehaviour
{

    //public static AssetBundle AssetBundle, AssetBundleVoice;
    public static GameObject UIObject, CursorUIObject;
    public static bool InGame = false;
    public static float GameElapsedTime;

    public delegate void EventVoid();
    public delegate IEnumerator EventIEnumerator();

    public delegate bool BoolDelegate();

    private static Vector2 T2;
    private static Vector3 T3;

    //CONSTANTS

    public static readonly string ASSETBUNDLE_BASE_NAME = "basegame";
    public static readonly string ASSETBUNDLE_EP2_NAME = "ep2";
    public static readonly string ASSETBUNDLE_BGM = "bgm";
    public static readonly string ASSETBUNDLE_SFX = "sfx";
    public static readonly string ASSETBUNDLE_ANIM = "anim";
    public static readonly string ASSETBUNDLE_SPRITE = "sprite";

    private static readonly int fps = 60;

    void Awake()
    {
		
    }

    void Update()
    {
        if (InGame)
            GameElapsedTime += Time.deltaTime;
    }

    public static string GetEnumName<T>(T name)
    {
        return typeof(T).Name;
    }

    public static int GetHighestEnumValue(Type enumType)
    {
        int val, highest = 1;

        foreach (var enumValue in Enum.GetValues(enumType))
        {
            val = (int)enumValue;
            if (val > highest)
                highest = val;
        }

        return highest;
    }

    public static bool IsFloatZero(float f)
    {
        return Mathf.Abs(f) < .0001f;
    }

    void OnDestroy()
    {
		
    }

    public static int FPS
    {
        get { return fps; }
    }

    public static T AddOrGetComponent<T>(GameObject O)
    {
        if (O.GetComponent<T>() == null)
        {
            O.AddComponent(typeof(T));
        }

        return O.GetComponent<T>();
    }

    public static void SetRigidbody(Rigidbody2D rb)
    {
        rb.simulated = true;
        rb.isKinematic = true;
        rb.gravityScale = 0;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    public static float Lerp(float a, float b, float t)
    {
        return a + (t * (b - a));
    }

    public static void StartRoutine(ref Coroutine cr, IEnumerator ie, MonoBehaviour mb)
    {
        if (cr != null)
            mb.StopCoroutine(cr);

        cr = mb.StartCoroutine(ie);

    }

    public static Vector3 SetVectorValue(Vector3 V, float val, CartesianCoordinate cc)
    {
        T3 = V;

        if (cc == CartesianCoordinate.X)
            T3.x = val;
        else if (cc == CartesianCoordinate.Y)
            T3.y = val;
        else
            T3.z = val;

        return T3;
    }
    public static Vector2 SetVectorValue(Vector2 V, float val, CartesianCoordinate cc)
    {
        T2 = V;

        if (cc == CartesianCoordinate.X)
            T2.x = val;
        else
            T2.y = val;

        return T2;
    }

    public static int NumberFromGameObjectName(string name) {
        string[] split = name.Split('(');
        int ret = -1;
        if (split.Length > 1) {
            int.TryParse(split[1].Substring(0, split[1].Length - 1), out ret);
        }

        return ret;
    }

    public static bool RandomChance(float chanceOfSuccess) {
        float diceRoll = UnityEngine.Random.Range(0f, 1f);
        return chanceOfSuccess >= diceRoll;
    }

    public static float PixelsToUnits(int numPixels) {
        return numPixels * GlobalMiscData.PPU;
    }
    //PROPERTIES

    public static bool RandomBool
    {
        get
        {
            int R = UnityEngine.Random.Range(0, 1);
            return (R == 1) ? true : false;
        }
    }

	public static Vector2 CardinalToVector(CardinalDirection C){
		switch (C) {
		case CardinalDirection.Up:
			return Vector2.up;
		case CardinalDirection.Down:
			return Vector2.down;
		case CardinalDirection.Left:
			return Vector2.left;
		case CardinalDirection.Right:
			return Vector2.right;
		default:
			return Vector2.zero;
		}
	}

	public static void SetY2(Vector2? V2, float newY){
		Vector2 T2 = new Vector2 (V2.Value.x, newY);
		V2 = T2;
	}

    public static CardinalDirection ReverseCardinalDirection(CardinalDirection initialDirection) {
        return initialDirection == CardinalDirection.Up ? CardinalDirection.Down :
            initialDirection == CardinalDirection.Down ? CardinalDirection.Up :
            initialDirection == CardinalDirection.Left ? CardinalDirection.Right :
            initialDirection == CardinalDirection.Right ? CardinalDirection.Left :
            CardinalDirection._NULL;
    }
}

public enum CartesianCoordinate
{
    X, Y, Z
}


public enum CardinalDirection {
    _NULL = 0,
    Up = 1,
    Down = 2,
    Left = 3,
    Right = 4,
}
		
//EXTENSIONS
static class ListExtensions
{
    public static void MoveToFront<T>(this List<T> list, int index)
    {
        T item = list[index];
        list.RemoveAt(index);
        list.Insert(0, item);
    }
}

static class Vector2Extensions{
	public static Vector2 NearestCardinal(this Vector2 V){
		Vector2 ret = new Vector2 ();

		if (Mathf.Abs(V.x) > Mathf.Abs(V.y)){
			ret.y = 0f;
			ret.x = (Mathf.Sign (V.x) >= 0) ? 1 : -1;
		}
		else{
			ret.x = 0f;
			ret.y = (Mathf.Sign (V.y) >= 0) ? 1 : -1;
		}

		return ret;
	}

    public static CardinalDirection ToCardinal(this Vector2 V) {
        Vector2 C = V.NearestCardinal();
        if (C == Vector2.left)
            return CardinalDirection.Left;
        if (C == Vector2.right)
            return CardinalDirection.Right;
        if (C == Vector2.down)
            return CardinalDirection.Down;
        if (C == Vector2.up)
            return CardinalDirection.Up;

        return CardinalDirection._NULL;
    }
}
static class Vector3Extensions{
	public static Vector3 Change(this Vector3 V, object newX, object newY, object newZ){
		return new Vector3 ((newX == null ? V.x : (float)newX), (newY == null ? V.y : (float)newY), (newZ == null ? V.z : (float)newZ));
	}
}

static class GameObjectExtensions {
    public static void setChildrenActive(this GameObject gameObject, bool newActive, bool recursive = false) {
        for (int i = 0; i < gameObject.transform.childCount; ++i) {
            if (recursive)
                gameObject.transform.GetChild(i).gameObject.setChildrenActive(newActive, true);

            gameObject.transform.GetChild(i).gameObject.SetActive(newActive);
        }
    }
}

[System.Serializable]
public struct IntVector2 {
    public int x;
    public int y;

    public static implicit operator Vector2(IntVector2 me) {
        Vector2 ret = new Vector2();
        ret.x = me.x;
        ret.y = me.y;
        return ret;
    }
}

public enum Axis { x, y };

