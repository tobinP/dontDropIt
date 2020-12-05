using UnityEngine;
using System.Collections.Generic;
using WebXR;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(FixedJoint))]
[RequireComponent(typeof(WebXRController))]
public class ControllerInteraction : MonoBehaviour
{
	public int launchForce = 400;
	private Rigidbody currentRigidBody;
	private List<Rigidbody> contactRigidBodies = new List<Rigidbody>();
	private Vector3 lastPosition;
	private Quaternion lastRotation;
	private FixedJoint attachJoint;
	private Animator animator;
	private WebXRController controller;
	private Beam beam;

	void Awake()
	{
		attachJoint = GetComponent<FixedJoint>();
		animator = GetComponent<Animator>();
		controller = GetComponent<WebXRController>();
		beam = GetComponent<Beam>();
	}

	void Update()
	{
		if (currentRigidBody && controller.GetButtonDown(C.grip))
		{
			Drop();
			currentRigidBody.AddForce(transform.forward.normalized * launchForce);
			currentRigidBody = null;
		}

		if (controller.GetButtonDown(C.trigger))
			Pickup();

		if (controller.GetButtonUp(C.trigger))
		{
			Drop();
			currentRigidBody = null;
		}

		if (beam != null)
		{
			if (controller.GetButtonDown(C.grip))
			{
				beam.on = true;
			}

			if (controller.GetButtonUp(C.grip))
			{
				beam.PullObject();
			}
		}

		// Use the controller button or axis position to manipulate the playback time for hand model.
		var normalizedTime = controller.GetButton(C.trigger) ? 1 : controller.GetAxis(C.grip);
		animator.Play("Take", -1, normalizedTime);
	}

	void FixedUpdate()
	{
		if (!currentRigidBody) return;

		lastPosition = currentRigidBody.position;
		lastRotation = currentRigidBody.rotation;
	}

	void OnTriggerEnter(Collider other)
	{
		if (!other.gameObject.CompareTag(C.interactable))
			return;

		contactRigidBodies.Add(other.attachedRigidbody);
	}

	void OnTriggerExit(Collider other)
	{
		if (!other.gameObject.CompareTag(C.interactable))
			return;

		contactRigidBodies.Remove(other.attachedRigidbody);
	}

	private void Pickup()
	{
		currentRigidBody = GetNearestRigidBody();

		if (!currentRigidBody)
			return;

		currentRigidBody.MovePosition(transform.position);
		attachJoint.connectedBody = currentRigidBody;

		lastPosition = currentRigidBody.position;
		lastRotation = currentRigidBody.rotation;
	}

	private void Drop()
	{
		if (!currentRigidBody)
			return;

		attachJoint.connectedBody = null;

		currentRigidBody.velocity = (currentRigidBody.position - lastPosition) / Time.deltaTime;

		var deltaRotation = currentRigidBody.rotation * Quaternion.Inverse(lastRotation);
		float angle;
		Vector3 axis;
		deltaRotation.ToAngleAxis(out angle, out axis);
		angle *= Mathf.Deg2Rad;
		currentRigidBody.angularVelocity = axis * angle / Time.deltaTime;
	}

	private Rigidbody GetNearestRigidBody()
	{
		Rigidbody nearestRigidBody = null;
		float minDistance = float.MaxValue;
		float distance;

		foreach (Rigidbody contactBody in contactRigidBodies)
		{
			distance = (contactBody.transform.position - transform.position).sqrMagnitude;

			if (distance < minDistance)
			{
				minDistance = distance;
				nearestRigidBody = contactBody;
			}
		}

		return nearestRigidBody;
	}
}
