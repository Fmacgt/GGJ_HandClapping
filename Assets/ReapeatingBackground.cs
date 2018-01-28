using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReapeatingBackground : MonoBehaviour {
	private BoxCollider2D m_BackGroundCollider;
	public float m_BackgroundSize;

	// Use this for initialization
	void Start () {
		m_BackGroundCollider = GetComponent<BoxCollider2D> ();
		m_BackgroundSize = m_BackGroundCollider.size.x * transform.localScale.x;
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.position.x < -m_BackgroundSize)
			RepeatBackround ();
		
	}

	void RepeatBackround (){
		Vector2 BGoffset = new Vector2 (m_BackgroundSize * 2F,0);
		transform.position = (Vector2)transform.position + BGoffset;
	
	}
}
