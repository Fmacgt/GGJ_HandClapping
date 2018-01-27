
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
			rotateHand.stop();

			if (!isLeftHand) {
				var otherRb = other.attachedRigidbody;
				if (otherRb != null) {
					var otherHand = otherRb.gameObject.GetComponent<HandTrigger>();
					float matchRatio = Mathf.Abs(Vector3.Dot(transform.up, otherHand.transform.up));
					Debug.LogFormat("{0}% matching", matchRatio * 100f);
					control.onHandHeld(matchRatio);
				}
			}
		}
	}
}
