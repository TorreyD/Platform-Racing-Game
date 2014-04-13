using UnityEngine;
using System.Collections;

public class Player1Controls : MonoBehaviour
{	
	public enum inputState 
	{ 
		None, 
		WalkLeft, 
		WalkRight, 
		Jump, 
		Pass
	}

	[HideInInspector] public inputState currentInputState;
	
	[HideInInspector] public enum facing { Right, Left }
	[HideInInspector] public facing facingDir;
	
	[HideInInspector] public bool alive = true;
	[HideInInspector] public Vector3 spawnPos;
	
	private Transform _transform;
	private Rigidbody2D _rigidbody;
	private int lastCheckPointReached;
	
	// edit these to tune character movement	
	private float walkVel = 2f; 	// walk speed
	private float jumpVel = 4f; 	// jump velocity
	private float jump2Vel = 2f; 	// double jump velocity
	private float fallVel = 1f;		// fall velocity, gravity

	private float moveVel;

	private int jumps = 0;
	private int maxJumps = 2; 		// set to 2 for double jump

	// raycast stuff
	private RaycastHit2D hit;
	private Vector2 physVel = new Vector2();
	[HideInInspector] public bool grounded = false;
	private int groundMask = 1 << 8; // Ground layer mask

	// Use this for initialization
	public void Start () 
	{
		_transform = transform;
		_rigidbody = rigidbody2D;
		lastCheckPointReached = 0;

		moveVel = walkVel;
		transform.position = new Vector3 (-3.763523f, 0.2351843f, 0);
		if (!networkView.isMine) 
		{
			GetComponent<Rigidbody2D>().gravityScale = 0;
		} 
	}

	// Update is called once per frame
	public void Update () 
	{
		if (networkView.isMine)
		{
			if (NetworkManager._playerPos == NetworkManager.PlayerPosition.Player1) 
			{
				CamMovement._target = GameObject.FindGameObjectWithTag("Player1");
			}

			// these are false unless one of keys is pressed
			currentInputState = inputState.None;
			
			// keyboard input
			if (Input.GetKey (KeyCode.LeftArrow)) 
			{ 
				currentInputState = inputState.WalkLeft;
				facingDir = facing.Left;
			}
			if (Input.GetKey (KeyCode.RightArrow) && currentInputState != inputState.WalkLeft) 
			{ 
				currentInputState = inputState.WalkRight;
				facingDir = facing.Right;
			}
			
			if (Input.GetKeyDown (KeyCode.UpArrow)) 
			{ 
				currentInputState = inputState.Jump;
			}
			
			UpdatePhysics ();
			checkIfFallen();
			manageCheckPoints();
		}
	}

	// Update is called once per frame
	private void checkIfFallen() 
	{
		// teleport me to a checkpoint
		if(_transform.position.y < -3f)
		{
			respawnMe();
		}
	}
	
	// ============================== FIXEDUPDATE ==============================//
	
	private void UpdatePhysics()
	{
		physVel = Vector2.zero;
		
		// move left
		if(currentInputState == inputState.WalkLeft)
		{
			physVel.x = -moveVel;
		}
		
		// move right
		if(currentInputState == inputState.WalkRight)
		{
			physVel.x = moveVel;
		}
		
		// jump
		if(currentInputState == inputState.Jump)
		{
			if(jumps < maxJumps)
			{
				jumps += 1;
				if(jumps == 1)
				{
					_rigidbody.velocity = new Vector2(physVel.x, jumpVel);
				}
				else if(jumps == 2)
				{
					_rigidbody.velocity = new Vector2(physVel.x, jump2Vel);
				}
			}
		}
		
		// use raycasts to determine if the player is standing on the ground or not
		if (Physics2D.Raycast(new Vector2(_transform.position.x-0.1f,_transform.position.y), -Vector2.up, .26f, groundMask) 
		    || Physics2D.Raycast(new Vector2(_transform.position.x+0.1f,_transform.position.y), -Vector2.up, .26f, groundMask))
		{
			grounded = true;
			jumps = 0;
		}
		else
		{
			grounded = false;
			_rigidbody.AddForce(-Vector3.up * fallVel);
		}
		
		// actually move the player
		_rigidbody.velocity = new Vector2(physVel.x, _rigidbody.velocity.y);
	}

	public void respawnMe()
	{
		if (lastCheckPointReached == 1)
		{
			_transform.position = GameObject.FindGameObjectWithTag("checkpoint1").transform.position;
		}
		else if (lastCheckPointReached == 2)
		{
			_transform.position = GameObject.FindGameObjectWithTag("checkpoint2").transform.position;
		}
	}

	public void manageCheckPoints()
	{
		if (_transform.position.x > GameObject.FindGameObjectWithTag("checkpoint1").transform.position.x &&
		    _transform.position.x < GameObject.FindGameObjectWithTag("checkpoint2").transform.position.x)
		{
			lastCheckPointReached = 1;
		}
		else if (_transform.position.x > GameObject.FindGameObjectWithTag("checkpoint1").transform.position.x &&
		         _transform.position.x > GameObject.FindGameObjectWithTag("checkpoint2").transform.position.x)
		{
			lastCheckPointReached = 2;
		}
	}

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		Vector3 newSyncPosition = Vector3.zero;
		Vector3 newFacingVector = Vector3.zero;
		int newAnimState = 0;

		if (stream.isWriting)
		{
			newFacingVector = _transform.localScale;
			newSyncPosition = _transform.position;
			newAnimState = this.GetComponent<Animator>().GetInteger(Animator.StringToHash ("P1AnimState"));

			stream.Serialize(ref newSyncPosition);
			stream.Serialize(ref newFacingVector);
			stream.Serialize(ref newAnimState);
		}
		else
		{
			stream.Serialize(ref newSyncPosition);
			stream.Serialize(ref newFacingVector);
			stream.Serialize(ref newAnimState);

			_transform.localScale = newFacingVector;
			_transform.position = newSyncPosition;
			this.GetComponent<Animator>().SetInteger(Animator.StringToHash ("P1AnimState"), newAnimState);
		}
	}
}
