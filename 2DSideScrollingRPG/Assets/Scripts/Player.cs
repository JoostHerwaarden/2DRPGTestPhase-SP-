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

		ResetValues ();
	}

	private void HandleMovement(float horizontal)
	{
		if (!this.myAnimator.GetCurrentAnimatorStateInfo(0).IsTag("PlayerAttack") && (isGrounded || aircontrol))
		{
			myRigidBody.velocity = new Vector2 (horizontal * movementSpeed, myRigidBody.velocity.y);
		}

		if (isGrounded && jump) 
		{
			isGrounded = false;
			myRigidBody.AddForce (new Vector2 (0, jumpForce));
		}


		myAnimator.SetFloat ("speed", Mathf.Abs(horizontal));
	}

	private void HandleAttacks()
	{
		if (attack && !this.myAnimator.GetCurrentAnimatorStateInfo(0).IsTag("PlayerAttack")) 
		{
			myAnimator.SetTrigger ("attack");
			myRigidBody.velocity = Vector2.zero;
		}
	}

	private void HandleInput()
	{
		if (Input.GetKeyDown (KeyCode.LeftControl)) 
		{
			attack = true;
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
						return true;
					}
				}
			}
		}
		return false;
	}
}
