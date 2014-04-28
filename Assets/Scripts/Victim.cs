using UnityEngine;
using System.Collections;

public class Victim : MonoBehaviour
{
	public GameObject body;

	private GameController gameController;

	void Awake ()
	{
		gameController = GameObject.FindGameObjectWithTag (Tags.gameController).GetComponent<GameController> ();
	}

	void Update ()
	{
		if (Input.GetMouseButtonUp (0)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;

			if (body.collider.Raycast (ray, out hit, float.MaxValue)) {
				if (hit.collider == body.collider) {
					body.renderer.material.color = Color.red;
					gameController.ChooseVictim (this.gameObject);
				}
			}
		}
	}
}
