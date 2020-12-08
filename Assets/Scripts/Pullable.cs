using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [RequireComponent(typeof(Rigidbody))]
public class Pullable : MonoBehaviour
{
	public int pullForce = 400;
	public int pushForce = 400;
	public Color[] colors;
	private Rigidbody rbody;
	private Renderer ren;
	private bool timerIsRunning;
	private float timeRemaining = 0;
	private int consectutiveCatches = 0;
	private bool isOnTheFloor = true;

	void Start()
	{
		rbody = GetComponent<Rigidbody>();
		ren = GetComponent<Renderer>();
		ren.material.SetColor(C.baseColor, colors[0]);
	}

	void Update()
	{
		if (!timerIsRunning) return;

		if (timeRemaining > 0)
		{
			timeRemaining -= Time.deltaTime;
		}
		else
		{
			timeRemaining = 0;
			timerIsRunning = false;
			ren.material.SetFloat(C.highlightOn, 0);
		}
	}

	void OnCollisionEnter(Collision other)
	{

		if (other.gameObject.CompareTag(C.floor))
		{
			isOnTheFloor = true;
			Dropped();
		}
	}

	void OnCollisionExit(Collision other)
	{
		if (other.gameObject.CompareTag(C.floor))
		{
			isOnTheFloor = false;
		}
	}

	public void TurnOnHighlight()
	{
		if (timerIsRunning) return;

		timerIsRunning = true;
		timeRemaining = 1;
		ren.material.SetFloat(C.highlightOn, 1);
	}

	public void GetOverHere(Vector3 target)
	{
		if (!isOnTheFloor) return;

		var direction = target - transform.position;
		rbody.AddForce(direction * pullForce);
	}

	public void Caught()
	{
		consectutiveCatches++;
		ren.material.SetColor(C.baseColor, colors[consectutiveCatches]);
	}

	public void Dropped()
	{
		consectutiveCatches = 0;
		ren.material.SetColor(C.baseColor, colors[consectutiveCatches]);
	}
}
