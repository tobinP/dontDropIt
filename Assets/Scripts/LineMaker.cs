using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineMaker : MonoBehaviour
{
	public bool buttonDown = false;
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

		if (buttonDown)
		{
			DrawLine();
		}
	}

	void OnMouseDown()
	{
		DrawLine();
	}

	void OnMouseUp()
	{
		PullObject();
	}

	public void DrawLine()
	{
		lineRenderer.forceRenderingOff = false;
		if (Physics.Raycast(transform.position, transform.forward, out var hit))
		{
			lineRenderer.SetPosition(1, hit.point);
			if (hit.transform.CompareTag("Interactable"))
			{
				Debug.Log("&&& CompareTag");
				hit.collider.SendMessage("TurnOnHighlight");
			}
		}
	}

	public void PullObject()
	{
		lineRenderer.forceRenderingOff = true;
		buttonDown = false;
		if (Physics.Raycast(transform.position, transform.forward, out var hit))
		{
			if (hit.transform.CompareTag("Interactable"))
			{
				hit.collider.SendMessage("GetOverHere", transform.position);
			}
		}
	}
}
