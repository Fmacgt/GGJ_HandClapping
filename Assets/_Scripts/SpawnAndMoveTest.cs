
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace GGJ18
{
	public sealed class SpawnAndMoveTest : MonoBehaviour
	{
		public Image hpBar;
		public GameObject gameOverGroup;
		public GameObject statusGroup;

		public Transform spawnTr;
		public GameObject passengerPrefab;

		public AudioSource audioPlayer;
		public AudioSource clapEffect;

		public SongPosition songPosition;
		public LevelData levelData;

		public Transform playerTr;
		public Player initPlayer;
		public float beatsToReachPlayer = 6;

		public float moveSpeed = 5f;

		public float hp = 100f;
		public float drainRate = 10f;
        public float gameTimer = 0f;
        public Text gameOverTimerText;

		//==============================================================================

		public enum Status
		{
			Idle,
			Running,
			GameOver
		}

		public Status status = Status.Idle;

		//==============================================================================

		private List<Passenger> _passengers;
		private Player _player;

		private float _lastBeatTime = 0f;
		private int _beatCounter = 0;
		private int _nextSpawnIdx = 0;

		public const float VERTICAL_LIMIT = 0.25f;

		/////////////////////////////////////////////////////////////////////////////////////

		private void Awake()
		{
			_passengers = new List<Passenger>();
			_player = initPlayer;
			_player.host = this;
		}

		private void Start()
		{
			var passenger = _player.gameObject.GetComponent<Passenger>();
			passenger.animationSet[0].enabled = false;
			passenger.animationSet[1].enabled = true;
			passenger.animationSet[2].enabled = false;

			var offset = playerTr.position - spawnTr.position;
			offset.y = 0f;
			moveSpeed = offset.magnitude / (levelData.crotchet * beatsToReachPlayer);
		}

		private void Update()
		{
			if (status != Status.Running) {
				return;
			}

            gameTimer += Time.deltaTime;

			float songPt = songPosition.Value;

			for (int i = 0; i < _passengers.Count; i++) {
				var passenger = _passengers[i];
				passenger.updatePosition(songPt);
				passenger.updateStatus(songPt);
			}

			if (songPt >= _lastBeatTime + levelData.crotchet) {
				_lastBeatTime += levelData.crotchet;

				_beatCounter++;
				if (_nextSpawnIdx < levelData.timeTable.Length &&
						_beatCounter >= levelData.timeTable[_nextSpawnIdx].beat) {
					spawnPassenger(_lastBeatTime, songPt, levelData.timeTable[_nextSpawnIdx].onBlock);

					_nextSpawnIdx++;
					if (_nextSpawnIdx >= levelData.timeTable.Length && levelData.looping) {
						_beatCounter = 0;
						_nextSpawnIdx = 0;
					}
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


			hp -= drainRate * Time.deltaTime;
			hpBar.fillAmount = Mathf.Clamp(hp, 0, 100f) * 0.01f;
			if (hp <= 0f) {
				hp = 0f;
				status = Status.GameOver;

				gameOverGroup.SetActive(true);
                gameOverTimerText.text = "You spreaded happiness for "+ gameTimer.ToString()+" seconds!";
				statusGroup.SetActive(false);
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////

		public void startGame()
		{
			songPosition.reset(audioPlayer, levelData.leadingOffset);
			audioPlayer.Play();
			_lastBeatTime = 0f;
			_beatCounter = 0;
			_nextSpawnIdx = 0;

			_passengers.Clear();
			_player.crotchet = levelData.crotchet;


			statusGroup.SetActive(true);
			status = Status.Running;
		}

		public void spawnPassenger(float startTime, float currentTime, bool isOnBlock)
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
			passenger.bodySprite.flipX = true;

			passenger.updatePosition(currentTime);
			if (isOnBlock) {
				passenger.setOnBlock();
			}

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
			bool matched = false;
			for (int i = 0; i < _passengers.Count; i++) {
				var passenger = _passengers[i];
				if (passenger.status == Passenger.Status.RaisingHand) {
					var offset = passenger.bodyTr.position - _player.bodyTr.position;
					if (Mathf.Abs(offset.y) <= VERTICAL_LIMIT) {
						// TODO: compute matching rate(?)
						passenger.onMatched();
						matched = true;

						/**
						// TODO: switch passenger and player
						var oldStatus = _player.status;
						_player = passenger.switchToPlayer(_player, songPosition.Value);
						_player.transform.position = playerTr.position;
						_player.host = this;
						_player.crotchet = levelData.crotchet;
						if (oldStatus == Player.Status.Falling || oldStatus == Player.Status.Jumping) {
							_player.startLanding(songPosition.Value);
						}
						**/
					}
				}
			}
			if (!matched) {
				_player.gameObject.GetComponent<Passenger>().setAnimation(1);
			}
		}

		public void applyMatching(Passenger passenger)
		{
			// TODO: switch passenger and player
			var oldStatus = _player.status;
			_player = passenger.switchToPlayer(_player, songPosition.Value);
			_player.transform.position = playerTr.position;
			_player.host = this;
			_player.crotchet = levelData.crotchet;
			if (oldStatus == Player.Status.Falling || oldStatus == Player.Status.Jumping) {
				_player.startLanding(songPosition.Value);
			}

			clapEffect.Play();

			hp = Mathf.Clamp(hp + _player.data.healRate, 0f, 100f);
			hpBar.fillAmount = Mathf.Clamp(hp, 0, 100f) * 0.01f;
		}
	}
}
