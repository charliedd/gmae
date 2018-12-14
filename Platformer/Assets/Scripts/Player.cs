using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour {

	float moveSpeed = 10;
	float wallSlideSpeedMax = 3;
	float dashSpeed = 50;
	float dashDuration = 0.05f;
	float timer = 0;
	float wallTimer = 0;
	float DEADZONE_MOVEMENT = 0.15f;

	float maxJumpHeight = 4;
	float minJumpHeight = 1;
	float timeToJumpApex = 0.4f;
	float maxJumpVelocity;
	float minJumpVelocity;
	float gravity;

	public Vector2 wallJumpClimb;
	public Vector2 wallJumpOff;
	public Vector2 wallLeap;

	public float wallStickTime = .25f;
	float timeToWallUnstick;
	PlayerState playerState;
	float velocityXSmoothing;
	Vector3 velocity;

	float wallTimeJump = .15f;
	//float wallTimeLeap = .25f;

	float accelerationTimeAirborne = .2f;
	float accelerationTimeGrounded = .1f;

	public struct PlayerState{
		public bool dashing;
		public bool facingRight;

		public bool wallJumping;
	}


	Controller2D controller;
	void Start(){
		controller = GetComponent<Controller2D> ();
		playerState.dashing = false;
		playerState.facingRight = true;
		playerState.wallJumping = false;
		gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs(gravity * timeToJumpApex);
		minJumpVelocity = Mathf.Sqrt(2*Mathf.Abs(gravity)* minJumpHeight);

	}

	void Update(){
		handleMovement ();
		
	
	
		

	}

	public void handleMovement(){
		bool wallSliding = false;

		Vector2 input = new Vector2 (Input.GetAxis ("LeftJoystickHorizontal"), Input.GetAxis ("LeftJoystickVertical"));
		if (input.magnitude < DEADZONE_MOVEMENT){
			input = Vector2.zero;
		}

		int wallDirx = (controller.collisions.left)? -1 : 1;

		if((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0){
			wallSliding = true;
			//playerState.wallJumping = false;
			wallTimer = 0;

			if(velocity.y < -wallSlideSpeedMax){
				velocity.y = -wallSlideSpeedMax;
			}
		}

		if (controller.collisions.above || controller.collisions.below) {
			velocity.y = 0;
		}

		if (Input.GetButtonDown ("BButton")) {
			if(wallSliding){
				if(wallDirx == input.x){
					velocity.x = -wallDirx * wallJumpClimb.x;
					velocity.y = wallJumpClimb.y;
				}
				else{
					velocity.x = -wallDirx * wallLeap.x;
					velocity.y = wallLeap.y;
					//wallTimeJump = .5f;
				}
				playerState.wallJumping = true;
			}
			if(controller.collisions.below){
				velocity.y = maxJumpVelocity;
			}
		}

		if(Input.GetButtonUp("BButton")){
			if(velocity.y > minJumpVelocity){
				velocity.y = minJumpVelocity;
			}
			
		}
		
			
		if (input.x > 0) {
			playerState.facingRight = true;
			} else if (input.x < 0) {
				playerState.facingRight = false;
				}

		
		if(playerState.wallJumping){
			if(wallTimer <= wallTimeJump){
				wallTimer+= Time.deltaTime;
				float targetVelocityX = input.x * moveSpeed;
				velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing,accelerationTimeAirborne);
			}else{
				playerState.wallJumping = false;
				wallTimeJump = .15f;
				wallTimer = 0;
			}
			

		}else{
			velocity.x = input.x * moveSpeed;
		}
		

		velocity.y += gravity * Time.deltaTime;

		if (!playerState.dashing) {
			if (Input.GetButtonDown ("XButton")) {
				Debug.Log ("DASH");
				playerState.dashing = true;
				timer = 0;
			} 
		} else {
			if (timer <= dashDuration) {
				if (playerState.facingRight) {
					velocity.x = dashSpeed;
				} else {
					velocity.x = -dashSpeed;
				}
				timer += Time.deltaTime;
				velocity.y = 0;
			} else {
				playerState.dashing = false;
				//velocity.x = 3;
			}
		}


		//velocity.y += gravity * Time.deltaTime;
		controller.Move (velocity * Time.deltaTime);
	}
		
}
