
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
			host.addPassenger(otherPassenger);


			host.removePassenger(this);
			bodySprite.color = Color.gray;
			return gameObject.GetComponent<Player>();
		}

		/////////////////////////////////////////////////////////////////////////////////////

		public void startRaisingHand()
		{
			status = Status.RaisingHand;

			bodySprite.color = Color.blue;
		}

		public void onMissed(float currentTime)
		{
			status = Status.Missed;

			bodySprite.color = Color.red;

			var offset = toPos - fromPos;
			fromPos = transform.position;
			toPos = fromPos + offset;
			startTime = currentTime;
		}

		public void onMatched()
		{
			status = Status.Switching;

			bodySprite.color = Color.green;
		}
	}
}
