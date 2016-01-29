using UnityEngine;
using System.Collections;

public class PowerUpScript : MonoBehaviour {

	PowerType tipo;
	float lifeTime = 10;
	GameObject[] playersT1;
	GameObject[] playersT2;
	GameObject ball;

	// Use this for initialization
	void Start () {
		playersT1 = GameObject.FindGameObjectsWithTag ("Team1");
		playersT2 = GameObject.FindGameObjectsWithTag ("Team2");
		tipo = GetRandomEnum<PowerType> ();
		switch (tipo) {
			case PowerType.Big:
				this.gameObject.GetComponent<SpriteRenderer> ().sprite = Resources.Load("powerup_ball_bigger", typeof(Sprite)) as Sprite;
				break;
			case PowerType.Small:
				this.gameObject.GetComponent<SpriteRenderer> ().sprite = Resources.Load("powerup_ball_smaller", typeof(Sprite)) as Sprite;
				break;
			case PowerType.Gravity:
				this.gameObject.GetComponent<SpriteRenderer> ().sprite = Resources.Load("powerup_swap_gravity", typeof(Sprite)) as Sprite;
				break;
			case PowerType.MoreBouncy:
				this.gameObject.GetComponent<SpriteRenderer> ().sprite = Resources.Load("powerup_ball_more_bouncy", typeof(Sprite)) as Sprite;
				break;
			case PowerType.LessBouncy:
				this.gameObject.GetComponent<SpriteRenderer> ().sprite = Resources.Load("powerup_ball_less_bouncy", typeof(Sprite)) as Sprite;
				break;
			case PowerType.MoreJump:
				this.gameObject.GetComponent<SpriteRenderer> ().sprite = Resources.Load("powerup_more_power", typeof(Sprite)) as Sprite;
				break;
			case PowerType.LessJump:
				this.gameObject.GetComponent<SpriteRenderer> ().sprite = Resources.Load("powerup_less_power", typeof(Sprite)) as Sprite;
				break;
			case PowerType.BigPlayer:
				this.gameObject.GetComponent<SpriteRenderer> ().sprite = Resources.Load("powerup_player_bigger", typeof(Sprite)) as Sprite;
				break;
			case PowerType.SmallPlayer:
				this.gameObject.GetComponent<SpriteRenderer> ().sprite = Resources.Load("powerup_player_smaller", typeof(Sprite)) as Sprite;
				break;
		}

	}
	
	// Update is called once per frame
	void Update () {
		if (lifeTime > 0)
			lifeTime -= Time.deltaTime;
		else
			Destroy(this.gameObject);
	}


	void OnTriggerEnter2D (Collider2D hitInfo) {
		if (hitInfo.name == "Ball" && !GMScript.timerStopped) {
			Debug.Log("Bola ha tocado objeto " + this.tipo);
			ball = hitInfo.gameObject;
			Action ();
			Destroy(this.gameObject);
		}
	}

	void Action(){
		switch (tipo) {
			case PowerType.Big:
				ball.GetComponent<BallScript>().MakeBigger();
				break;

			case PowerType.Small:
				ball.GetComponent<BallScript>().MakeSmaller();
				break;

			case PowerType.Gravity:
				for (int i=0; i<playersT1.Length; i++) 
				{
					playersT1[i].GetComponent<Player>().SwitchGravity();	
				}
				for (int i=0; i<playersT2.Length; i++) 
				{
					playersT2[i].GetComponent<Player>().SwitchGravity();	
				}
				ball.GetComponent<BallScript>().SwitchGravity();
				break;

			case PowerType.MoreBouncy:
				ball.GetComponent<BallScript>().MakeMoreBouncy();
			    break;

			case PowerType.LessBouncy:
				ball.GetComponent<BallScript>().MakeLessBouncy();
				break;

			case PowerType.MoreJump:
				if (GMScript.lastPlayer == "Team1") {
					for (int i = 0; i < playersT1.Length; i++) {
						playersT1[i].GetComponent<Player>().MoreJump();
					}
				}
				else if (GMScript.lastPlayer == "Team2") {
					for (int i = 0; i < playersT2.Length; i++) {
						playersT2[i].GetComponent<Player>().MoreJump();
					}
				}
				break;

			case PowerType.LessJump:
				if (GMScript.lastPlayer == "Team1") {
					for (int i = 0; i < playersT2.Length; i++) {
						playersT2[i].GetComponent<Player>().LessJump();
					}
				}
				else if (GMScript.lastPlayer == "Team2") {
					for (int i = 0; i < playersT1.Length; i++) {
						playersT1[i].GetComponent<Player>().LessJump();
					}
				}
				break;

			case PowerType.BigPlayer:
				if (GMScript.lastPlayer == "Team1") {
					for (int i = 0; i < playersT1.Length; i++) {
						playersT1[i].GetComponent<Player>().MakeBigger();
					}
				}
				else if (GMScript.lastPlayer == "Team2") {
					for (int i = 0; i < playersT2.Length; i++) {
						playersT2[i].GetComponent<Player>().MakeBigger();
					}
				}
				break;

			case PowerType.SmallPlayer:
				if (GMScript.lastPlayer == "Team1") {
					for (int i = 0; i < playersT2.Length; i++) {
						playersT2[i].GetComponent<Player>().MakeSmaller();
					}
				}
				else if (GMScript.lastPlayer == "Team2") {
					for (int i = 0; i < playersT1.Length; i++) {
						playersT1[i].GetComponent<Player>().MakeSmaller();
					}
				}
				break;
		}

	}


	static T GetRandomEnum<T>()
	{
		System.Array A = System.Enum.GetValues(typeof(T));
		T V = (T) A.GetValue(UnityEngine.Random.Range(0,A.Length));
		return V;
	}


	enum PowerType{
		Big = 1, Small = 2, Gravity = 3, MoreBouncy = 4, LessBouncy = 5, MoreJump = 6, LessJump = 7, BigPlayer = 8, SmallPlayer = 9
	}

}
