using UnityEngine;
using System.Collections;

public class PlayersScript : MonoBehaviour{
	private float resetPosX;
	private float resetPosY;
	
	public KeyCode moveUp;
	public KeyCode moveRight;
	public KeyCode moveLeft;

	public int speed = 4;
	public int salto = 4;

	float contGrav = 0, contJump = 0, contSize = 0;
	bool gravitySwap, moreJump, lessJump, isBig, isSmall;

	//Se trata de un porcentaje, asi que conviene ponerlo entre 0 y 100.
	public static int retardoParada = 99;

	float movimientoX = 0;

	// Use this for initialization
	void Start () {
		resetPosX = GetComponent<Rigidbody2D>().position.x;
		resetPosY = GetComponent<Rigidbody2D>().position.y;

	}
	
	// Update is called once per frame
	void Update () {
		movimientoX = GetComponent<Rigidbody2D>().velocity.x;
		
		//We make the object stop emitting, and if it is pressing up, emit.
		transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().enableEmission = false;
		
		if (Input.GetKey(moveUp) && GMScript.timerStopped == false) {
			Jump ();
			transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().enableEmission = true;
		}

		if (Input.GetKey(moveLeft) && GMScript.timerStopped == false) {
			movimientoX = -speed;
		}
		else if (Input.GetKey(moveRight) && GMScript.timerStopped == false) {
			movimientoX = speed;
		}
		else if (movimientoX == speed || movimientoX == -speed) {
			GetComponent<Rigidbody2D>().velocity = new Vector2(0f, GetComponent<Rigidbody2D>().velocity.y);
			movimientoX = 0f;
		}

		if (contGrav > 0) {
			contGrav -= Time.deltaTime;
		} 
		if (gravitySwap && contGrav <= 0) {
			gravitySwap = false;
			this.GetComponent<Rigidbody2D>().gravityScale = 1;
			this.transform.Rotate( new Vector3(180f, 0f, 0f) );
		}

		if (contJump > 0) {
			contJump -= Time.deltaTime;
		}
		if ((moreJump || lessJump) && contJump <= 0) {
			moreJump = false;
			lessJump = false;
			ResetJump ();
		}

		if (contSize > 0) {
			contSize -= Time.deltaTime;
		} 
		if ((isBig || isSmall) && contSize <= 0) {
			ResetSize ();
		} 

		GetComponent<Rigidbody2D>().velocity = new Vector2 (movimientoX, GetComponent<Rigidbody2D>().velocity.y);
	}

	void Jump(){
		if (!gravitySwap) {
			GetComponent<Rigidbody2D>().velocity = new Vector2 (GetComponent<Rigidbody2D>().velocity.x, salto);
		} else {
			GetComponent<Rigidbody2D>().velocity = new Vector2 (GetComponent<Rigidbody2D>().velocity.x, -salto);
		}
	}
	
	public void ResetPlayer (){
		GetComponent<Rigidbody2D>().position = new Vector2 (resetPosX, resetPosY);
		GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

		//SI SE DESCOMENTA EL CODIGO SIGUIENTE, LOS JUGADORES NO SE RESETEAN TRAS UN GOL NO SE POR QUE.
		if (isBig || isSmall) {
			ResetSize();
		}
		if (gravitySwap) {
			contGrav = 0;
			gravitySwap = false;
			this.GetComponent<Rigidbody2D>().gravityScale = 1;
			this.transform.Rotate( new Vector3(180f, 0f, 0f) );
		}
	}

	public void SwitchGravity () {
		this.transform.Rotate( new Vector3(180f, 0f, 0f) );
		gravitySwap = true;
		this.GetComponent<Rigidbody2D>().gravityScale = -1;
		contGrav = 7;
	}

	public void MoreJump () {
		moreJump = true;
		lessJump = false;
		this.salto = 8;
		contJump = 7;
	}

	public void LessJump () {
		moreJump = false;
		lessJump = true;
		this.salto = 1;
		contJump = 7;
	}

	public void ResetJump(){
		moreJump = false;
		lessJump = false;
		this.salto = 4;
		contJump = 0;
	}

	public void MakeBigger(){
		isSmall = false;
		isBig = true;
		transform.localScale = new Vector2 (2, 2);
		contSize = 7;
	}
	
	public void MakeSmaller(){
		isBig = false;
		isSmall = true;
		transform.localScale = new Vector2 (0.5f, 0.5f);
		contSize = 7;
	}
	
	public void ResetSize(){
		isBig = false;
		isSmall = false;
		transform.localScale = new Vector2 (1, 1);
		contSize = 0;
	}
	
}
