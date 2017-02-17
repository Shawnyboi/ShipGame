using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;

/// <summary>
/// This class will be responsible for keeping track of where the player has been and which areas are inaccessible;
/// It will also keep track of the information of which enemies can show up in each location for the different levels of difficulty
/// It will interface with the levelmanager in order to tell it what to spawn
/// It is also one of the three classes that persist beyond each individual scene
/// </summary>
public class OverworldDataController : MonoBehaviour {


	public GameObject[] m_GenericEnemyShips;
	private List<LocationData> m_LocationDataList;
	private LevelData m_CurrentLevelData;

	private string m_LocationDataFile = "location_data_objects";
	private string m_LevelDataFile = "level_data_objects";
	private string m_EnemyShipFolderName = "EnemyShips";

	private int m_PlayerLocation;

	void Awake () {
		
		m_LocationDataList = new List <LocationData>();
		LoadLocationData (m_LocationDataFile);
		DontDestroyOnLoad (gameObject);

	}

	/// <summary>
	/// This function will return an array of ship prefabs dependant on which level index is passed in
	/// Different levels have different types of enemy ships and they are usually somewhat randomly generated
	/// </summary>
	/// <returns>The enemy ships for level.</returns>
	/// <param name="levelSceneIndex">Level scene index.</param>
	public GameObject[] GetEnemyShipsForLevel(int levelSceneIndex){

		LoadLevelData (levelSceneIndex, m_LevelDataFile);
		return CreateEnemyShipArrayFromLevelData ();

	}

	/// <summary>
	/// A simple getter for the overworld controller to call to initialize itself
	/// </summary>
	/// <returns>The player location index.</returns>
	public int GetPlayerLocation(){
		return m_PlayerLocation;
	}

	/// <summary>
	/// Sets the player location index
	/// </summary>
	/// <param name="newLocationIndex">New location index of the location button as tracke in the overworld controller.</param>
	public void SetPlayerLocation(int newLocationIndex){
		m_PlayerLocation = newLocationIndex;
	}

	/// <summary>
	/// Initializes the overworld data controller by setting the player's current location to the aprropriate initial location.
	/// </summary>
	/// <param name="newGame">If set to <c>true</c> new game.</param>
	/// <param name="saveIndex"> the index where the save file is held</param>
	public void InitializeOverworldDataController(bool newGame = true, int saveIndex = 0){
		if (newGame) {
			m_PlayerLocation = 0;
		} else if (saveIndex == 0) {
			//load saved position
		}
	}
	/// <summary>
	/// This function loads data in from the xml file pertaining to the location information
	/// This is run when the overworld data controller is created.
	/// </summary>
	/// <param name="dataPath">Data path.</param>
	private void LoadLocationData(string dataPath){

		//get the xml file as a string and turn that into an "XmlDocument" Object
		TextAsset locationDataFile = Resources.Load (dataPath) as TextAsset;
		XmlDocument xmlDocument = new XmlDocument ();
		xmlDocument.LoadXml (locationDataFile.text);

		//loop through elements of document and parse information
		foreach (XmlNode location in xmlDocument["Locations"].ChildNodes) {

			//each iteration will create a new location data object
			LocationData locData = new LocationData ();
			locData.m_LocationName = location.Attributes ["name"].Value;
			int locIndex;
			int.TryParse (location ["Location_Index"].InnerText, out locIndex);
			locData.m_LocationIndex = locIndex;

			foreach (XmlNode connection in location["Location_Connections"].ChildNodes) {
				
				int locConnection;
				int.TryParse (connection.InnerText, out locConnection);
				locData.m_LocationConnections.Add (locConnection);  

			}

			foreach (XmlNode eventIndex in location["Event_Indeces"].ChildNodes) {
				
				int locEventIndex;
				int.TryParse (eventIndex.InnerText, out locEventIndex);
				locData.m_EventIndeces.Add (locEventIndex);

			}

			//add each created location data object to the member list
			m_LocationDataList.Add (locData);

		}

		//order list by location index value
		m_LocationDataList.Sort ((x, y) => x.m_LocationIndex.CompareTo (y.m_LocationIndex));

	}

	/// <summary>
	/// This function loads the information from an xml file into a scenedata object and stores it in the memeber scene data object of this class
	/// </summary>
	/// <param name="sceneIndex">index to identitfy the scene to be loaded.</param>
	private void LoadLevelData(int sceneIndex, string dataPath){
		
		//get the xml file as a string and turn that into an "XmlDocument" Object
		TextAsset locationDataFile = Resources.Load (dataPath) as TextAsset;
		XmlDocument xmlDocument = new XmlDocument ();
		xmlDocument.LoadXml (locationDataFile.text);

		LevelData levelData = new LevelData();

		foreach (XmlNode scene in xmlDocument["Scenes"].ChildNodes) {
			
			string stringAttribute = scene.Attributes ["index"].Value;
			int intAttribute;
			int.TryParse (stringAttribute, out intAttribute);

			if (intAttribute == sceneIndex) {

				levelData.m_Boss = scene ["Boss"].InnerText;

				foreach (XmlNode enemy in scene["Enemies"].ChildNodes) {
					levelData.m_Enemies.Add (enemy.InnerText);
				}
			}
		}

		m_CurrentLevelData = levelData;

	}

	/// <summary>
	/// This function can be called to check if a desired location is connected to the current location
	/// Generally will be called from the overworld controller when the player tries to move
	/// </summary>
	/// <returns><c>true</c> if location connected <c>false</c> otherwise.</returns>
	/// <param name="currentLocationIndex">Current location index.</param>
	/// <param name="desiredLocationIndex">Desired location index.</param>
	public bool CheckIfLocationConnected(int currentLocationIndex,int desiredLocationIndex){
		foreach(int connection in m_LocationDataList[currentLocationIndex].m_LocationConnections){
			if (connection == desiredLocationIndex) {
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// This summary returns a random Event index associated with a location
	/// </summary>
	/// <param name="locationIndex">Location index.</param>
	public int GetRandomEventIndexAtLocation(int locationIndex){
		return m_LocationDataList [locationIndex].m_EventIndeces [Random.Range (0, m_LocationDataList [locationIndex].m_EventIndeces.Count - 1)];
	}


	private GameObject[] CreateEnemyShipArrayFromLevelData(){
		
		if (m_CurrentLevelData == null) {
			Debug.Log ("Scene data not loaded properly");
			return null;
		}

		GameObject[] shipArray = new GameObject[m_CurrentLevelData.m_Enemies.Count];

		for (int i = 0; i < shipArray.Length; i++) {
			
			GameObject ship = Resources.Load (m_EnemyShipFolderName + "/" + m_CurrentLevelData.m_Enemies [i]) as GameObject;

			shipArray [i] = ship;

		}

		return shipArray;
	}


}


