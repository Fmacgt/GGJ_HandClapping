
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


namespace GGJ18
{
	public sealed class Control : MonoBehaviour
	{
		public Transform cameraTr;

		public Transform centerTr;
		public int personCount = 12;
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

		private float _radius;
		private Person[] _persons;

		private Person _currentPerson;
		private Person _nextPerson;

		private bool _canHold = true;

		private int _counter = 0;
		private float _filledPercent;

		/////////////////////////////////////////////////////////////////////////////////////

		private void Start()
		{
			float circleLength = personCount * spawnDistanceStep;
			_radius = circleLength * 0.5f / Mathf.PI;
			Debug.LogFormat("{0} person every {1} => {2} length and {3} radius",
					personCount, spawnDistanceStep, circleLength, _radius);

			_persons = new Person[personCount];
			var d = _radius * Vector3.up;
			for (int i = 0; i < personCount; i++) {
				float angle = i * 360f / (float)personCount;
				var rotate = Quaternion.AngleAxis(angle, Vector3.forward);
				var offset = rotate * d;
				var spawnPt = centerTr.position - offset;

				var personObj = Instantiate(personPrefab);
				var personTr = personObj.transform;
				personTr.position = spawnPt;
				personTr.rotation = rotate;

				var person = personObj.GetComponent<Person>();
				person.leftHandRotate.control = this;
				person.leftTrigger.control = this;
				person.rightHandRotate.control = this;
				person.rightTrigger.control = this;

				_persons[i] = person;
			}

			for (int i = 0; i < personCount - 1; i++) {
				_persons[i].next = _persons[i + 1];
			}
			_persons[personCount-1].next = _persons[0];

			_currentPerson = _persons[0];
			_nextPerson = _persons[1];


			_persons[0].triggered = true;
			

			_filledPercent = initialFillingPercent;
			_currentPerson.setFillPercent(_filledPercent);


			focusAt(_currentPerson.transform, _currentPerson.transform, 0f);
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

			var up = Vector3.Slerp(trA.up, trB.up, 0.5f);

			if (duration >= 0.005f) {
				LeanTween.move(cameraTr.gameObject, cameraTr.position + offset, duration)
					.setEase(focusCurve)
					.setOnComplete(() => {
							_nextPerson.triggerSelf();
						});
				var oldUp = cameraTr.up;
				LeanTween.value(gameObject, 0f, 1f, duration)
					.setEase(focusCurve)
					.setOnUpdate((t) => {
							var newUp = Vector3.Slerp(oldUp, up, t);
							cameraTr.rotation = Quaternion.LookRotation(cameraTr.forward, newUp);
						});
			} else {
				cameraTr.Translate(offset, Space.World);
				cameraTr.rotation = Quaternion.LookRotation(cameraTr.forward, up);
			}
		}

		public void startGame()
		{
			state = GameStates.Started;

			_counter = 0;

			focusAt(_currentPerson.transform, _nextPerson.transform, focusTime);
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


			// TODO: update current and next 'Person'
			_currentPerson = _nextPerson;


			// TODO: update filled percent
			_filledPercent += (matchRatio - 0.5f) * 2f * _currentPerson.data.fillIncrement;
			_currentPerson.setFillPercent(_filledPercent);

			_nextPerson = _currentPerson.next;
			_nextPerson.triggered = false;


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
