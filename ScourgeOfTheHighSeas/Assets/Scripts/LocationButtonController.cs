using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationButtonController : MonoBehaviour {

	//the scene index of the level associated with this button
	public int m_LocationIndex;

	//These four variables will be measured as a fracton of the screen
	public float m_ButtonXCoordinate;
	public float m_ButtonYCoordinate;
	public float m_ButtonWidth;
	public float m_ButtonHeight; 

	private Rect m_ButtonRect;
	private float m_ScreenWidth;
	private float m_ScreenHeight;

	public RectTransform m_RectTransform;



	void Awake(){
		m_ScreenWidth = Screen.width;
		m_ScreenHeight = Screen.height;
		m_ButtonRect = new Rect(new Vector2 (m_ScreenWidth * m_ButtonXCoordinate,  m_ScreenHeight * m_ButtonYCoordinate), new Vector2 (m_ScreenWidth * m_ButtonWidth, m_ScreenHeight * m_ButtonHeight));
		m_RectTransform.anchoredPosition = m_ButtonRect.position;
		m_RectTransform.anchorMin = new Vector2 (0f, 0f);
		m_RectTransform.anchorMax = new Vector2 (m_ButtonWidth, m_ButtonHeight);
		m_RectTransform.sizeDelta = new Vector2 (m_ButtonRect.width, m_ButtonRect.height);
	}
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
