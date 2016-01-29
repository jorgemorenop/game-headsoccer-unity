using UnityEngine;
using System.Collections;

public class GoalScript : MonoBehaviour {

	public int equipo;
	private GameObject controller;

	void Start() {
		this.controller = GameObject.FindWithTag ("GameController");
	}

	 //Este metodo es llamado cada vez que las porterias detectan una colision
	 void OnTriggerEnter2D (Collider2D hitInfo) {
		//Si es de una pelota
		if (hitInfo.name == "Ball" && !controller.GetComponent<GameManager> ().getTimerStopped() && controller.GetComponent<GameManager> ().getTimer() > 0f) {
			controller.GetComponent<GameManager> ().Goal (this.equipo); //mandamos el goal
			GetComponent<AudioSource>().Play();	 //play sound
		}
	}
}
