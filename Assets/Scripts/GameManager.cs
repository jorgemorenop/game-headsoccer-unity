using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public GUISkin theSkin;

	public int porcentajePowerUps;

	private int team1Score, team2Score;

	private GameObject[] playersT1, playersT2;
	private GameObject ball;

	private float timer = 60, timer2;

	private bool timerStopped, thingsCanMove, pausado;


	//Setters y getters
	public bool getTimerStopped() {
		return this.timerStopped;
	}
	public float getTimer() {
		return this.timer;
	}

	// Use this for initialization
	void Start () {
		playersT1 = GameObject.FindGameObjectsWithTag ("Team1");
		playersT2 = GameObject.FindGameObjectsWithTag ("Team2");
		ball = GameObject.FindGameObjectWithTag("Ball");

		team1Score = 0; team2Score = 0;

		//Para que empiece con un contador
		timerStopped = true;
		thingsCanMove = false; 
		timer2 = 3f;
		Bloquear();
	}

	// Update is called once per frame
	void Update () {
		//Si no esta parado ni pausado, que siga el juego
		if (!timerStopped && !pausado) {
			timer -= Time.deltaTime;
			GeneraPowerUp();
		}
		//Si esta parado es que ha habido un gol
		else if (!pausado) {
			timer2 -= Time.deltaTime;

			if (timer2 < 3f && thingsCanMove) {
				ResetAllGameObjectsPosition();
				Bloquear();
			}
			if (timer2 < 0) { //Cuando ha habido un gol y hay que reiniciar
				GetComponent<AudioSource>().Play(); //whistle
				timerStopped = false;
				Desbloquear();
			}				
		}

		if (timer <= -4)
		{
			Application.LoadLevel(0);
		}
	}

	//Bloqueamos a los jugadores y la pelota para que no se puedan mover
	public void Bloquear() {
		foreach (GameObject p in playersT1)
			p.GetComponent<Rigidbody2D>().isKinematic = true;
		foreach (GameObject p in playersT2)
			p.GetComponent<Rigidbody2D>().isKinematic = true;
		ball.GetComponent<Rigidbody2D>().isKinematic = true;
		thingsCanMove = false;
	}

	public void Desbloquear() {
		foreach (GameObject p in playersT1)
			p.GetComponent<Rigidbody2D>().isKinematic = false;
		foreach (GameObject p in playersT2)
			p.GetComponent<Rigidbody2D>().isKinematic = false;
		ball.GetComponent<Rigidbody2D>().isKinematic = false;
		thingsCanMove = true;
	}

	public void Pausar() {
		pausado = true;
		Bloquear();
	}

	public void Reanudar() {
		pausado = false;
		Desbloquear();
	}

	public void ResetAllGameObjectsPosition() {
		//Recolocamos los jugadores y la bola
		foreach (GameObject p in playersT1)
			p.GetComponent<Player>().ResetPosition();
		foreach (GameObject p in playersT2)
			p.GetComponent<Player>().ResetPosition();
		ball.GetComponent<BallScript>().ResetPosition();

		GameObject[] bufos = GameObject.FindGameObjectsWithTag("PowerUp");
		foreach (GameObject bufo in bufos)
			Destroy(bufo);
	}

	public void Goal(int equipo) {
		if (!timerStopped && timer > 0) {
			switch (equipo) {
				//porteria izquierda
				case 1: 
					team2Score++; break;
				//porteria derecha
				case 2:
					team1Score++; break;
				default: 
					break;
			}
		}

		timerStopped = true;
		timer2 = 6f;
	}

	public void GeneraPowerUp() {
		if (Random.Range(0,1000) < porcentajePowerUps) {
			Debug.Log("CReanod");

			GameObject instance = (GameObject) Instantiate(Resources.Load("PowerUp"));
			instance.tag = "PowerUp";
		
			//Aqui luego le pondre que se generen en un lugar aleatorio
			float x = UnityEngine.Random.Range(-6f,6f);
			float y = UnityEngine.Random.Range(-2f, 2f);

			instance.transform.position = new Vector3 (x, y, 5);
		}
	}

	public void OnGUI() {
		GUI.skin = theSkin;
		GUI.Label (new Rect (3*Screen.width/10, 20, Screen.width/10, 100), "" + team1Score);
		GUI.Label (new Rect (6*Screen.width/10, 20, Screen.width/10, 100), "" + team2Score);

		if (timer > 0) {
			GUI.Label (new Rect (Screen.width/2 - 20, 100, 100, 100), "" + Mathf.Round(timer) );
		}

		if (timerStopped && timer2 < 3f)
			GUI.Label (new Rect (Screen.width/2 - 20, 250, 100, 100), "" + Mathf.Ceil(timer2) );

		if (GUI.Button(new Rect(8*Screen.width/10, 0, Screen.width/10, Screen.height/8), "Pausa")) {
			if (!pausado)
				Pausar();
			else
				Reanudar();
		}
			
		if (GUI.Button(new Rect(9*Screen.width/10, 0, Screen.width/10, Screen.height/8), "Salir"))
			Application.LoadLevel(0);
			
	}
}