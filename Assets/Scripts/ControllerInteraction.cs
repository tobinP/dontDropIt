using UnityEngine;
using System.Collections.Generic;
using WebXR;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(FixedJoint))]
[RequireComponent(typeof(WebXRController))]
public class ControllerInteraction : MonoBehaviour
{
	public LineMaker lineMaker;
	private FixedJoint attachJoint;
	private Rigidbody currentRigidBody;
	private List<Rigidbody> contactRigidBodies = new List<Rigidbody>();
	private WebXRController controller;
	private Vector3 lastPosition;
	private Quaternion lastRotation;
	private Animator animator;

	void Awake()
	{
		attachJoint = GetComponent<FixedJoint>();
		animator = GetComponent<Animator>();
		controller = GetComponent<WebXRController>();
	}

	void Update()
	{

		if (controller.GetButtonDown("Trigger"))
			Pickup();

		if (controller.GetButtonUp("Trigger"))
			Drop();

		if (lineMaker != null)
		{
			if (controller.GetButtonDown("Grip"))
			{
				// lineMaker.DrawLine();
				lineMaker.buttonDown = true;
			}

			if (controller.GetButtonUp("Grip"))
			{
				lineMaker.PullObject();
			}
		}

		// Use the controller button or axis position to manipulate the playback time for hand model.
		var normalizedTime = controller.GetButton("Trigger") ? 1 : controller.GetAxis("Grip");
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
		if (!other.gameObject.CompareTag("Interactable"))
			return;

		contactRigidBodies.Add(other.attachedRigidbody);
	}

	void OnTriggerExit(Collider other)
	{
		if (!other.gameObject.CompareTag("Interactable"))
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

		currentRigidBody = null;
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
