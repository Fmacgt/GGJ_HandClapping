
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

		public float crotchet;

		// TEMP
		public float beatPerSpawn = 2f;

		/////////////////////////////////////////////////////////////////////////////////////

		private void OnEnable()
		{
			crotchet = 60f / bpm;
		}
	}
}
