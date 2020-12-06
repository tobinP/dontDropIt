using UnityEngine;
using System.Collections.Generic;
using WebXR;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(FixedJoint))]
[RequireComponent(typeof(WebXRController))]
public class ControllerInteraction : MonoBehaviour
{
	public int launchForce = 400;
	private Rigidbody ballRigidBody;
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
		if (ballRigidBody && controller.GetButtonDown(C.grip))
		{
			ballRigidBody.AddForce(transform.forward.normalized * launchForce);
			Drop();
			return;
		}

		if (controller.GetButtonDown(C.trigger))
		{
			Pickup();
			return;
		}

		if (controller.GetButtonUp(C.trigger))
		{
			Drop();
			return;
		}
			
		if (controller.GetButtonDown(C.grip))
		{
			beam.on = true;
			return;
		}

		if (controller.GetButtonUp(C.grip))
		{
			beam.PullObject();
			return;
		}

		// Use the controller button or axis position to manipulate the playback time for hand model.
		var normalizedTime = controller.GetButton(C.trigger) ? 1 : controller.GetAxis(C.grip);
		animator.Play("Take", -1, normalizedTime);
	}

	void FixedUpdate()
	{
		if (!ballRigidBody) return;

		lastPosition = ballRigidBody.position;
		lastRotation = ballRigidBody.rotation;
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag(C.interactable))
		{
			ballRigidBody = other.attachedRigidbody;
		}
	}

	private void Pickup()
	{
		if (!ballRigidBody) return;

		ballRigidBody.SendMessage(C.caught);
		ballRigidBody.MovePosition(transform.position);
		attachJoint.connectedBody = ballRigidBody;

		lastPosition = ballRigidBody.position;
		lastRotation = ballRigidBody.rotation;
	}

	private void Drop()
	{
		if (!ballRigidBody) return;

		attachJoint.connectedBody = null;

		ballRigidBody.velocity = (ballRigidBody.position - lastPosition) / Time.deltaTime;

		var deltaRotation = ballRigidBody.rotation * Quaternion.Inverse(lastRotation);
		float angle;
		Vector3 axis;
		deltaRotation.ToAngleAxis(out angle, out axis);
		angle *= Mathf.Deg2Rad;
		ballRigidBody.angularVelocity = axis * angle / Time.deltaTime;
		ballRigidBody = null;
	}
}
