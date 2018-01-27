
using System.Collections;
using UnityEngine;


namespace GGJ18
{
	public sealed class Person : MonoBehaviour
	{
		public RotateHand leftHandRotate;
		public RotateHand rightHandRotate;

		public float leftFromAngle = 150f;
		public float leftToAngle = 30f;
		public float leftRotateTime = 0.5f;

		public bool triggered = false;

		/////////////////////////////////////////////////////////////////////////////////////

		public void triggerSelf()
		{
			if (!triggered) {
				triggered = true;

				leftHandRotate.play(leftFromAngle, leftToAngle, leftRotateTime);
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////

		private IEnumerator Start()
		{
			yield return new WaitForSeconds(1f);

			triggerSelf();
		}
	}
}
