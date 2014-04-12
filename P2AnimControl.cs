using UnityEngine;
using System.Collections;

public class P2AnimControl : MonoBehaviour 
{
	private Transform _transform;
	private Animator _animator;
	private Player2Controls character;

	public enum anim 
	{ 
		None,
		WalkLeft,
		WalkRight,
		StandLeft,
		StandRight,
		FallLeft,
		FallRight
	}

	private anim currentAnim;

	// hash the animation state string to save performance
	private int _p2AnimState = Animator.StringToHash("P2AnimState");
	private int _animState;

	void Awake()
	{
		// cache components to save on performance
		_transform = transform;
		_animator = this.GetComponent<Animator>();
		character = this.GetComponent<Player2Controls>();

		_animState = _p2AnimState;
	}
	
	void Update() 
	{
		if (networkView.isMine) 
		{
			// if the game is over, don't bother updating any animations
			//if(xa.gameOver == true) return;

			// run left
			if (character.currentInputState == Player2Controls.inputState.WalkLeft && character.grounded == true && currentAnim != anim.WalkLeft) {
					currentAnim = anim.WalkLeft;
					_animator.SetInteger (_animState, 1);
					_transform.localScale = new Vector3 (-1, 1, 1);
			}

			// stand left
			if (character.currentInputState != Player2Controls.inputState.WalkLeft && character.grounded == true && currentAnim != anim.StandLeft && character.facingDir == Player2Controls.facing.Left) {
					currentAnim = anim.StandLeft;
					_animator.SetInteger (_animState, 0);
					_transform.localScale = new Vector3 (-1, 1, 1);
			}

			// run right
			if (character.currentInputState == Player2Controls.inputState.WalkRight && character.grounded == true && currentAnim != anim.WalkRight) {
					currentAnim = anim.WalkRight;
					_animator.SetInteger (_animState, 1);
					_transform.localScale = new Vector3 (1, 1, 1);
			}

			// stand right
			if (character.currentInputState != Player2Controls.inputState.WalkRight && character.grounded == true && currentAnim != anim.StandRight && character.facingDir == Player2Controls.facing.Right) {
					currentAnim = anim.StandRight;
					_animator.SetInteger (_animState, 0);
					_transform.localScale = new Vector3 (1, 1, 1);
			}

			// fall or jump left
			if (character.grounded == false && currentAnim != anim.FallLeft && character.facingDir == Player2Controls.facing.Left) {
					currentAnim = anim.FallLeft;
					_animator.SetInteger (_animState, 2);
					_transform.localScale = new Vector3 (-1, 1, 1);
			}

			// fall or jump right
			if (character.grounded == false && currentAnim != anim.FallRight && character.facingDir == Player2Controls.facing.Right) {
					currentAnim = anim.FallRight;
					_animator.SetInteger (_animState, 2);
					_transform.localScale = new Vector3 (1, 1, 1);
			}
		} 
	}
}
