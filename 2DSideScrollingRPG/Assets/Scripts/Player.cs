using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
	private static Player instance;

	public static Player Instance
	{
		get 
		{
			if (instance == null) 
			{
				instance = GameObject.FindObjectOfType<Player>();
			}
			return instance;
		}
	}

	private Animator myAnimator;

	public float movementSpeed;

	private bool facingRight;

	[SerializeField]
	private Transform[] groundPoints;

	[SerializeField]
	private float groundRadius;

	[SerializeField]
	private LayerMask WhatIsGround;

	[SerializeField]
	private bool aircontrol;

	[SerializeField]
	private float jumpForce;

	public Rigidbody2D MyRigibody { get; set; }

	public bool Attack { get; set; }

	public bool Jump { get; set; }

	public bool OnGround { get; set; }

	// Use this for initialization
	void Start () 
	{
		facingRight = false;
		MyRigibody = GetComponent<Rigidbody2D> ();
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

		OnGround = IsGrounded ();

		HandleMovement(horizontal);

		Flip (horizontal);

		HandleLayers ();
	}

	private void HandleMovement(float horizontal)
	{
		if (MyRigibody.velocity.y < 0) 
		{
			myAnimator.SetBool ("land", true);
		}
		if (!Attack && (OnGround || aircontrol)) 
		{
			MyRigibody.velocity = new Vector2 (horizontal * movementSpeed, MyRigibody.velocity.y);
		}
		if (Jump && MyRigibody.velocity.y == 0) 
		{
			MyRigibody.AddForce(new Vector2 (0, jumpForce));
		}

		myAnimator.SetFloat("speed", Mathf.Abs (horizontal));
	}

	private void HandleInput()
	{
		if (Input.GetKeyDown (KeyCode.LeftControl)) 
		{
			myAnimator.SetTrigger("attack");
		}
		if (Input.GetKeyDown (KeyCode.LeftAlt)) 
		{
			myAnimator.SetTrigger("jump");
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

	private bool IsGrounded()
	{
		if (MyRigibody.velocity.y <= 1) 
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

	private void HandleLayers()
	{
		if (!OnGround) 
		{
			myAnimator.SetLayerWeight (1, 1);
		} else 
		{
			myAnimator.SetLayerWeight (1, 0);
		}
	}
}