using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
	private Rigidbody2D myRigidBody;

	private Animator myAnimator;

	public float movementSpeed;

	private bool attack;

	private bool facingRight;

	[SerializeField]
	private Transform[] groundPoints;

	[SerializeField]
	private float groundRadius;

	[SerializeField]
	private LayerMask WhatIsGround;

	private bool isGrounded;

	private bool jump;

	private bool jumpAttack;

	[SerializeField]
	private bool aircontrol;

	[SerializeField]
	private float jumpForce;



	// Use this for initialization
	void Start () 
	{
		facingRight = false;
		myRigidBody = GetComponent<Rigidbody2D> ();
		myAnimator = GetComponent<Animator> ();
	}

	void Update ()
	{
		HandleInput ();
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		float horizontal = Input.GetAxis ("Horizontal");

		isGrounded = IsGrounded ();

		HandleMovement(horizontal);

		Flip (horizontal);

		HandleAttacks ();

		HandleLayers ();

		ResetValues ();
	}

	private void HandleMovement(float horizontal)
	{
		if (myRigidBody.velocity.y < 0) 
		{
			myAnimator.SetBool ("land", true);
		}

		if (!this.myAnimator.GetCurrentAnimatorStateInfo(0).IsTag("PlayerAttack") && (isGrounded || aircontrol))
		{
			myRigidBody.velocity = new Vector2 (horizontal * movementSpeed, myRigidBody.velocity.y);
		}

		if (isGrounded && jump) 
		{
			isGrounded = false;
			myRigidBody.AddForce (new Vector2 (0, jumpForce));
			myAnimator.SetTrigger ("jump");
		}


		myAnimator.SetFloat ("speed", Mathf.Abs(horizontal));
	}

	private void HandleAttacks()
	{
		if (attack && isGrounded && !this.myAnimator.GetCurrentAnimatorStateInfo(0).IsTag("PlayerAttack")) 
		{
			myAnimator.SetTrigger ("attack");
			myRigidBody.velocity = Vector2.zero;
		}
		if (jumpAttack && !isGrounded && !this.myAnimator.GetCurrentAnimatorStateInfo(1).IsName("JumpAttack")) 
		{
			myAnimator.SetBool("jumpAttack", true);
		}
		if (!jumpAttack && !this.myAnimator.GetCurrentAnimatorStateInfo(1).IsName("JumpAttack")) 
		{
			myAnimator.SetBool("jumpAttack", false);
		}
	}

	private void HandleInput()
	{
		if (Input.GetKeyDown (KeyCode.LeftControl)) 
		{
			attack = true;
			jumpAttack = true;
		}
		if (Input.GetKeyDown (KeyCode.LeftAlt)) 
		{
			jump = true;
		}
	}

	private void Flip(float horizontal)
	{
		if (horizontal > 0 && !facingRight || horizontal < 0 && facingRight) 
		{
			facingRight = !facingRight;

			Vector3 theScale = transform.localScale;

			theScale.x *= -1;

			transform.localScale = theScale;
		}
	}

	private void ResetValues()
	{
		attack = false;
		jump = false;
		jumpAttack = false;
	}

	private bool IsGrounded()
	{
		if (myRigidBody.velocity.y <= 0) 
		{
			foreach (Transform point in groundPoints) 
			{
				Collider2D[] colliders = Physics2D.OverlapCircleAll (point.position, groundRadius, WhatIsGround);

				for (int i = 0; i < colliders.Length; i++) 
				{
					if (colliders [i].gameObject != gameObject) 
					{
						myAnimator.ResetTrigger ("jump");
						myAnimator.SetBool ("land", false);
						return true;
					}
				}
			}
		}
		return false;
	}

	private void HandleLayers()
	{
		if (!isGrounded) 
		{
			myAnimator.SetLayerWeight (1, 1);
		} else 
		{
			myAnimator.SetLayerWeight (1, 0);
		}
	}
}

//line added to commit (accidentely did it with my old account)
