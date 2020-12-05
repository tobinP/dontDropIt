using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour
{
	public bool on = false;
	private LineRenderer lineRenderer;
	private Renderer otherRenderer;

	void Start()
	{
		lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.SetPosition(1, transform.position);
		lineRenderer.forceRenderingOff = true;
	}

	void Update()
	{
		lineRenderer.SetPosition(0, transform.position);

		if (on)
		{
			EmitBeam();
		}
	}

	public void EmitBeam()
	{
		lineRenderer.forceRenderingOff = false;
		if (Physics.Raycast(transform.position, transform.forward, out var hit))
		{
			lineRenderer.SetPosition(1, hit.point);
			if (hit.transform.CompareTag(C.interactable))
			{
				hit.collider.SendMessage(C.turnOnHighlight);
			}
		}
	}

	public void PullObject()
	{
		lineRenderer.forceRenderingOff = true;
		on = false;
		if (Physics.Raycast(transform.position, transform.forward, out var hit))
		{
			if (hit.transform.CompareTag(C.interactable))
			{
				hit.collider.SendMessage(C.getOverHere, transform.position);
			}
		}
	}
}
