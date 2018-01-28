
using System.Collections;
using UnityEngine;


namespace GGJ18
{
	public sealed class SpriteAnimation : MonoBehaviour
	{
		public SpriteRenderer image;
		public Sprite[] imgList;
		public float frameRate = 10f;

		//==============================================================================

		private float _frameTime;
		private float _ticks = 0f;
		private int _imgIdx = 0;

		/////////////////////////////////////////////////////////////////////////////////////

		private void Awake()
		{
			_frameTime = 1f / frameRate;
			image.sprite = imgList[0];
		}

		private void Update()
		{
			_ticks += Time.deltaTime;
			if (_ticks >= _frameTime) {
				_ticks -= _frameTime;

				_imgIdx = (_imgIdx + 1) % imgList.Length;
				image.sprite = imgList[_imgIdx];
			}
		}
	}
}
