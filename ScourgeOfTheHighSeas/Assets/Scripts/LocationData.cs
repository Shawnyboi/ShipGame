using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
	//These indeces correspond with the dialoguer dialogues
	public List<int> m_EventIndeces;

	//constructor
	public LocationData (){
		m_LocationConnections = new List<int> ();
		m_EventIndeces = new List<int> ();
	}



}

