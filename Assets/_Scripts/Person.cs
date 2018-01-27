
using System.Collections;
using UnityEngine;


namespace GGJ18
{
	public sealed class Person : MonoBehaviour
	{
		public RotateHand leftHandRotate;
		public HandTrigger leftTrigger;
		public SpriteRenderer leftHandRenderer;

		public RotateHand rightHandRotate;
		public HandTrigger rightTrigger;
		public SpriteRenderer rightHandRenderer;

		public float leftFromAngle = 150f;
		public float leftToAngle = 30f;
		public float leftRotateTime = 0.5f;

		public bool triggered = false;

		public Color successColor = Color.green;
		public Color failureColor = Color.red;

		public Transform fillerTr;

		/////////////////////////////////////////////////////////////////////////////////////

		public void triggerSelf()
		{
			if (!triggered) {
				triggered = true;

				leftHandRotate.play(leftFromAngle, leftToAngle, leftRotateTime);
			}
		}

		public void showHoldHandEffect(bool showLeft)
		{
			if (showLeft) {
				leftHandRenderer.color = successColor;
			} else {
				rightHandRenderer.color = successColor;
			}
		}

		public void showMissedEffect(bool showLeft)
		{
			if (showLeft) {
				leftHandRenderer.color = failureColor;
			} else {
				rightHandRenderer.color = failureColor;
			}
		}

		public void setFillPercent(float percent)
		{
			percent = Mathf.Clamp(percent, 0f, 1f);

			fillerTr.localScale = new Vector3(1f, percent, 1f);
			fillerTr.localPosition = Vector3.down * (1f - percent) * 0.5f;
		}

		/////////////////////////////////////////////////////////////////////////////////////

		/**
		private IEnumerator Start()
		{
			yield return new WaitForSeconds(1f);

			triggerSelf();
		}
		**/
	}
}
