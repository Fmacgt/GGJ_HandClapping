
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GGJ18
{
	public sealed class SpawnAndMoveTest : MonoBehaviour
	{
		public Transform spawnTr;
		public GameObject passengerPrefab;

		public AudioSource audioPlayer;

		public SongPosition songPosition;
		public LevelData levelData;

		public float moveSpeed = 5f;

		//==============================================================================

		private List<Passenger> _passengers;

		private float _lastBeatTime = 0f;
		private int _beatCounter = 0;

		/////////////////////////////////////////////////////////////////////////////////////

		private void Awake()
		{
			_passengers = new List<Passenger>();
		}

		private void Start()
		{
			songPosition.reset(audioPlayer, levelData.leadingOffset);
			_lastBeatTime = 0f;
			_beatCounter = 0;

			_passengers.Clear();
		}

		private void Update()
		{
			float songPt = songPosition.Value;

			for (int i = 0; i < _passengers.Count; i++) {
				var passenger = _passengers[i];
				var tr = passenger.transform;
				tr.Translate(Vector3.left * moveSpeed * Time.deltaTime, Space.World);
			}

			if (songPt >= _lastBeatTime + levelData.crotchet) {
				_lastBeatTime += levelData.crotchet;

				_beatCounter++;

				// TODO: check and spawn a new passenger
				if (_beatCounter % levelData.beatPerSpawn == 0) {
					spawnPassenger();
				}
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////

		public void spawnPassenger()
		{
			var obj = Instantiate(passengerPrefab);
			obj.transform.position = spawnTr.position;

			_passengers.Add(obj.GetComponent<Passenger>());
		}
	}
}
