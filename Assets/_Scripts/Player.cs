
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

		/////////////////////////////////////////////////////////////////////////////////////

		public bool inCooldown(float currentTime)
		{
			return currentTime - actionStartTime < crotchet * 0.5f;
		}

		public void startHighFive(float startTime, float currentTime)
		{
			actionStartTime = startTime;

			status = Status.RaisingHand;

			LeanTween.delayedCall(gameObject, crotchet * 0.25f, () => {
					host.checkHighFive();
				});
		}

		public void startJumpUp(float startTime, float currentTime)
		{
			actionStartTime = startTime;

			status = Status.Jumping;
		}

		public void startDiveDown(float startTime, float currentTime)
		{
			actionStartTime = startTime;

			status = Status.DashingDown;
		}

		public void onMissed()
		{
			if (status == Status.Jumping) {
				status = Status.Falling;
			} else {
				status = Status.Running;
			}
		}


		public void turnOff()
		{
			status = Status.Standing;
		}
	}
}
