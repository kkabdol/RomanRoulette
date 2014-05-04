using UnityEngine;
using System.Collections;

public class Victim : MonoBehaviour
{
	public GameObject body;
	public GameObject selectSign;

	private GameController gameController;

	void Awake ()
	{
		HideBody ();
		gameController = GameObject.FindGameObjectWithTag (Tags.gameController).GetComponent<GameController> ();
	}

	void Update ()
	{
		if (gameController.curState == GameState.Choose &&
			Input.GetMouseButtonUp (0)) {

			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;

			if (selectSign.collider.Raycast (ray, out hit, float.MaxValue)) {
				if (hit.collider == selectSign.collider) {
					SelectSelf ();
				}
			}
		}
	}

	public void StartTwinkling ()
	{
		selectSign.SetActive (true);
		iTween.FadeTo (selectSign, iTween.Hash ("alpha", 0f, "looptype", iTween.LoopType.pingPong, "time", 1f));
	}

	public void StopTwinkling ()
	{
		iTween.Stop (selectSign);
		selectSign.SetActive (false);
	}

	private void SelectSelf ()
	{
		body.renderer.material.color = Color.red;
		gameController.ChooseVictim (this.gameObject);
	}

	public void ShowBody ()
	{
		body.SetActive (true);
	}

	private void HideBody ()
	{
		body.SetActive (false);
	}
}
