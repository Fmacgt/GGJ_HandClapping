using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroller : MonoBehaviour {
	private Rigidbody2D rigidBody;
	private float m_Speed = -5f;

	[SerializeField] private bool m_StopScrolling = false;
	// Use this for initialization
	void Start () {
		rigidBody = GetComponent<Rigidbody2D> ();
		rigidBody.velocity = new Vector2 (m_Speed, 0);

			
		
	}
	
	// Update is called once per frame
	void Update () {
		if (m_StopScrolling)
			rigidBody.velocity = Vector2.zero;
		else
			rigidBody.velocity = new Vector2 (m_Speed, 0);
		
	}
}
