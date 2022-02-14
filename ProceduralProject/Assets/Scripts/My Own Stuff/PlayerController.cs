using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	// UI
	[Header("Player GUI")]
	public Canvas playerUI;

	// Exposed variables
	[Header("Movement settings")]
	public float walkSpeed = 8;
	public float runSpeed = 14;
	public float jumpForce = 20;
	public float vSmoothTime = 0.1f;
	public float airSmoothTime = 0.5f;
	public float stickToGroundForce = 0;
	public float gravityConst = -15.0f;

	public float jetpackForce = 10;
	public float jetpackDuration = 2;
	public float jetpackRefuelTime = 2;
	public float jetpackRefuelDelay = 2;

	[Header("Mouse settings")]
	public float mouseSensitivityMultiplier = 1;
	public float mouseSensitivity = 5;
	public float maxMouseSmoothTime = 0.3f;
	public float mouseSmoothing = 0.05f;
	public Vector2 pitchMinMax = new Vector2(-40, 85);

	[Header("Other")]
	public float mass = 70;
	public LayerMask walkableMask;
	public Transform feet;

	// Private
	Rigidbody rb;

	float yaw;
	float pitch;
	float smoothYaw;
	float smoothPitch;

	float yawSmoothV;
	float pitchSmoothV;

	Vector3 targetVelocity;
	Vector3 cameraLocalPos;
	Vector3 smoothVelocity;
	Vector3 smoothVRef;

	// Jetpack
	bool usingJetpack;
	float jetpackFuelPercent = 1;
	float lastJetpackUseTime;

	Planet closestPlanet;
	public Planet[] planets;

	Camera cam;

	bool paused = false;

	void Awake()
	{
		cam = GetComponentInChildren<Camera>();
		cameraLocalPos = cam.transform.localPosition;
		InitRigidbody();

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		playerUI.enabled = false;
	}

    void InitRigidbody()
	{
		rb = GetComponent<Rigidbody>();
		rb.interpolation = RigidbodyInterpolation.Interpolate;
		rb.useGravity = false;
		rb.isKinematic = false;
		rb.mass = mass;
	}

	void Update()
	{
		HandleMovement();
	}

	public void ExitGame()
    {
		Application.Quit();
    }

	public void ResetPlayer() // called by UI button
    {
		transform.position = new Vector3(0, 140);
    }

	void HandleMovement()
	{
		if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
		{
			paused = !paused;
			if (paused)
            {
				Time.timeScale = 0;
				playerUI.enabled = true;
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
			else
            {
				Time.timeScale = 1;
				playerUI.enabled = false;
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
		}

		if (Time.timeScale == 0)
		{
			return;
		}
		// Look input
		yaw += Input.GetAxisRaw("Mouse X") * mouseSensitivity / 10 * mouseSensitivityMultiplier;
		pitch -= Input.GetAxisRaw("Mouse Y") * mouseSensitivity / 10 * mouseSensitivityMultiplier;
		pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
		float mouseSmoothTime = Mathf.Lerp(0.01f, maxMouseSmoothTime, mouseSmoothing);
		smoothPitch = Mathf.SmoothDampAngle(smoothPitch, pitch, ref pitchSmoothV, mouseSmoothTime);
		float smoothYawOld = smoothYaw;
		smoothYaw = Mathf.SmoothDampAngle(smoothYaw, yaw, ref yawSmoothV, mouseSmoothTime);

		cam.transform.localEulerAngles = Vector3.right * smoothPitch;
		transform.Rotate(Vector3.up * Mathf.DeltaAngle(smoothYawOld, smoothYaw), Space.Self);

		// Movement
		bool isGrounded = IsGrounded();
		Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
		bool running = Input.GetKey(KeyCode.LeftShift);
		targetVelocity = transform.TransformDirection(input.normalized) * ((running) ? runSpeed : walkSpeed);
		smoothVelocity = Vector3.SmoothDamp(smoothVelocity, targetVelocity, ref smoothVRef, (isGrounded) ? vSmoothTime : airSmoothTime);

		if (isGrounded)
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				rb.AddForce(transform.up * jumpForce, ForceMode.VelocityChange);
				isGrounded = false;
			}
			else
			{
				// Apply small downward force to prevent player from bouncing when going down slopes
				rb.AddForce(-transform.up * stickToGroundForce, ForceMode.VelocityChange);
			}
		}
		else
		{
			// Press (and hold) spacebar while above ground to engage jetpack
			if (Input.GetKeyDown(KeyCode.Space))
			{
				usingJetpack = true;
			}
		}

		if (usingJetpack && Input.GetKey(KeyCode.Space) && jetpackFuelPercent > 0)
		{
			lastJetpackUseTime = Time.time;
			jetpackFuelPercent -= Time.deltaTime / jetpackDuration;
			rb.AddForce(transform.up * jetpackForce, ForceMode.Acceleration);
		}
		else
		{
			usingJetpack = false;
		}

		// Refuel jetpack
		if (Time.time - lastJetpackUseTime > jetpackRefuelDelay)
		{
			jetpackFuelPercent = Mathf.Clamp01(jetpackFuelPercent + Time.deltaTime / jetpackRefuelTime);
		}
	}

	bool IsGrounded()
	{
		// Sphere must not overlay terrain at origin otherwise no collision will be detected
		// so rayRadius should not be larger than controller's capsule collider radius
		const float rayRadius = .3f;
		const float groundedRayDst = .2f;
		bool grounded = false;

		if (closestPlanet)
		{
			RaycastHit hit;
			Vector3 offsetToFeet = (feet.position - transform.position);
			Vector3 rayOrigin = rb.position + offsetToFeet + transform.up * rayRadius;
			Vector3 rayDir = -transform.up;

			grounded = Physics.SphereCast(rayOrigin, rayRadius, rayDir, out hit, groundedRayDst, walkableMask);
		}

		return grounded;
	}

	void FixedUpdate()
	{
		Vector3 gravityOfNearestBody = Vector3.zero;
		float nearestSurfaceDst = float.MaxValue;

		// Gravity
		foreach (Planet planet in planets)
		{
			float sqrDst = (rb.position - planet.transform.position).sqrMagnitude;
			Vector3 forceDir = (rb.position - planet.transform.position).normalized;
			Vector3 acceleration = forceDir * gravityConst * planet.shapeSettings.planetRadius / sqrDst;
			rb.AddForce(acceleration, ForceMode.Acceleration);

			float dstToSurface = Mathf.Sqrt(sqrDst) - planet.shapeSettings.planetRadius;

			// Find body with strongest gravitational pull 
			if (dstToSurface < nearestSurfaceDst)
			{
				nearestSurfaceDst = dstToSurface;
				gravityOfNearestBody = acceleration;
				closestPlanet = planet;
			}
		}

		// Rotate to align with gravity up
		Vector3 gravityUp = -gravityOfNearestBody.normalized;
		rb.rotation = Quaternion.FromToRotation(transform.up, gravityUp) * rb.rotation;

		// Move
		rb.MovePosition(rb.position + smoothVelocity * Time.fixedDeltaTime);
	}

	public void SetVelocity(Vector3 velocity)
	{
		rb.velocity = velocity;
	}
}