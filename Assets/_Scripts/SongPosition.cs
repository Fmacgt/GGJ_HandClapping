
using System.Collections;
using UnityEngine;


namespace GGJ18
{
	[CreateAssetMenu(menuName = "Scriptable Objects/Song Positon")]
	public sealed class SongPosition : ScriptableObject
	{

		//==============================================================================

		private float _leadingOffset = 0f;
		private float _pitch = 1f;
		private double _startDspTime = 0f;

		/////////////////////////////////////////////////////////////////////////////////////

		public float Value
		{
			get {
				return (float)(AudioSettings.dspTime - _startDspTime) * _pitch - _leadingOffset;
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////

		public void reset(AudioSource targetSong, float leadingOffset)
		{
			_leadingOffset = leadingOffset;
			_pitch = targetSong.pitch;
			_startDspTime = AudioSettings.dspTime;
		}
	}
}
