
using System.Collections;
using UnityEngine;


namespace GGJ18
{
	public sealed class Player : MonoBehaviour
	{
		public PersonData data;
		public float crotchet;

		public float actionStartTime;
		public SpawnAndMoveTest host;

		public Transform bodyTr;

		public float highFiveTimeRatio = 0.25f;
		public float jumpTimeRatio = 0.5f;
		public float fallTimeRatio = 0.5f;
		public float diveTimeRatio = 0.5f;

		public AnimationCurve jumpCurve;
		public AnimationCurve landCurve;
		public AnimationCurve dashCurve;

		//==============================================================================

		public enum Status
		{
			Running,
			RaisingHand,
			Jumping,
			Falling,
			DashingDown,
			Standing
		}

		public Status status = Status.Running;

		//==============================================================================

		private Vector3 _landPos;
		private Vector3 _jumpPos;

		/////////////////////////////////////////////////////////////////////////////////////

		private void Awake()
		{
			_landPos = bodyTr.localPosition;
			_jumpPos = _landPos + Vector3.up;
		}

		/////////////////////////////////////////////////////////////////////////////////////

		public bool inCooldown(float currentTime)
		{
			return currentTime - actionStartTime < crotchet * 0.5f;
		}

		public void startHighFive(float startTime, float currentTime)
		{
			actionStartTime = currentTime;

			status = Status.RaisingHand;

			LeanTween.delayedCall(gameObject, crotchet * highFiveTimeRatio, () => {
					host.checkHighFive();
				});
		}

		public void startJumpUp(float startTime, float currentTime)
		{
			actionStartTime = currentTime;

			status = Status.Jumping;

			LeanTween.moveLocal(bodyTr.gameObject, _jumpPos, crotchet * jumpTimeRatio)
				.setEase(jumpCurve)
				.setOnComplete(_onJumpEnd);
		}

		public void startDiveDown(float startTime, float currentTime)
		{
			actionStartTime = currentTime;

			status = Status.DashingDown;

			LeanTween.moveLocal(bodyTr.gameObject, _landPos, crotchet * diveTimeRatio)
				.setEase(jumpCurve)
				.setOnComplete(_onDashLanded);
		}

		public void onMissed()
		{
			if (status == Status.Jumping) {
				status = Status.Falling;
			} else {
				status = Status.Running;
			}
		}

		public void startLanding(float currentTime)
		{
			actionStartTime = currentTime;
			status = Status.Falling;

			LeanTween.moveLocal(bodyTr.gameObject, _landPos, crotchet * fallTimeRatio)
				.setEase(landCurve)
				.setOnComplete(_onLanded);
		}


		public void turnOff()
		{
			status = Status.Standing;
		}

		/////////////////////////////////////////////////////////////////////////////////////

		private void _onJumpEnd()
		{
			host.checkHighFive();

			startLanding(actionStartTime);// + crotchet * jumpTimeRatio);
		}

		private void _onLanded()
		{
			status = Status.Running;
		}

		private void _onDashLanded()
		{
			status = Status.Running;

			host.checkHighFive();
		}
	}
}
