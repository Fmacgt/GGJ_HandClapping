
using System.Collections;
using UnityEngine;
using UnityEditor;


namespace GGJ18
{
	[CustomEditor(typeof(LevelData))]
	public sealed class LevelDataEditor : Editor
	{
		private bool _isPlaying = false;

		private double _startDspTime = 0f;
		private float _bpm;
		private float _crotchet = 0f;

		private float _lastBeatTime = 0f;
		private int _beatCounter = 0;

		/////////////////////////////////////////////////////////////////////////////////////

		private void OnEnable()
		{
			var bpmProp = serializedObject.FindProperty("bpm");
			_bpm = bpmProp.floatValue;
			_crotchet = 60f / _bpm;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			if (_isPlaying) {
				float time = (float)(AudioSettings.dspTime - _startDspTime);
				EditorGUILayout.LabelField("Time", time.ToString());

				if (time >= _lastBeatTime + _crotchet) {
					_lastBeatTime += _crotchet;
					_beatCounter++;
				}
				EditorGUILayout.LabelField("Beat", _beatCounter.ToString());

				if (GUILayout.Button("Stop")) {
					PublicAudioUtil.StopAllClips();
					_isPlaying = false;
				}
			} else {
				if (GUILayout.Button("Play")) {
					PublicAudioUtil.PlayClip(_getAudioClip());
					_startDspTime = AudioSettings.dspTime;
					_lastBeatTime = 0f;
					_beatCounter = 0;

					_isPlaying = true;
				}
			}

			DrawDefaultInspector();
		}

		/////////////////////////////////////////////////////////////////////////////////////

		private AudioClip _getAudioClip()
		{
			var clipProp = serializedObject.FindProperty("songClip");
			if (clipProp != null) {
				return clipProp.objectReferenceValue as AudioClip;
			}

			return null;
		}
	}
}
