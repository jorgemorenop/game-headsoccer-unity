using UnityEngine;
using System.Collections;

public class GMScript : MonoBehaviour {
	float timer = 60;

	public int porcentajePowerUps = 400;
	int playerScore01, playerScore02, timer2 = -1;
	
	GameObject[] playersT1;
	GameObject[] playersT2;
	GameObject ball;
	public static string lastPlayer = "Team1"; //Equipo al que pertenece el ultimo jugador en tocar la bola. Puede ser Team1 o Team2

	public AudioSource whistleSound;
	bool canPlayWhistle = true;
	
	public static bool timerStopped = false, goalPlayer1 = false, goalPlayer2 = false;
	
	public GUISkin theSkin;

	public static GMScript currentGame;

	GameObject instance;
	
	// Use this for initialization
	void Start () {
		playerScore01 = 0;
		playerScore02 = 0;
		goalPlayer1 = false;
		goalPlayer2 = false;
		
		playersT1 = GameObject.FindGameObjectsWithTag ("Team1");
		playersT2 = GameObject.FindGameObjectsWithTag ("Team2");
		ball = GameObject.FindGameObjectWithTag("Ball");

		currentGame = this;

		StartCoroutine(stopThings());	
	}
	
	// Update is called once per frame
	void Update () {
		if (!timerStopped) {
			timer -= Time.deltaTime;
			GeneraPowerUp ();
		}

		if (timer <= -4)
		{
			Application.LoadLevel(0);
		}

	}
	
	public IEnumerator Goal(string wallName){

		//If valid goal
		if (timer > 0 && !goalPlayer1 && !goalPlayer2) {
			timerStopped = true;
			if (wallName == "RightGoal")
			{
				goalPlayer1 = true;
				yield return new WaitForSeconds(2);
				goalPlayer1 = false;

				playerScore01 += 1;
			}
			else if (wallName == "LeftGoal")
			{
				goalPlayer2 = true;
				yield return new WaitForSeconds(2);
				goalPlayer2 = false;

				playerScore02 += 1;
			}


			for (int i=0; i<playersT1.Length; i++) 
			{
				playersT1[i].SendMessage("ResetPlayer");	
			}
			for (int i=0; i<playersT2.Length; i++) 
			{
				playersT2[i].SendMessage("ResetPlayer");	
			}

			ball.SendMessage ("ResetBall");

			timerStopped = false;

			StartCoroutine(stopThings());
		}
	}
	
	void OnGUI(){
		GUI.skin = theSkin;
		GUI.Label (new Rect (40, 20, 100, 100), "" + playerScore01);
		GUI.Label (new Rect (Screen.width - 60, 20, 100, 100), "" + playerScore02);
		if (timer > 0) {
			GUI.Label (new Rect (Screen.width/2 - 20, 100, 100, 100), "" + Mathf.Round(timer) );
		}

		//game has ended
		if (timer < 0) {
			if (playerScore01 == playerScore02) {
				GUI.Label (new Rect (Screen.width/2 - 87, Screen.height/2, 400, 100), "It's a TIE");
			}
			else if (playerScore01 < playerScore02) {
				GUI.Label (new Rect (Screen.width/2 - 115, Screen.height/2, 400, 100), "Team 2 WINS");
			}
			else {
				GUI.Label (new Rect (Screen.width/2 - 115, Screen.height/2, 400, 100), "Team 1 WINS");
			}
		}

		if (goalPlayer1) {
			GUI.Label (new Rect (Screen.width/2 - 120, Screen.height/2, 400, 100), "GOAL TEAM 1!");	
		}
		else if (goalPlayer2){
			GUI.Label (new Rect (Screen.width/2 - 120, Screen.height/2, 400, 100), "GOAL TEAM 2!");
		}
		else if (timerStopped && timer > 0) {
			if (timer2 > 0){
				GUI.Label (new Rect (Screen.width/2-10, Screen.height/2+30, 400, 100), ""+timer2);
				if (timer2 == 3) {
					//We delete the current Power Ups
					GameObject[] powerUps = GameObject.FindGameObjectsWithTag ("PowerUp");
					for (int i = 0; i < powerUps.Length; i++) {
						Destroy (powerUps [i].gameObject);
					}
				}
			}
			else if (timer2 == 0){
				GUI.Label (new Rect (Screen.width/2-30, Screen.height/2+30, 400, 100), "GO!");
				StartCoroutine( playWhistle() );

			}
		}
		
		if (GUI.Button (new Rect (Screen.width/2 - 126, 35, 121, 53), "RESET") )
		{
			GetComponent<AudioSource>().Play ();
			ResetGame();
		} 
		if (GUI.Button (new Rect (Screen.width/2 + 5, 35, 121, 53), "QUIT") )
		{
			GetComponent<AudioSource>().Play ();
			Application.LoadLevel(0);
		} 
		
	}

	public void ResetGame(){
		playerScore01 = 0;
		playerScore02 = 0;
		timer = 60;

		goalPlayer1 = false;
		goalPlayer2 = false;

		ResetPositions ();

	}
	

	public void ResetPositions(){
		for (int i=0; i<playersT1.Length; i++) 
		{
			playersT1[i].SendMessage("ResetPlayer");	
		}
		for (int i=0; i<playersT2.Length; i++) 
		{
			playersT2[i].SendMessage("ResetPlayer");	
		}

		ball.SendMessage ("ResetBall");

		StartCoroutine(stopThings());
	}

	//Reproduce el sonido del silbato si puede
	IEnumerator playWhistle () {
		if (canPlayWhistle) {
			whistleSound.Play ();
		}
		canPlayWhistle = false;
		yield return new WaitForSeconds (2);
		canPlayWhistle = true;
	}

	//Method that disables player input and ball physics for 2 seconds. 
	IEnumerator stopThings() {
		timerStopped = true;
		ball.SendMessage ("SetCanSpeedChange", false);
		timer2 = 3; 
		while (timer2>-1) {
			yield return new WaitForSeconds (1);
			timer2--;
		}
		ball.SendMessage ("SetCanSpeedChange", true);
		timerStopped = false;
	}

	public void GeneraPowerUp(){
		if (UnityEngine.Random.Range(0,1000) < porcentajePowerUps){
			instance = (GameObject) Instantiate(Resources.Load("PowerUp"));
			instance.tag = "PowerUp";
		
			//Aqui luego le pondre que se generen en un lugar aleatorio
			int x = UnityEngine.Random.Range(-4,4);
			int y = UnityEngine.Random.Range(-3,4);

			instance.transform.position = new Vector3 (x, y, 5);
		}
	}

}
