
using System.Collections;
using UnityEngine;


namespace GGJ18
{
	public sealed class Passenger : MonoBehaviour
	{
		public float lifeTime;
		public float startTime;

		public Vector3 fromPos;
		public Vector3 toPos;

		public float crotchet;


		public SpriteRenderer bodySprite;
		public SpawnAndMoveTest host;

		public Transform bodyTr;

		public bool onABlock = false;
		public GameObject blockObj;
		public float blockOffset = 1f;

		public SpriteAnimation[] animationSet;

		public Color[] colorList;

		//==============================================================================

		public enum Status
		{
			Standing,
			RaisingHand,
			Missed,
			Switching,
			Disabled,
		}

		public Status status = Status.Standing;

		/////////////////////////////////////////////////////////////////////////////////////

		private void Start()
		{
			bodySprite.color = colorList[Random.Range(0, colorList.Length)];
		}

		/////////////////////////////////////////////////////////////////////////////////////

		public void updatePosition(float currentTime)
		{
			float deltaTime = currentTime - startTime;
			float t = deltaTime / lifeTime;

			transform.position = Vector3.Lerp(fromPos, toPos, t);
		}

		public void updateStatus(float currentTime)
		{
			float deltaTime = currentTime - startTime;
			if (lifeTime >= deltaTime) {
				float remainingTime = lifeTime - deltaTime;
				if (status == Status.Standing && remainingTime <= crotchet * 0.5f) {
					startRaisingHand();
				}
			} else {
				if (status == Status.RaisingHand) {
					onMissed(currentTime);
				} else if (status == Status.Missed) {
					host.removePassenger(this);
					Destroy(gameObject);
				} else if (status == Status.Switching) {
					host.applyMatching(this);
				}
			}
		}

		public Player switchToPlayer(Player oldPlayer, float currentTime)
		{
			oldPlayer.turnOff();

			var otherPassenger = oldPlayer.gameObject.GetComponent<Passenger>();
			var offset = toPos - fromPos;
			offset.y = 0f;
			otherPassenger.fromPos = otherPassenger.transform.position;
			otherPassenger.toPos = otherPassenger.fromPos + offset;
			otherPassenger.lifeTime = lifeTime;
			otherPassenger.startTime = currentTime;
			otherPassenger.host = host;
			otherPassenger.status = Status.Missed;
			otherPassenger.bodySprite.flipX = true;
			host.addPassenger(otherPassenger);


			if (onABlock) {
				onABlock = false;
				blockObj.SetActive(false);

				otherPassenger.blockObj.SetActive(true);
			}


			host.removePassenger(this);
			bodySprite.flipX = false;

			animationSet[0].enabled = false;
			animationSet[1].enabled = true;
			animationSet[2].enabled = false;

			return gameObject.GetComponent<Player>();
		}

		public void setOnBlock()
		{
			onABlock = true;
			blockObj.SetActive(true);
			bodyTr.localPosition += Vector3.up * blockOffset;
		}

		public void setAnimation(int idx)
		{
			for (int i = 0; i < 3; i++) {
				animationSet[i].enabled = false;
			}

			animationSet[idx].enabled = true;
		}

		/////////////////////////////////////////////////////////////////////////////////////

		public void startRaisingHand()
		{
			status = Status.RaisingHand;

			animationSet[0].enabled = false;
			animationSet[1].enabled = false;
			animationSet[2].enabled = true;
		}

		public void onMissed(float currentTime)
		{
			status = Status.Missed;

			var offset = toPos - fromPos;
			fromPos = transform.position;
			toPos = fromPos + offset;
			startTime = currentTime;
		}

		public void onMatched()
		{
			status = Status.Switching;
		}
	}
}
