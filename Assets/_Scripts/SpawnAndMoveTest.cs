
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

		public Transform playerTr;
		public Player initPlayer;
		public float beatsToReachPlayer = 6;

		public float moveSpeed = 5f;

		//==============================================================================

		private List<Passenger> _passengers;
		private Player _player;

		private float _lastBeatTime = 0f;
		private int _beatCounter = 0;

		/////////////////////////////////////////////////////////////////////////////////////

		private void Awake()
		{
			_passengers = new List<Passenger>();
			_player = initPlayer;
			_player.host = this;
		}

		private void Start()
		{
			songPosition.reset(audioPlayer, levelData.leadingOffset);
			_lastBeatTime = 0f;
			_beatCounter = 0;

			_passengers.Clear();
			_player.crotchet = levelData.crotchet;

			var offset = playerTr.position - spawnTr.position;
			offset.y = 0f;
			moveSpeed = offset.magnitude / (levelData.crotchet * beatsToReachPlayer);
		}

		private void Update()
		{
			float songPt = songPosition.Value;

			for (int i = 0; i < _passengers.Count; i++) {
				var passenger = _passengers[i];
				passenger.updatePosition(songPt);
				passenger.updateStatus(songPt);
			}

			if (songPt >= _lastBeatTime + levelData.crotchet) {
				_lastBeatTime += levelData.crotchet;

				_beatCounter++;

				// TODO: check and spawn a new passenger
				if (_beatCounter % levelData.beatPerSpawn == 0) {
					spawnPassenger(_lastBeatTime, songPt);
				}
			}


			if (!_player.inCooldown(songPt)) {
				if (Input.GetKeyDown(KeyCode.Space)) {
					// TODO: try to high-five
					_player.startHighFive(_lastBeatTime, songPt);
				} else if (Input.GetKeyDown(KeyCode.UpArrow)) {
					// TODO: try to jump up and high-five
					_player.startJumpUp(_lastBeatTime, songPt);
				} else if (Input.GetKeyDown(KeyCode.DownArrow)) {
					// TODO: try to dash down and high-five
					_player.startDiveDown(_lastBeatTime, songPt);
				}
			} else {
				Debug.Log("In cooldown");
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////

		public void spawnPassenger(float startTime, float currentTime)
		{
			var obj = Instantiate(passengerPrefab);
			obj.transform.position = spawnTr.position;

			var passenger = obj.GetComponent<Passenger>();
			passenger.lifeTime = beatsToReachPlayer * levelData.crotchet;
			passenger.startTime = startTime;
			passenger.fromPos = spawnTr.position;
			passenger.toPos = playerTr.position;
			passenger.crotchet = levelData.crotchet;
			passenger.host = this;

			passenger.updatePosition(currentTime);

			_passengers.Add(passenger);
		}

		public void removePassenger(Passenger target)
		{
			_passengers.Remove(target);
		}

		public void addPassenger(Passenger target)
		{
			_passengers.Add(target);
		}

		public void checkHighFive()
		{
			for (int i = 0; i < _passengers.Count; i++) {
				var passenger = _passengers[i];
				if (passenger.status == Passenger.Status.RaisingHand) {
					// TODO: compute matching rate(?)
					passenger.onMatched();

					// TODO: switch passenger and player
					_player = passenger.switchToPlayer(_player, songPosition.Value);
					_player.transform.position = playerTr.position;
					_player.host = this;
					_player.crotchet = levelData.crotchet;
				}
			}
		}
	}
}
