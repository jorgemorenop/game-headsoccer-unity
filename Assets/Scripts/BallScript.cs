using UnityEngine;
using System.Collections;

public class BallScript : MonoBehaviour {

	//Guardamos la posicion inicial para despues poder volver a ella
	private float posIniX, posIniY;

	//le quito el static por si en el futuro le ponemos mas bolas o algo :P
	float contSize = 0;
	float contGrav = 0;
	float contBouncy = 0;
	bool isBig, isSmall, gravitySwap, moreBouncy, lessBouncy;

	void Start() {
		posIniX = GetComponent<Rigidbody2D>().position.x;
		posIniY = GetComponent<Rigidbody2D>().position.y;

		gameObject.GetComponent<Collider2D>().sharedMaterial.bounciness = 0.8f;
	}


	void Update(){
		//Handles all the times
		if (contSize > 0) {
			contSize -= Time.deltaTime;
		} 
		if ((isBig || isSmall) && contSize <= 0) {
			ResetSize ();
		} 

		if (contGrav > 0) {
			contGrav -= Time.deltaTime;
		}
		if (gravitySwap && contGrav <= 0) {
			ResetGravity();
		}

		if (contBouncy > 0) {
			contBouncy -= Time.deltaTime;
		} 
		if ((moreBouncy || lessBouncy) && contBouncy <= 0) {
			ResetBouncy ();
		} 

	}

	//Pone la bola en la posicion inicial
	public void ResetPosition() {
		this.transform.position = new Vector2 (this.posIniX, this.posIniY);
		this.gameObject.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		this.gameObject.GetComponent<Rigidbody2D> ().angularVelocity = 0f;
		ResetBouncy();
		ResetGravity();
		ResetSize();
	}

	void OnCollisionEnter2D (Collision2D coll) {
		if (coll.gameObject.tag == "Team1") {
			GMScript.lastPlayer = "Team1";
		} 
		else if (coll.gameObject.tag == "Team2") {
			GMScript.lastPlayer = "Team2";
		}
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

	public void SwitchGravity () {
		if (!gravitySwap) {
			gravitySwap = true;
			this.GetComponent<Rigidbody2D> ().gravityScale = -1;
		}
		contGrav = 7;
	}

	public void ResetGravity() {
		if (gravitySwap) {
			contGrav = 0;
			gravitySwap = false;
			this.GetComponent<Rigidbody2D>().gravityScale = 1;
			this.transform.Rotate( new Vector3(180f, 0f, 0f) );
		}
	}

	public void MakeMoreBouncy(){
		moreBouncy = true;
		lessBouncy = false;
		gameObject.GetComponent<Collider2D>().sharedMaterial.bounciness = 1f;
		contBouncy = 7;
	}
	
	public void MakeLessBouncy(){
		moreBouncy = false;
		lessBouncy = true;
		gameObject.GetComponent<Collider2D>().sharedMaterial.bounciness = 0.2f;
		contBouncy = 7;
	}
	
	public void ResetBouncy(){
		moreBouncy = false;
		lessBouncy = false;
		gameObject.GetComponent<Collider2D>().sharedMaterial.bounciness = 0.8f;
		contBouncy = 0;
	}
}
