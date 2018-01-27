
using System.Collections;
using UnityEngine;


namespace GGJ18
{
	public sealed class HandTrigger : MonoBehaviour
	{
		public bool isLeftHand;
		public RotateHand rotateHand;

		/////////////////////////////////////////////////////////////////////////////////////

		private void OnTriggerEnter2D(Collider2D other)
		{
			var otherRb = other.attachedRigidbody;
			if (otherRb != null) {
				var otherTrigger = otherRb.gameObject.GetComponent<HandTrigger>();
				Debug.LogFormat("Hold hand trigger for {0} hand", isLeftHand ? "Left" : "Right");

				rotateHand.stop();
			}
		}
	}
}
