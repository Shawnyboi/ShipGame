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

	private int m_PlayerLocation;

	void Awake () {
		m_LocationDataList = new List <LocationData>();
		LoadLocationData ("location_data_objects");
		DontDestroyOnLoad (gameObject);
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

			foreach (XmlNode sceneIndex in location["Scene_Indeces"].ChildNodes) {
				int locSceneIndex;
				int.TryParse (sceneIndex.InnerText, out locSceneIndex);
				locData.m_SceneIndeces.Add (locSceneIndex);
			}
			//add each created location data object to the member list
			m_LocationDataList.Add (locData);
		}

		//order list by location index value
		m_LocationDataList.Sort ((x, y) => x.m_LocationIndex.CompareTo (y.m_LocationIndex));

		foreach (LocationData locData in m_LocationDataList) {
			Debug.Log ("location index " + locData.m_LocationIndex);
			Debug.Log ("location name " + locData.m_LocationName);
			Debug.Log ("Connections: ");				
			foreach (int connection in locData.m_LocationConnections) {
				Debug.Log (connection);
			}
			Debug.Log ("Scene Indeces: ");
			foreach (int scene in locData.m_SceneIndeces) {
				Debug.Log (scene);
			}
		}

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
	/// This summary returns a random scene index associated with a location
	/// </summary>
	/// <param name="locationIndex">Location index.</param>
	public int GetRandomSceneIndexAtLocation(int locationIndex){
		return m_LocationDataList [locationIndex].m_SceneIndeces [Random.Range (0, m_LocationDataList [locationIndex].m_SceneIndeces.Count - 1)];
	}



}

/// <summary>
/// This class will contain information pertaining to a specific game location
/// They should be created by reading xml files
/// </summary>
//[XmlRoot ("Location")]
public class LocationData{


	//the name of this location 
	public string m_LocationName;
	//the identifier of this location
	public int m_LocationIndex;
	//the locations that can be traveled to from this location
	public List<int> m_LocationConnections;
	//the scenes associated with this location
	public List<int> m_SceneIndeces;

	//constructor
	public LocationData (){
		m_LocationConnections = new List<int> ();
		m_SceneIndeces = new List<int> ();
	}
	


}
