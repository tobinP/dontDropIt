using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [RequireComponent(typeof(Rigidbody))]
public class Pullable : MonoBehaviour
{
	public int force = 400;
	private Rigidbody rbody;
	private Renderer renderer1;
	private bool timerIsRunning;
	private float timeRemaining = 0;

	void Start()
	{
		rbody = GetComponent<Rigidbody>();
		renderer1 = GetComponent<Renderer>();
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
				renderer1.material.SetFloat("HighlightOn", 0);
			}
		}
	}

	public void TurnOnHighlight()
	{
		if (timerIsRunning) return;

		timerIsRunning = true;
		timeRemaining = 1;
		renderer1.material.SetFloat("HighlightOn", 1);
	}

	public void GetOverHere(Vector3 target)
	{
		var direction = target - transform.position;
		rbody.AddForce(direction * force);
	}
}
