using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class controls the Location marker objects in the overworld
/// </summary>
public class LocationMarkerController : MonoBehaviour {

	public int m_LocationIndex;

	public Canvas m_NameCanvas;
	public RectTransform m_NameRectTransform;
	public Vector3 m_NameVerticalDisplacement;

	private Transform m_Transform;


	private void Awake(){

		m_Transform = gameObject.transform;
		m_NameCanvas.worldCamera = Camera.main;
		m_NameCanvas.enabled = false;

	}


	private void Update(){

		m_NameCanvas.enabled = false;

	}
	private void FixedUpdate(){

		m_NameRectTransform.position = m_Transform.position + m_NameVerticalDisplacement;

	}



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

		m_NameCanvas.enabled = true;

	}

}
