
using System.Collections;
using UnityEngine;


namespace GGJ18
{
	public sealed class RotateHand : MonoBehaviour
	{
		public Transform axisTr;
		public AnimationCurve rotateCurve;

		public float fromAngle;
		public float toAngle;
		public float rotateTime;

		//==============================================================================

		private Quaternion _fromRotate;
		private Quaternion _toRotate;

		/////////////////////////////////////////////////////////////////////////////////////

		public void play()
		{
			play(fromAngle, toAngle, rotateTime);
		}

		public void play(float fromAngle, float toAngle, float duration)
		{
			_fromRotate = Quaternion.AngleAxis(fromAngle, axisTr.forward);
			_toRotate = Quaternion.AngleAxis(toAngle, axisTr.forward);
			LeanTween.value(0f, 1f, duration).setEase(rotateCurve)
				.setOnUpdate(_updateRotate);
		}

		public void stop()
		{
			LeanTween.cancelAll();
		}

		/////////////////////////////////////////////////////////////////////////////////////

		private void _updateRotate(float t)
		{
			axisTr.rotation = Quaternion.Lerp(_fromRotate, _toRotate, t);
		}
	}
}
