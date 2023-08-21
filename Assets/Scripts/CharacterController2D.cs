using BanhMy.Tools;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[RequireComponent (typeof (BoxCollider2D))]
public class CharacterController2D : MonoBehaviour
{
	public const float GRAVITY = -9.8f;
	private const float _movementDirectionThreshold = 0.0001f;

	// Used to determine which objects to collide with
	[SerializeField] private LayerMask _platformMask;
	[SerializeField] private LayerMask _oneWayPlatformMask;
	[SerializeField] private float _skinWidth;
	[SerializeField] private int _horizontalRayCount;
	[SerializeField] private int _verticalRayCount;
	[SerializeField] private float _fallMultiplier;
	[SerializeField] private bool _castRaysOnBothSides;
	[SerializeField] private float _distanceToTheGroundRayMaximumLenght = 100f;

	public CharacterControllerState State;

	private Vector2 _velocity;
	private Vector2 _lastFrameVelocity;
	private int _movementDirecion;
	private int _storedMovementDirection;
	private float _gravity = GRAVITY;
	private float _currentGravity;
	private float _overrideGravity;
	private bool _hasOverrideGravity = false;
	private bool _isGravityActive = true;
	private float _horizontalRaySpacing;
	private float _verticalRaySpacing;
	private float _distanceToTheGround;
	private BoxCollider2D _boxCollider;
	private RaycastOrigins _raycastOrigins;
	LayerMask _savePlatformMask;

	public bool IsGravityActive { get => _isGravityActive; }
	public float Gravity { get { return _hasOverrideGravity ? _overrideGravity : _gravity; } }
	public float DistanceToTheGround { get => _distanceToTheGround; }

	// Start is called before the first frame update
	private void Awake()
	{
		_boxCollider = GetComponent<BoxCollider2D>();
		State = new CharacterControllerState();
		CalculateRaySpacing();

		_savePlatformMask = _platformMask;
		_platformMask |= _oneWayPlatformMask;
	}

	public void SetGravityActive(bool active)
	{
		_isGravityActive = active;
	}

	public void SetOverrideGravity(float value)
	{
		_hasOverrideGravity = true;
		_overrideGravity = value;
	}

	public void SetForce(Vector2 force)
	{
		_velocity = force;
	}

	public void SetHorizontalForce(float force)
	{
		_velocity.x = force;
	}

	public void SetVerticalFoce(float force)
	{
		_velocity.y = force;
	}

	public IEnumerator DisableCollisionFor(float duration)
	{
		DisableCollision();
		yield return new WaitForSeconds(duration);
		EnableCollision();
	}

	public void EnableCollision()
	{
		_platformMask = _savePlatformMask;
		_platformMask |= _oneWayPlatformMask;
	}

	public void DisableCollision()
	{
		_platformMask = 0;
	}

	private void Update()
	{
		FrameInitialization();
		ApplyGravity();

		// Calculate the velocity that will be applied to the transform in this update
		Vector2 appliedVelocity = (_lastFrameVelocity + _velocity) * 0.5f * Time.deltaTime;

		UpdateRaycastOrigins();

		DetermineMovementDirection(appliedVelocity);
		if (_castRaysOnBothSides)
		{
			CastRaysToTheSides(ref appliedVelocity, -1);
			CastRaysToTheSides(ref appliedVelocity, 1);
		}
		else
		{
			if (_movementDirecion == 1)
			{
				CastRaysToTheSides(ref appliedVelocity, 1);
			}
			else
			{
				CastRaysToTheSides(ref appliedVelocity, - 1);
			}
		}

		if (appliedVelocity.y > 0)
		{
			CastRayAbove(ref appliedVelocity);
		}
		else if (appliedVelocity.y < 0)
		{
			CastRayBelow(ref appliedVelocity);
		}

		// Move the transform
		transform.Translate(appliedVelocity);

		UpdateRaycastOrigins();

		// Set state
		if (!State.WasGroundedLastFrame && State.IsCollidingBelow)
		{
			State.JustGotGrounded = true;
		}
		ComputeDistanceToTheGround();

		// Force the physic engine to synchronize physic model after making changes in transform.
		// Prevent player from constantly sinking to the ground at microseconds.
		Physics2D.SyncTransforms();
	}

	private void FrameInitialization()
	{
		_lastFrameVelocity = _velocity;
		State.WasGroundedLastFrame = State.IsCollidingBelow;
		State.WasTouchingTheCeilingLastFrame = State.IsCollidingAbove;
		State.Reset();
	}

	private void ApplyGravity()
	{
		_currentGravity = Gravity;
		if (_velocity.y < 0)
		{
			_currentGravity *= _fallMultiplier;
		}

		if (_isGravityActive)
		{
			_velocity.y += _currentGravity * Time.deltaTime;
		}
	}

	private void DetermineMovementDirection(Vector2 velocity)
	{
		_movementDirecion = _storedMovementDirection;
		if (velocity.x < -_movementDirectionThreshold)
		{
			_movementDirecion = -1;
		}
		else if (velocity.x > _movementDirectionThreshold)
		{
			_movementDirecion = 1;
		}

		_storedMovementDirection = _movementDirecion;
	}

	private void CastRaysToTheSides(ref Vector2 velocity, int rayDirection)
	{
		var movementDirection = Mathf.Sign(velocity.x);
		float rayLength = Mathf.Abs(velocity.x) + _skinWidth;
		RaycastHit2D[] sideHitsList = new RaycastHit2D[_horizontalRayCount];

		for (int i = 0; i < _horizontalRayCount; i++)
		{
			Vector2 rayOrigin = (rayDirection == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (_horizontalRaySpacing * i);
			sideHitsList[i] = BMDebug.RayCast(rayOrigin, rayDirection * transform.right, rayLength, _platformMask & ~_oneWayPlatformMask, Color.green, true);

			if (sideHitsList[i].distance > 0)
			{
				float hitAngle = Mathf.Abs(Vector2.Angle(sideHitsList[i].normal, transform.up));

				if (rayDirection < 0)
				{
					State.IsCollidingLeft = true;
				}

				if (rayDirection > 0)
				{
					State.IsCollidingRight = true;
				}

				if (movementDirection == rayDirection)
				{
					velocity.x = (sideHitsList[i].distance - _skinWidth) * rayDirection;
					SetHorizontalForce(0f);
				}
				break;
			}
		}
	}

	private void CastRayAbove(ref Vector2 velocity)
	{
		float rayLength = Mathf.Abs(velocity.y) + _skinWidth;
		bool isHitConnected = false;
		float smallestDistance = float.MaxValue;
		RaycastHit2D[] aboveHitsList = new RaycastHit2D[_verticalRayCount];

		for (int i = 0; i < _verticalRayCount; i++)
		{
			Vector2 rayOrigin = _raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (_verticalRaySpacing * i + velocity.x);
			aboveHitsList[i] = BMDebug.RayCast(rayOrigin, transform.up, rayLength, _platformMask & ~_oneWayPlatformMask, Color.cyan, true);

			if (aboveHitsList[i])
			{
				isHitConnected = true;

				if (aboveHitsList[i].distance < smallestDistance)
				{
					smallestDistance = aboveHitsList[i].distance;
				}
			}
		}

		if (isHitConnected)
		{
			velocity.y = smallestDistance - _skinWidth;

			if (State.IsGrounded && velocity.y < 0f)
			{
				velocity.x = 0f;
			}

			State.IsCollidingAbove = true;

			// Make the character fall down when touch the ceiling
			if (!State.WasTouchingTheCeilingLastFrame)
			{
				velocity.y = 0f;
			}

			SetVerticalFoce(0f);
		}
	}

	private void CastRayBelow(ref Vector2 velocity)
	{
		State.IsFalling = Mathf.Sign(velocity.y) < 0f;

		float rayLength = Mathf.Abs(velocity.y) + _skinWidth;
		RaycastHit2D[] belowHitsList = new RaycastHit2D[_verticalRayCount];
		float smallestDistance = float.MaxValue;
		bool isHitConnected = false;

		for (int i = 0; i < _verticalRayCount; i++)
		{
			Vector2 rayOrigin = _raycastOrigins.bottomLeft;
			rayOrigin += Vector2.right * (_verticalRaySpacing * i + velocity.x);
			belowHitsList[i] = BMDebug.RayCast(rayOrigin, -transform.up, rayLength, _platformMask, Color.blue, true);

			if (belowHitsList[i])
			{
				isHitConnected = true;

				if (belowHitsList[i].distance < smallestDistance)
				{
					smallestDistance = belowHitsList[i].distance;
				}
			}
		}

		if (isHitConnected)
		{

			State.IsFalling = false;
			State.IsCollidingBelow = true;
			// If is jumping;
			if (_velocity.y > 0f)
			{
				State.IsCollidingBelow = false;
			}
			// If not, adjust the velocity based on raycast hit
			else
			{
				velocity.y = -smallestDistance + _skinWidth;
				SetVerticalFoce(0f);
			}
		}
		
	}

	private void ComputeDistanceToTheGround()
	{
		if (_distanceToTheGroundRayMaximumLenght <= 0f)
		{
			_distanceToTheGround = -1;
			return;
		}

		if (State.IsGrounded)
		{
			_distanceToTheGround = 0f;
			return;
		}

		float rayLength = _distanceToTheGroundRayMaximumLenght + _skinWidth;
		RaycastHit2D[] belowHitsList = new RaycastHit2D[_verticalRayCount];
		float smallestDistance = float.MaxValue;
		bool isHitConnected = false;

		for (int i = 0; i < _verticalRayCount; i++)
		{
			Vector2 rayOrigin = _raycastOrigins.bottomLeft;
			rayOrigin += Vector2.right * (_verticalRaySpacing * i);
			belowHitsList[i] = BMDebug.RayCast(rayOrigin, -transform.up, rayLength, _platformMask, Color.blue, true);

			if (belowHitsList[i])
			{
				isHitConnected = true;

				if (belowHitsList[i].distance < smallestDistance)
				{
					smallestDistance = belowHitsList[i].distance;
				}
			}
		}

		if (isHitConnected)
		{
			_distanceToTheGround = smallestDistance - _skinWidth;
		}
		else
		{
			_distanceToTheGround = -1;
		}
	}

	private void UpdateRaycastOrigins()
	{
		Bounds bounds = _boxCollider.bounds;
		bounds.Expand(_skinWidth * -2);

		_raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
		_raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
		_raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
		_raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
	}

	private void CalculateRaySpacing()
	{
		Bounds bounds = _boxCollider.bounds;
		bounds.Expand(_skinWidth * -2);

		_horizontalRayCount = Mathf.Clamp(_horizontalRayCount, 2, int.MaxValue);
		_verticalRayCount = Mathf.Clamp(_verticalRayCount, 2, int.MaxValue);

		_horizontalRaySpacing = bounds.size.y / (_horizontalRayCount - 1);
		_verticalRaySpacing = bounds.size.x / (_verticalRayCount - 1);
	}
}

public struct RaycastOrigins
{
	public Vector2 topLeft, topRight;
	public Vector2 bottomLeft, bottomRight;
	public Vector2 middleLeft, middleRight;
}
