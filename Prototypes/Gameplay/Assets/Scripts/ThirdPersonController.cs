using UnityEngine;
using System.Collections;

public class ThirdPersonController : MonoBehaviour 
{

	public AnimationClip idleAnimation;
	public AnimationClip walkAnimation;
	public AnimationClip runAnimation;
	
	public float walkMaxAnimationSpeed = 0.75f;
	public float trotMaxAnimationSpeed = 1.0f;
	public float runMaxAnimationSpeed = 1.0f;
	public float landAnimationSpeed = 1.0f;

	enum CharacterState
	{
		Idle = 0,
		Walking = 1,
		Running = 3,
	}
	private CharacterState _characterState;
	private Animation _animation;

	// The speed when walking
	public float walkSpeed = 4.0f;
	// when pressing "Fire3" button (cmd) we start running
	public float runSpeed = 6.0f;
	
	public float speedSmoothing = 10.0f;
	public float rotateSpeed = 50000.0f;
	

	// The camera doesnt start following the target immediately but waits for a split second to avoid too much waving around.
	private float lockCameraTimer = 0.0f;
	
	// The current move direction in x-z
	private Vector3 moveDirection = Vector3.zero;
	// The current x-z move speed
	private float moveSpeed = 0.0f;
	
	// Are we moving backwards (This locks the camera to not do a 180 degree spin)
	private bool movingBack = false;
	// Is the user pressing any keys?
	private bool isMoving = false;

	private bool isControllable = true;


	void Awake ()
	{
		moveDirection = transform.TransformDirection(Vector3.forward);
		
		_animation = GetComponent<Animation>();
		if(!_animation)
			Debug.Log("The character you would like to control doesn't have animations. Moving her might look weird.");

		if(!idleAnimation) {
			_animation = null;
			Debug.Log("No idle animation found. Turning off animations.");
		}
		if(!walkAnimation) {
			_animation = null;
			Debug.Log("No walk animation found. Turning off animations.");
		}
		if(!runAnimation) {
			_animation = null;
			Debug.Log("No run animation found. Turning off animations.");
		}
		
	}
	
	
	void UpdateSmoothedMovementDirection ()
	{
		Transform cameraTransform = Camera.main.transform;
		
		// Forward vector relative to the camera along the x-z plane	
		Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);
		forward.y = 0;
		forward = forward.normalized;
		
		// Right vector relative to the camera
		// Always orthogonal to the forward vector
		Vector3 right = new Vector3(forward.z, 0, -forward.x);
		
		var v = Input.GetAxisRaw("Vertical");
		var h = Input.GetAxisRaw("Horizontal");
		
		// Are we moving backwards or looking backwards
		if (v < -0.2)
			movingBack = true;
		else
			movingBack = false;
		
		var wasMoving = isMoving;
		isMoving = Mathf.Abs (h) > 0.1 || Mathf.Abs (v) > 0.1;
		
		// Target direction relative to the camera
		var targetDirection = h * right + v * forward;
		
		// Grounded controls
		// Lock camera for short period when transitioning moving & standing still
		lockCameraTimer += Time.deltaTime;
		if (isMoving != wasMoving)
			lockCameraTimer = 0.0f;
		
		// We store speed and direction seperately,
		// so that when the character stands still we still have a valid forward direction
		// moveDirection is always normalized, and we only update it if there is user input.
		if (targetDirection != Vector3.zero)
		{
			// If we are really slow, just snap to the target direction
			if (moveSpeed < walkSpeed * 0.9f)
			{
				moveDirection = targetDirection.normalized;
			}
			// Otherwise smoothly turn towards it
			else
			{
				moveDirection = Vector3.RotateTowards(moveDirection, targetDirection, rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);
				
				moveDirection = moveDirection.normalized;
			}
		}
		
		// Smooth the speed based on the current target direction
		var curSmooth = speedSmoothing * Time.deltaTime;
		
		// Choose target speed
		//* We want to support analog input but make sure you cant walk faster diagonally than just forward or sideways
		var targetSpeed = Mathf.Min(targetDirection.magnitude, 1.0f);
		
		_characterState = CharacterState.Idle;
		
		// Pick speed modifier
		if (GetComponent<Player>().canRun)
		{
			targetSpeed *= runSpeed;
			_characterState = CharacterState.Running;
		}
		else
		{
			targetSpeed *= walkSpeed;
			_characterState = CharacterState.Walking;
		}
		
		moveSpeed = Mathf.Lerp (moveSpeed, targetSpeed, curSmooth);		
	}
	

	void Update() {
		
		if (!isControllable)
		{
			// kill all inputs if not controllable.
			Input.ResetInputAxes();
		}

		UpdateSmoothedMovementDirection();

        // Calculate actual motion
        Vector3 movement = moveDirection * moveSpeed;
		movement *= Time.deltaTime;
		
		// Move the controller
		Rigidbody controller = GetComponent<Rigidbody>();
        controller.position = movement + controller.position;
		
		// ANIMATION sector
		if(_animation) {

			if(movement.sqrMagnitude < 0.001) {
				_animation.CrossFade(idleAnimation.name);
			}
			else 
			{
				if(_characterState == CharacterState.Running) {
                    _animation[runAnimation.name].speed = runMaxAnimationSpeed;//Mathf.Clamp(controller.velocity.magnitude, 0.0f, runMaxAnimationSpeed);
                    _animation.CrossFade(runAnimation.name);	
				}
				else if(_characterState == CharacterState.Walking) {
                    _animation[walkAnimation.name].speed = walkMaxAnimationSpeed;// Mathf.Clamp(controller.velocity.magnitude, 0.0f, walkMaxAnimationSpeed);
					_animation.CrossFade(walkAnimation.name);	
				}
				
			}

		}
		
		// Set rotation to the move direction
		transform.rotation = Quaternion.LookRotation(moveDirection);

	}
	
	float GetSpeed () {
		return moveSpeed;
	}
	
	Vector3 GetDirection () {
		return moveDirection;
	}
	
	bool IsMovingBackwards () {
		return movingBack;
	}
	
	float GetLockCameraTimer () 
	{
		return lockCameraTimer;
	}
	
	bool IsMoving ()
	{
		return Mathf.Abs(Input.GetAxisRaw("Vertical")) + Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.5;
	}


}
