using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class will be responsible for all the functionality of the overworld map itself
/// It will be tied closely to the OverworldDatacontroller and the PlayerDataController
/// </summary>
public class OverworldController : MonoBehaviour {


	private bool m_LocationSelected;
	private int m_IndexOfLocationSelected;

	void Awake(){
		m_LocationSelected = false;
	}

	void Start(){
		Debug.Log ("calling start for overworldcontroller");
		StartCoroutine (WaitForLocationSelection());
	}

	public IEnumerator WaitForLocationSelection(){
		Debug.Log ("starting wait for selection coroutinie");
		while (!m_LocationSelected) {
			
			yield return null;

		}
		Debug.Log ("ending wait for selection coroutine");
		yield return null;

	}

	/// <summary>
	/// This function sets the selected location
	/// it is intended to be called by a unity button
	/// It will break the WaitForSelection loop and therby the overworld loop in the game manager thus starting the new level
	/// </summary>
	/// <param name="index">Index.</param>
	public void SelectLocation(int index){
		Debug.Log ("Selecting Location");
		m_LocationSelected = true;
		m_IndexOfLocationSelected = index;
	}

	public int GetSelectedLevelSceneIndex(){
		return m_IndexOfLocationSelected;
	}



}
