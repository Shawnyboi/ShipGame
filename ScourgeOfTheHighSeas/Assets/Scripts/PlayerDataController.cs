using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This class will be responsible for keeping track of the ships in the players party and their attributes as well as
/// the players gold and inventory (if inventory will be in the game)
/// </summary>
public class PlayerDataController : MonoBehaviour {

	public List<GameObject> m_PlayerFleet;

	void Awake () {
		DontDestroyOnLoad (gameObject);
	}

	void Start(){

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}


