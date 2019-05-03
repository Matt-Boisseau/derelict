using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	public float groundAcceleration, airAcceleration, groundFriction, strafeFriction, forwardFriction, backwardFriction, vSpeedMax, jumpSpeed, jumpHoldGravityMultiplier;

	private float vSpeed = 0;
	private Vector3 velocity;
	private CharacterController controller;

	private void Start() {
		controller = GetComponent<CharacterController>();
	}

	private void Update() {

		// Input
		Vector3 inputVector = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

		// Direction
		Quaternion cameraRotation = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0);
		inputVector = cameraRotation * inputVector;

		if(controller.isGrounded) {

			// Acceleration
			inputVector = Vector3.ClampMagnitude(inputVector * groundAcceleration, groundAcceleration * Time.deltaTime);
			velocity += inputVector;

			// Land
			vSpeed = 0;

			// Jump
			if(Input.GetButton("Jump")) {
				vSpeed = jumpSpeed;
			}
			// Ground friction (only if not jumping immediately)
			else {
				velocity /= 1 + (groundFriction * Time.deltaTime);
			}
		}
		else {

			// Acceleration
			inputVector = Vector3.ClampMagnitude(inputVector * airAcceleration, airAcceleration * Time.deltaTime);
			velocity += inputVector;

			// Fall
			if(Input.GetButton("Jump")) {
				vSpeed -= Physics.gravity.magnitude * jumpHoldGravityMultiplier * Time.deltaTime;
			}
			else {
				vSpeed -= Physics.gravity.magnitude * Time.deltaTime;
			}
			vSpeed = Mathf.Clamp(vSpeed, -vSpeedMax, vSpeedMax);
			
			// Air friction
			Vector3 relativeVelocity = Quaternion.Inverse(cameraRotation) * velocity;
			relativeVelocity.x /= 1 + (strafeFriction * Time.deltaTime);
			relativeVelocity.z /= 1 + ((relativeVelocity.z >= 0 ? forwardFriction : backwardFriction) * Time.deltaTime);
			velocity = cameraRotation * relativeVelocity;
		}

		// Apply
		Vector3 totalVector = velocity + (Vector3.up * vSpeed);
		controller.Move(totalVector * Time.deltaTime);
	}
}
