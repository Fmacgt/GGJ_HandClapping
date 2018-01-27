
using System.Collections;
using UnityEngine;


namespace GGJ18
{
	public sealed class HandTrigger : MonoBehaviour
	{
		public bool isLeftHand;
		public RotateHand rotateHand;
		public Control control;

		/////////////////////////////////////////////////////////////////////////////////////

		private void OnTriggerEnter2D(Collider2D other)
		{
			var otherRb = other.attachedRigidbody;
			rotateHand.stop();

			if (!isLeftHand) {
				Debug.LogFormat("calling {0}.onHandHeld()", control);
				control.onHandHeld();
			}
		}
	}
}
