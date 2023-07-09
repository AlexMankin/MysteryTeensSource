using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer {

	public bool UsingUnscaledTime;

	private float startTime, duration, pauseTime;
	private bool isPaused, currentlyRunning;

	public Timer(){
		Clear ();
	}
	public Timer(float t){
		Clear ();
		Begin (t);
	}
	public Timer(float t, bool useUnscaledTime){
		Clear ();
		Begin (t, useUnscaledTime);
	}

	public Timer Begin(float t){
		if (IsRunning)
			Clear ();

		currentlyRunning = true;
		startTime = CurrentTime;
		duration = t;

		return this;
	}
	public Timer Begin(float t, bool useUnscaledTime){
		UsingUnscaledTime = useUnscaledTime;
		return Begin (t);
	}
	/// Begin with the previous duration used.
	public void Begin(){
		if (duration >= 0) {
			Begin (duration);
		} else {
			Debug.LogError ("Timer has not been given a duration.");
		}
	}
		
	public bool Run(float t, bool useUnscaledTime = false){
		if (IsRunning)
			return true;

		if (currentlyRunning) {
			currentlyRunning = false;
			return false;
		}

		Begin (t, useUnscaledTime);
		return true;
	}

	public void Pause(){
		pauseTime = CurrentTime;
		isPaused = true;
	}

	public void Resume(){
		startTime += CurrentTime - pauseTime;
		isPaused = false;
	}

	public void Reset(){
		Begin ();
	}

	public void Clear(){
		startTime = -1f;
		duration = -2f;
		pauseTime = 0f;
		isPaused = false;
	}

	public void Complete(){
		startTime = CurrentTime - duration;
		pauseTime = startTime;
	}

	public float Percentage{
		get{ 
			if (IsComplete) {
				return 1f;
			}

			return Mathf.Clamp((CurrentTime - startTime) / duration, 0f, 1f);
		}
	}

	public float PercentageInverse{
		get { return 1 - Percentage; }
	}
	
	public float Elapsed{
		get { return Mathf.Clamp(CurrentTime - startTime, 0f, duration); }
	}

	public float Remaining{
		get { return Mathf.Max(0f, (startTime + duration) - Elapsed);}
	}

	public bool IsRunning{
		get { return !IsComplete && !isPaused && duration >= 0f; }
	}

	public bool IsComplete{
		get {
			if (duration < 0f && startTime < 0f)
				return false;
		
			if (Helpers.IsFloatZero(duration))
				return true;

			return CurrentTime - startTime > duration;
		}
	}

	public float CurrentTime{
		get{
			return (UsingUnscaledTime ? Time.unscaledTime : Time.time);
		}
	}


}
