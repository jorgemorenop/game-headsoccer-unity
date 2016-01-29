using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	public KeyCode up, down, left, right, shoot;

	public int velocidad = 4;
	public float maxVel = 2;
	public int salto = 4; 
	private int saltoInicial;

	float contGrav = 0, contJump = 0, contSize = 0;
	bool gravitySwap, moreJump, lessJump, isBig, isSmall;

	public bool controlIA;
	public int IAnivel; //Entre 1 y 9 por ejemplo
	public IAState tipoIA;

	//Cada jugador contendra informacion sobre la pelota, la ubicacion de su porteria y la porteria del enemigo
	private GameObject ball, myGoal, opponentGoal, controller, bota;

	//Guardamos la posicion inicial para despues poder volver a ella
	private float posIniX, posIniY;
	private bool canJump, agachado;


	// Use this for initialization
	void Start () {	
		this.saltoInicial = salto;
		posIniX = GetComponent<Rigidbody2D>().position.x;
		posIniY = GetComponent<Rigidbody2D>().position.y;

		ball = GameObject.FindWithTag ("Ball");
		controller = GameObject.FindWithTag ("GameController");

		if (this.tag == "Team1") {
			myGoal = GameObject.FindWithTag ("LeftGoal");
			opponentGoal = GameObject.FindWithTag ("RightGoal");
		} else if (this.tag == "Team2") {
			myGoal = GameObject.FindWithTag ("RightGoal");
			opponentGoal = GameObject.FindWithTag ("LeftGoal");
		}

		canJump = true;

		//bota = this.transform.Find("Boot").gameObject;

		if (bota==null)					//Si bota es nula (bota==null)
			Debug.Log("Bota nula");
	}

	public void setJump(bool canJump) {
		this.canJump = canJump;
	}
	public bool getAgachado() {
		return this.agachado;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		ControlarBufos ();

		if (controlIA) {
			IAPlay ();
		}
		//Human player
		else {
			if (Input.GetKey (up))
				Jump ();
			if (Input.GetKey (left))
				MoveLeft ();
			if (Input.GetKey (right))
				MoveRight ();

			if (Input.GetKey (down)) {
				agachado = true;
				transform.localScale = new Vector2 (transform.localScale.x, transform.localScale.x / 2);
			} else {
				agachado = false;
				transform.localScale = new Vector2 (transform.localScale.x, transform.localScale.x);
			}

			/*if (bota.transform.eulerAngles.z < 90) {
				if (Input.GetKey (shoot)) 
					bota.GetComponent<Rigidbody2D> ().AddTorque (200);// = new Vector3(0,0,bota.transform.eulerAngles.z+20);
				else if (bota.transform.eulerAngles.z > 0)
						bota.GetComponent<Rigidbody2D> ().AddTorque (-100);
					else
						bota.transform.eulerAngles = new Vector3 (0, 0, 0);
				}
				else bota.transform.eulerAngles = new Vector3(0,0,90);*/
			
		}

		//bota.transform.position = transform.position;
	}

	public void Jump () {
		Vector2 fuerza = gravitySwap ? new Vector2 (0, -50 * salto): new Vector2 (0, 50 * salto);
		if (this.canJump) {
			this.gameObject.GetComponent<Rigidbody2D> ().velocity = new Vector2 (this.gameObject.GetComponent<Rigidbody2D> ().velocity.x, 0); //Ponemos 0 en la Y para que no acumule
			this.gameObject.GetComponent<Rigidbody2D> ().AddForce (fuerza);
			this.canJump = false;
		}
	}

	public void MegaJump() {
		Vector2 fuerza = gravitySwap ? new Vector2 (0, -50 * salto * 2): new Vector2 (0, 50 * salto * 2);
		this.gameObject.GetComponent<Rigidbody2D> ().velocity = new Vector2 (this.gameObject.GetComponent<Rigidbody2D> ().velocity.x, 0); //Ponemos 0 en la Y para que no acumule
		this.gameObject.GetComponent<Rigidbody2D> ().AddForce (fuerza);
		this.canJump = false;
	}

	public void MoveLeft() {
		this.gameObject.GetComponent<Rigidbody2D>().AddForce (new Vector2 (-velocidad, 0));

		//Si la velocidad del eje X es menor que -maxVel, la ponemos a ese valor
		if (this.gameObject.GetComponent<Rigidbody2D> ().velocity.x < -maxVel) {
			this.gameObject.GetComponent<Rigidbody2D> ().velocity = new Vector2 (-maxVel, this.gameObject.GetComponent<Rigidbody2D> ().velocity.y);
			//this.gameObject.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		}
	}

	public void MoveRight() {
		this.gameObject.GetComponent<Rigidbody2D>().AddForce (new Vector2 (velocidad, 0));

		//Si la velocidad del eje X es mayor que maxVel, la ponemos a ese valor
		if (this.gameObject.GetComponent<Rigidbody2D> ().velocity.x > maxVel) {
			this.gameObject.GetComponent<Rigidbody2D> ().velocity = new Vector2 (maxVel, this.gameObject.GetComponent<Rigidbody2D> ().velocity.y);
			//this.gameObject.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		}
	}

	//Pone al jugador en la posicion inicial
	public void ResetPosition() {
		this.transform.position = new Vector2 (this.posIniX, this.posIniY);
		this.gameObject.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		this.ResetSize();
		this.ResetJump();
		this.ResetGravity();
	}

	public void IAPlay ()
	{
		//Guardamos los valores relativos de la posicion de la pelota respecto al jugador
		float relBallPosX = ball.transform.position.x - this.transform.position.x;
		float relBallPosY = ball.transform.position.y - this.transform.position.y;
		bool bolaCerca = (relBallPosX > -0.5f && relBallPosX < 0.5f && relBallPosY > 0 && relBallPosY < 2f);

		float desiredX = 0;

		//Decide como se tiene que mover en funcion del tipo de IA que es
		switch (tipoIA)
		{
			case IAState.Defensiva:
				//Querra estar centrada entre la porteria y la pelota
				desiredX = (ball.transform.position.x + myGoal.transform.position.x) / 2;

				//A no ser que se meta en la porteria
				if (this.tag == "Team1" && desiredX < -6f)
					desiredX = -6f;
				else if (this.tag == "Team2" && desiredX > 6f)
					desiredX = 6f;
				break;

			case IAState.Agresiva:
				//Si es del equipo 1 quiere estar un poco a la izquierda de la bola, y si es del 2, a la derecha
				if (this.tag == "Team1") {
					desiredX = ball.transform.position.x - 0.3f;
				} else if (this.tag == "Team2") {
					desiredX = ball.transform.position.x + 0.3f;
				} 
				break;

			default:
				break;
		}

		if (this.transform.position.x < desiredX)
			MoveRight();
		else if (this.transform.position.x > desiredX)
			MoveLeft();

		//Si la bola esta cerca
		if (bolaCerca) {
			if (Random.Range(0, 10) < this.IAnivel)
				this.Jump ();
		}
	}

	public void Agacharse () {
		
	}

	public void ControlarBufos() {
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
			ResetJump ();
		}

		if (contSize > 0) {
			contSize -= Time.deltaTime;
		} 
		if ((isBig || isSmall) && contSize <= 0) {
			ResetSize ();
		} 
	}

	public void SwitchGravity ()
	{
		if (!gravitySwap) {
			this.transform.Rotate (new Vector3 (180f, 0f, 0f));
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
		this.salto = saltoInicial;
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

public enum IAState { Agresiva, Defensiva }