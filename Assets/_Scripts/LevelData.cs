
using System.Collections;
using UnityEngine;


namespace GGJ18
{
	[CreateAssetMenu(menuName="Scriptable Objects/Level Data")]
	public sealed class LevelData : ScriptableObject
	{
		public AudioClip songClip;

		public float leadingOffset = 0f;
		public float pitch = 1f;
		public float bpm = 100f;

		public float crotchet = 0.6f;

		// TEMP
		public float beatPerSpawn = 2f;

		//==============================================================================

		[System.Serializable]
		public struct SpawnTime
		{
			public int beat;
			public PersonData typeData;
			public bool onBlock;
		}

		public SpawnTime[] timeTable;

		/////////////////////////////////////////////////////////////////////////////////////

		private void OnEnable()
		{
			crotchet = 60f / bpm;
		}
	}
}
