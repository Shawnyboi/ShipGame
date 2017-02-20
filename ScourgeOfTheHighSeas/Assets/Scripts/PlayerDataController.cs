using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


/// <summary>
/// This class will be responsible for keeping track of the ships in the players party and their attributes as well as
/// the players gold and inventory (if inventory will be in the game)
/// </summary>
public class PlayerDataController : MonoBehaviour {

	private string m_ShipFolderName = "Ships";
	private string m_StarterShipName = "Ship";
	private int m_StarterFleetSize = 3;
	private int m_StartingGold = 100;



	//the player fleet must be a list of easily copied and controlled structs so they can be saved and kept between scenes
	private List<ShipAttributesData> m_PlayerFleet;

	private int m_PlayerGold;

	void Awake () {
		DontDestroyOnLoad (gameObject);
	}

	void Start(){

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/// <summary>
	/// Get the prefab instance of the ship in the fleet 
	/// </summary>
	/// <returns>The ship from fleet.</returns>
	/// <param name="index">Index in fleet.</param>
	public GameObject GetShipFromFleet(int index){
		Debug.Log("get ship from fleet path: "+ m_ShipFolderName + "/" + m_PlayerFleet [index].shipName);
		GameObject ship = Resources.Load (m_ShipFolderName + "/" + m_PlayerFleet [index].shipName) as GameObject;
		return ship; 
	}

	/// <summary>
	/// Gets the ship attributes data struct from fleet at the index passed in by the parameter.
	/// </summary>
	/// <returns>The ship attributes data struct from m_PlayerFleet in PlayerDataController.</returns>
	/// <param name="index">Index in fleet.</param>
	public ShipAttributesData GetShipAttributesDataFromFleet(int index){
		return m_PlayerFleet [index];
	}

	/// <summary>
	/// A function to get the list of ship data 
	/// </summary>
	/// <returns>The fleet data.</returns>
	public List<ShipAttributesData> GetFleetData(){ 
		return m_PlayerFleet;
	}

	/// <summary>
	/// Set the player fleet and player gold
	/// Data should be coming in from a save file
	/// </summary>
	/// <param name="fleetData">Fleet data.</param>
	/// <param name="playerGold">Player gold.</param>
	public void PassInPlayerData(List<ShipAttributesData> fleetData, int playerGold){
		m_PlayerFleet = fleetData;
		m_PlayerGold = playerGold;
	}

	/// <summary>
	/// Gets the player gold.
	/// </summary>
	/// <returns>The player gold.</returns>
	public int GetPlayerGold(){
		return m_PlayerGold;
	}

	/// <summary>
	/// Changes the player gold.
	/// Can add or subtract depending on if the parameter is negative
	/// </summary>
	/// <param name="amount">Amount.</param>
	public void ChangePlayerGold(int amount){
		m_PlayerGold += amount;
	}



	/// <summary>
	/// Add a ship to the fleet by passing in its prefab name
	/// </summary>
	/// <param name="shipName">Ship name.</param>
	public void AddShipToFleet(string shipName){
		GameObject ship = Resources.Load(m_ShipFolderName + "/" + shipName) as GameObject;
		ShipAttributes shipAttributes = ship.GetComponent<ShipAttributes> ();
		ShipAttributesData shipAttributesData = shipAttributes.CreateShipAttributesData ();
		m_PlayerFleet.Add (shipAttributesData);
	}

	/// <summary>
	/// Add a ships ship attributes data to the fleet by passing in the ship itself
	/// </summary>
	/// <param name="ship">the ship object.</param>
	public void AddShipToFleet(GameObject ship){
		m_PlayerFleet.Add (ship.GetComponent<ShipAttributes> ().CreateShipAttributesData());
	}

	/// <summary>
	/// Take a ship out of the fleet
	/// </summary>
	/// <param name="index">Index of the ship, defaults as zero.</param>
	public void RemoveShipFromFleet(int index = 0){
		m_PlayerFleet.RemoveAt (index);
	}

	/// <summary>
	/// Take several ships out of a fleet
	/// </summary>
	/// <param name="index">index of where to start removing</param>
	/// <param name="count">number of ships to remove.</param>
	public void RemoveShipsfromFleet(int index , int count){
		m_PlayerFleet.RemoveRange(index, count);
	}
	/// <summary>
	/// Get the number of ships in the fleet
	/// </summary>
	/// <returns>The fleet size.</returns>
	public int GetFleetSize(){
		return m_PlayerFleet.Count;
	}

	/// <summary>
	/// Sets the current hull strength of each ship to its maximum
	/// </summary>
	public void RepairFleet(){
		for (int i = 0; i < m_PlayerFleet.Count; i++) {
			m_PlayerFleet [i].currentHullStrength = m_PlayerFleet [i].maxHullStrength;
		}

	}

	/// <summary>
	/// Initializes the player data controller by constructing the fleet list and filling it with the appropriate ships
	/// </summary>
	/// <param name="newGame">If set to <c>true</c> new game.</param>
	/// <param name="saveIndex"> the index where the save file is held</param>
	public void InitializePlayerDataController(bool newGame = true, int saveIndex = 0){
		m_PlayerFleet = new List<ShipAttributesData> ();

		if (newGame) {
			m_PlayerGold = m_StartingGold;
			for (int i = 0; i < m_StarterFleetSize; i++) {
				AddShipToFleet (m_StarterShipName); //if its a new game we make the fleet three generic ships
			}
		} 
	}



}

/// <summary>
/// The ship attributes data struct is necesarry for storing individual ship data between levels
/// It contains all the data necesarry for instantiating the ships attributes as the player would remember them
/// </summary>
[Serializable]
public class ShipAttributesData{

	public float speed;
	public float turningSpeed;
	public float stunBlockTime;
	public float maxHullStrength;
	public float currentHullStrength;
	public float range;
	public float damage;
	public float reloadTime;
	public float percentAccuracy;
	public float shotVarianceDegrees;
	public float launchSpeed;
	public string shipName;

}

