
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


namespace GGJ18
{
	public sealed class Control : MonoBehaviour
	{
		public Transform cameraTr;

		public Person firstPerson;
		public Person secondPerson;

		public float focusTime = 0.5f;
		public AnimationCurve focusCurve;

		public float spawnDistanceStep = 2.8f;
		public Person personPrefab;


		public GameObject gameOverGroup;
		public GameObject startGameGroup;

		public Text counterLabel;
		public Text totalCounterLabel;

		public float initialFillingPercent = 0.5f;

		//==============================================================================

		public enum GameStates
		{
			Idle,
			Started,
			GameOver,
		}

		public GameStates state = GameStates.Idle;

		//==============================================================================

		private Person _currentPerson;
		private Person _nextPerson;

		private bool _canHold = true;
		private Vector3 _personSpawnPt;

		private int _counter = 0;
		private float _filledPercent;

		/////////////////////////////////////////////////////////////////////////////////////

		private void Start()
		{
			_currentPerson = firstPerson;
			_currentPerson.leftHandRotate.control = this;
			_currentPerson.leftTrigger.control = this;
			_currentPerson.rightHandRotate.control = this;
			_currentPerson.rightTrigger.control = this;

			_nextPerson = secondPerson;
			_nextPerson.leftHandRotate.control = this;
			_nextPerson.leftTrigger.control = this;
			_nextPerson.rightHandRotate.control = this;
			_nextPerson.rightTrigger.control = this;
			
			_personSpawnPt = _nextPerson.transform.position;


			_filledPercent = initialFillingPercent;
			_currentPerson.setFillPercent(_filledPercent);


			startGameGroup.SetActive(true);
		}

		private void Update()
		{
			if (state == GameStates.Started & _canHold) {
				if (Input.GetMouseButtonDown(0)) {
					_currentPerson.rightHandRotate.play();
				}
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////

		public void focusAt(Transform trA, Transform trB, float duration)
		{
			var centerPt = Vector3.Lerp(trA.position, trB.position, 0.5f);
			var offset = centerPt - cameraTr.position;
			offset.z = 0f;

			if (duration >= 0.005f) {
				LeanTween.move(cameraTr.gameObject, cameraTr.position + offset, duration)
					.setEase(focusCurve)
					.setOnComplete(() => {
							_nextPerson.triggerSelf();
						});
			} else {
				cameraTr.Translate(offset, Space.World);
			}
		}

		public void startGame()
		{
			state = GameStates.Started;

			_counter = 0;

			focusAt(firstPerson.transform, secondPerson.transform, focusTime);
			startGameGroup.SetActive(false);
		}

		/////////////////////////////////////////////////////////////////////////////////////

		public void onHandHeld(float matchRatio)
		{
			// TODO: increase score?
			_counter++;
			counterLabel.text = _counter.ToString();
			var counterTr = counterLabel.GetComponent<RectTransform>();
			counterTr.localScale = Vector3.one * 0.5f;
			LeanTween.scale(counterTr, Vector3.one, 0.2f).setEase(LeanTweenType.easeOutBack);


			_currentPerson.showHoldHandEffect(false);
			_nextPerson.showHoldHandEffect(true);


			// TODO: spawn new 'Person'
			_personSpawnPt += _nextPerson.transform.right * spawnDistanceStep;

			var personObj = Instantiate(personPrefab);
			var personTr = personObj.transform;
			personTr.position = _personSpawnPt;
			float angle = Vector3.Angle(_nextPerson.transform.right, Vector3.right);
			personTr.rotation = Quaternion.AngleAxis(angle, Vector3.forward);


			// TODO: update current and next 'Person'
			_currentPerson = _nextPerson;


			// TODO: update filled percent
			_filledPercent += (matchRatio - 0.5f) * 2f * _currentPerson.data.fillIncrement;
			_currentPerson.setFillPercent(_filledPercent);

			_nextPerson = personObj.GetComponent<Person>();
			_nextPerson.leftHandRotate.control = this;
			_nextPerson.leftTrigger.control = this;
			_nextPerson.rightHandRotate.control = this;
			_nextPerson.rightTrigger.control = this;


			// TODO: re-focus camera
			focusAt(_currentPerson.transform, _nextPerson.transform, focusTime);
		}

		public void onGameOver()
		{
			state = GameStates.GameOver;


			_nextPerson.showMissedEffect(true);


			gameOverGroup.SetActive(true);
			totalCounterLabel.text = string.Format("Hold hands for {0} times...", _counter);
		}
	}
}
