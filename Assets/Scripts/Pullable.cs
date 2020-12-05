using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [RequireComponent(typeof(Rigidbody))]
public class Pullable : MonoBehaviour
{
	public int pullForce = 400;
	public int pushForce = 400;
	private Rigidbody rbody;
	private Renderer ren;
	private bool timerIsRunning;
	private float timeRemaining = 0;

	void Start()
	{
		rbody = GetComponent<Rigidbody>();
		ren = GetComponent<Renderer>();
	}

	void Update()
	{
		if (timerIsRunning)
		{
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
		var direction = target - transform.position;
		rbody.AddForce(direction * pullForce);
	}

	public void Push(Vector3 direction)
	{
		rbody.AddForce(direction * pushForce);
	}
}
