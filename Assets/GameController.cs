using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
	public int numOfVictims = 3;
	public GameObject victimPrefab;
	public GameObject killSign;

	private List<GameObject> victimList = new List<GameObject> ();
	private int firstVictimIndex = 0;

	void Awake ()
	{
		GenerateVictims ();
		ChooseFirstVictim ();
	}
		 
	void GenerateVictims ()
	{
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
		firstVictimIndex = Random.Range (0, victimList.Count);
		killSign.transform.position = victimList [firstVictimIndex].GetComponent<Victim> ().victimBody.transform.position;
	}
}
