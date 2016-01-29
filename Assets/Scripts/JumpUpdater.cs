using UnityEngine;
using System.Collections;

public class JumpUpdater : MonoBehaviour {
	
	public void OnTriggerEnter2D(Collider2D coll) {
		//Si es un jugador, la bola o el borde del mapa
		if (coll.tag == "Team1" || coll.tag == "Team2" || coll.tag == "Ball" || coll.tag == "StageBorder")
			this.transform.parent.gameObject.GetComponent<Player>().setJump (true);

		if (coll.tag == "Team1" || coll.tag == "Team2")
			if (coll.gameObject.GetComponent<Player>().getAgachado())
				this.transform.parent.gameObject.GetComponent<Player>().MegaJump();
	}
}
