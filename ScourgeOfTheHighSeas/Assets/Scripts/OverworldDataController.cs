using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class will be responsible for keeping track of where the player has been and which areas are inaccessible;
/// It will also keep track of the information of which enemies can show up in each location for the different levels of difficulty
/// It will interface with the levelmanager in order to tell it what to spawn
/// It is also one of the three classes that persist beyond each individual scene
/// </summary>
public class OverworldDataController : MonoBehaviour {


	public GameObject[] m_GenericEnemyShips;

	void Awake () {
		DontDestroyOnLoad (gameObject);
	}

	void Start(){
		//m_PlayerDataController = GameObject.FindGameObjectWithTag ("PlayerData").GetComponent<PlayerDataController> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/// <summary>
	/// This function will return an array of ship prefabs dependant on which level index is passed in
	/// Different levels have different types of enemy ships and they are usually somewhat randomly generated
	/// </summary>
	/// <returns>The enemy ships for level.</returns>
	/// <param name="levelSceneIndex">Level scene index.</param>
	public GameObject[] GetEnemyShipsForLevel(int levelSceneIndex){
		//for now we just return the generic enemyships
		return m_GenericEnemyShips;

	}

}

