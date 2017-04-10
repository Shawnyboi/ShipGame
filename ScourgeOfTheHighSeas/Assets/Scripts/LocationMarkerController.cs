using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class controls the Location marker objects in the overworld
/// </summary>
public class LocationMarkerController : MonoBehaviour {

	public int m_LocationIndex;

	/// <summary>
	/// Gets the index of the location.
	/// </summary>
	/// <returns>The location index.</returns>
	public int GetLocationIndex(){
		return m_LocationIndex;
	}

	/// <summary>
	/// Makes the location marker change color and display its name
	/// </summary>
	public void Highlight(){
		
		//Debug.Log ("Highlighting location marker " + m_LocationIndex);

	}

}
