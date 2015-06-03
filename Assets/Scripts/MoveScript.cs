using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoveScript : MonoBehaviour {

	public Vector3 DestinationPosition;
	public Sprite[] sprites;
	public bool facingLeft = true;
	public bool isMoving = false;

	public int MoveDirection = 0; // angles of moving direction - 0,45,90,135,180,225,270,315;
	// private int AllowedDistance = 5; // allowed destination from track
	// private int k_angle_current = 0; // currect coeffitient of moving
	// private float k_angle_delta = 0; // allowed delta from K when moving direction animation should switch
	private float duration = 10F;
	public SpriteRenderer SpRen; 

	// public GameObject track;
	public GameObject MovingPerson;
	Animator anim;

	void SetSprite(int MoveDir){
		// string spritePath = "Units/CurseOfMonkeyIsland_Manual_1";
		switch (MoveDir) 
		{
		case 45:
			SpRen.sprite = sprites[0];
			break;
		case 0:
			SpRen.sprite = sprites[1];
			break;
		case 315:
			SpRen.sprite = sprites[2];
			break;
		case 135:
			SpRen.sprite = sprites[3];
			break;
		case 180:
			SpRen.sprite = sprites[4];
			break;
		case 225:
			SpRen.sprite = sprites[5];
			break;
		default: 
			SpRen.sprite = sprites[0];
			break;
		}
		//Debug.Log
	}

	private void Flip()
	{
		// Switch the way the player is labelled as facing
		facingLeft = !facingLeft;
		
		// Multiply the player's x local scale by -1
		Vector3 theScale = MovingPerson.transform.localScale;
		theScale.x *= -1;
		MovingPerson.transform.localScale = theScale;
	}

	private int CalculateDirection(Vector3 DestP)
	{
		float k_angle_delta = 0.0F; 
		try 
		{
			k_angle_delta = (DestP.x - MovingPerson.transform.position.x) / (DestP.y + 0.6F - MovingPerson.transform.position.y);
		}
		catch (UnityException e)
		{
			Debug.Log ("Possible divide by zero");
		}
		float LRDirection = DestP.x - MovingPerson.transform.position.x;
		float UDDirection = DestP.y + 0.6F - MovingPerson.transform.position.y;
		Debug.Log ("k_angle_delta == " + k_angle_delta.ToString());
		float k = Mathf.Abs(k_angle_delta);		

		if (facingLeft) {
			if (LRDirection > 0) {Flip ();} 
		}
		else 
		{
			if (LRDirection < 0) {Flip ();}
		}

		if (k_angle_delta <= 0)
		{
			if ((LRDirection <= 0) && (UDDirection >= 0))
			{
				if (k <= 0.1) return 180; 
				else if ((k < 5) && (k > 0.1)) return 135;
				else if (k >= 5) return 90;
			}
			else 
			{
				if (k <= 0.1) return 0; 
				else if ((k < 5) && (k > 0.1)) return 315;
				else if (k >= 5) return 270;
			}
		}
		else 
		{
			if ((LRDirection >= 0) && (UDDirection >= 0))
			{
				if (k <= 0.1) return 180; 
				else if ((k < 5) && (k > 0.1)) return 225;
				else if (k >= 5) return 270;
			}
			else 
			{
				if (k <= 0.1) return 0; 
				else if ((k < 5) && (k > 0.1)) return 45;
				else if (k >= 5) return 90;
			}
		}
		return -1;
	}

	public void Move (Vector3 DestP)
	{
		isMoving = true;
		DestP.y = DestP.y + 0.6F;
		DestP.z = -5.0F;
		// double distanceLength = Mathf.Sqrt (Mathf.Pow (DestP.x - MovingPerson.transform.position.x, 2) + Mathf.Pow (DestP.y - MovingPerson.transform.position.y, 
		// transform.Translate (DestP*0.001F);
		transform.position = Vector3.Lerp(gameObject.transform.position, DestP, 1/(duration*(Vector3.Distance(gameObject.transform.position, DestP))));
	}

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		SpRen = MovingPerson.GetComponentInChildren<SpriteRenderer>();
		//SetSprite(0);
	}
	
	// Update is called once per frame
	void Update() {
		Debug.Log ("Update happening");
		// SetSprite(MoveDirection);
		Vector3 CheckPosition = new Vector3(DestinationPosition.x, DestinationPosition.y + 0.6F, -5.0F);
		if (isMoving) {
			if (MovingPerson.transform.position != CheckPosition)
			{
//				anim.Play("GB_walk_" + MoveDirection.ToString());
				Move (DestinationPosition);
			}
			else 
			{
				isMoving = false;
				anim.Play("Idle");
				SetSprite(MoveDirection);
			}
		}
		else SetSprite(MoveDirection);

		if (Input.GetMouseButtonDown (0)) 
		{
			//Debug.Log ("Pressed left click.");
			Vector3 ClickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Debug.Log ("Destination position: " + ClickPosition.ToString());
			Debug.Log ("Person position: " + MovingPerson.transform.position.ToString());
			DestinationPosition = ClickPosition;
			MoveDirection = CalculateDirection(DestinationPosition);
			Move(DestinationPosition);
			if (MoveDirection >= 0) 
			{
				//if (MoveDirection > 180) Flip ();
				anim.Play("GB_walk_" + MoveDirection.ToString());
				Debug.Log("Tried to play animation: GB_walk_" + MoveDirection.ToString());
			}
		}
		if(Input.GetMouseButtonDown(1))
			Debug.Log("Pressed right click.");
		if(Input.GetMouseButtonDown(2))
			Debug.Log("Pressed middle click.");
	}
}
