using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GameState
{
	Ready,
	ShowSign,
	Choose,
	Execution,
	Result,
};

public class GameController : MonoBehaviour
{
	public GameState curState = GameState.Ready;
	public int numberOfVictims = 3;
	public GameObject victimPrefab;
	public GameObject killSign;
	public AudioSource rifleAudioSource;
	public Transform riflePivot;
	public AudioClip clipYouKilledMe;
	public AudioClip clipDying;

	public AudioListener listener;

	private List<GameObject> victimList = new List<GameObject> ();
	private int curVictimIndex = 0;
	private GameObject chosenVictim;

	void Awake ()
	{
		Reset ();
	}

	#region ready state

	void Reset ()
	{
		ChangeGameState (GameState.Ready);

		HideSign ();
		ClearVictimList ();
		StartCoroutine (GenerateVictims ());

	}

	void ChangeGameState (GameState newState)
	{
		curState = newState;

		switch (newState) {
		case GameState.ShowSign:
			StartCoroutine (ShowSign ());
			break;
		case GameState.Execution:
			StartCoroutine (Execution ());
			break;
		default:
			break;
		}
	}

	void ClearVictimList ()
	{
		for (int i=0, imax=victimList.Count; i<imax; ++i) {
			Destroy (victimList [i]);
		}
		victimList.Clear ();
	}

	IEnumerator GenerateVictims ()
	{
		float deltaRotY = 360f / numberOfVictims;
		for (int i=0; i<numberOfVictims; ++i) {
			Quaternion rot = Quaternion.Euler (0f, deltaRotY * i, 0f);
			GameObject victim = Instantiate (victimPrefab, Vector3.zero, rot) as GameObject;
			victimList.Add (victim);

			yield return new WaitForSeconds (1f);
		}

		ChangeGameState (GameState.ShowSign);
	}

	void StartTwinklingToAllVictims ()
	{
		for (int i=0, imax=victimList.Count; i<imax; ++i) {
			victimList [i].GetComponent<Victim> ().StartTwinkling ();
		}
	}

	void ChooseFirstVictim ()
	{
		curVictimIndex = Random.Range (0, victimList.Count);
		MoveSign (victimList [curVictimIndex]);
	}

	void HideSign ()
	{
		killSign.SetActive (false);
	}

	void MoveSign (GameObject victim)
	{
		if (killSign.activeSelf == false) {
			killSign.SetActive (true);
		}
		killSign.transform.position = victim.GetComponent<Victim> ().body.transform.position;
	}

	#endregion

	#region choose state
	IEnumerator ShowSign ()
	{
		ChooseFirstVictim ();
		yield return new WaitForSeconds (1f);
		
		StartTwinklingToAllVictims ();
		ChangeGameState (GameState.Choose);
	}
	#endregion

	#region choose state
	public void ChooseVictim (GameObject chosen)
	{
		for (int i=0, imax=victimList.Count; i<imax; ++i) {
			if (victimList [i] == chosen) {
				chosenVictim = chosen;
				ChangeGameState (GameState.Execution);
				MoveAudioListener (chosen.GetComponent<Victim> ().body.transform);
				break;
			}
		}
	}

	void MoveAudioListener (Transform transform)
	{
		listener.gameObject.transform.position = transform.position;
		listener.gameObject.transform.rotation = transform.rotation;
	}
	#endregion

	#region execution state

	void ShowAllVictims ()
	{
		for (int i=0, imax=victimList.Count; i<imax; ++i) {
			Victim v = victimList [i].GetComponent<Victim> ();
			v.StopTwinkling ();
			v.ShowBody ();
		}
	}

	IEnumerator Execution ()
	{
		ShowAllVictims ();

		int count = 3;
		while (victimList.Count > 1) {
			yield return new WaitForSeconds (1f);

			if (count > 2) {

				PlayRifleShotOnRandomPosition ();
				GameObject killed = victimList [curVictimIndex];
				
				if (killed == chosenVictim) {
					AudioSource.PlayClipAtPoint (clipYouKilledMe, killed.GetComponent<Victim> ().body.transform.position);
					Reset ();
					yield break;
				} else {
					victimList.RemoveAt (curVictimIndex);
					--curVictimIndex;

					AudioSource.PlayClipAtPoint (clipDying, killed.GetComponent<Victim> ().body.transform.position);
					Destroy (killed);
				}

				count = 0;
			} else {
				++count;
				curVictimIndex = (curVictimIndex + 1) % victimList.Count;
				MoveSign (victimList [curVictimIndex]);
			}
		}

		++numberOfVictims;
		Reset ();
	}

	void PlayRifleShotOnRandomPosition ()
	{
		riflePivot.Rotate (Vector3.up, Random.Range (0, 360));
		rifleAudioSource.Play ();
	}
	#endregion
}
