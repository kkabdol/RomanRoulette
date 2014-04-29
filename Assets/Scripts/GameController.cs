using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
	public int numOfVictims = 3;
	public GameObject victimPrefab;
	public GameObject killSign;

	private List<GameObject> victimList = new List<GameObject> ();
	private int curVictimIndex = 0;
	private bool hasVictimChoosed = false;
	private GameObject chosenVictim;

	public const float delay = 1f;
	private float elapsedTime = 0f;
	private int count = 0;


	void Awake ()
	{
		Reset ();
	}

	void Update ()
	{
		if (hasVictimChoosed == true) {
			if (victimList.Count > 1) {
				elapsedTime += Time.deltaTime;

				if (elapsedTime > delay) {
					elapsedTime = 0;

					if (count > 2) {
						count = 0;

						GameObject killed = victimList [curVictimIndex];

						if (killed == chosenVictim) {
							Reset ();
						} else {
							victimList.RemoveAt (curVictimIndex);
							--curVictimIndex;

							Destroy (killed);
						}
					} else {
						++count;
						curVictimIndex = (curVictimIndex + 1) % victimList.Count;
						MoveSign (victimList [curVictimIndex]);
					}
				}
			} else {
				++numOfVictims;
				Reset ();
			}
		}
	}

	void Reset ()
	{
		hasVictimChoosed = false;
		elapsedTime = 0f;
		count = 3;

		GenerateVictims ();
		ChooseFirstVictim ();
	}
		 
	void GenerateVictims ()
	{
		for (int i=0, imax=victimList.Count; i<imax; ++i) {
			Destroy (victimList [i]);
		}
		victimList.Clear ();

		float deltaRotY = 360f / numOfVictims;
		for (int i=0; i<numOfVictims; ++i) {
			Quaternion rot = Quaternion.Euler (0f, deltaRotY * i, 0f);
			GameObject victim = Instantiate (victimPrefab, Vector3.zero, rot) as GameObject;
			victimList.Add (victim);
		}
	}

	void ChooseFirstVictim ()
	{
		curVictimIndex = Random.Range (0, victimList.Count);
		MoveSign (victimList [curVictimIndex]);
	}

	void MoveSign (GameObject victim)
	{
		killSign.transform.position = victim.GetComponent<Victim> ().body.transform.position;
	}

	public void ChooseVictim (GameObject chosen)
	{
		for (int i=0, imax=victimList.Count; i<imax; ++i) {
			if (victimList [i] == chosen) {
				chosenVictim = chosen;
				hasVictimChoosed = true;
				break;
			}
		}
	}
}
