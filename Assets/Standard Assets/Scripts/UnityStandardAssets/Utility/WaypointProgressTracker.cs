using UnityEngine;

namespace UnityStandardAssets.Utility
{
	public class WaypointProgressTracker : MonoBehaviour
	{
		public enum ProgressStyle
		{
			SmoothAlongRoute,
			PointToPoint
		}

		[SerializeField]
		private WaypointCircuit circuit;

		[SerializeField]
		private float lookAheadForTargetOffset = 5f;

		[SerializeField]
		private float lookAheadForTargetFactor = 0.1f;

		[SerializeField]
		private float lookAheadForSpeedOffset = 10f;

		[SerializeField]
		private float lookAheadForSpeedFactor = 0.2f;

		[SerializeField]
		private ProgressStyle progressStyle;

		[SerializeField]
		private float pointToPointThreshold = 4f;

		public Transform target;

		private float progressDistance;

		private int progressNum;

		private Vector3 lastPosition;

		private float speed;

		public WaypointCircuit.RoutePoint targetPoint
		{
			get;
			private set;
		}

		public WaypointCircuit.RoutePoint speedPoint
		{
			get;
			private set;
		}

		public WaypointCircuit.RoutePoint progressPoint
		{
			get;
			private set;
		}

		private void Start()
		{
			if (target == null)
			{
				target = new GameObject(base.name + " Waypoint Target").transform;
			}
			Reset();
		}

		public void Reset()
		{
			progressDistance = 0f;
			progressNum = 0;
			if (progressStyle == ProgressStyle.PointToPoint)
			{
				target.position = circuit.Waypoints[progressNum].position;
				target.rotation = circuit.Waypoints[progressNum].rotation;
			}
		}

		private void Update()
		{
			if (progressStyle == ProgressStyle.SmoothAlongRoute)
			{
				if (Time.deltaTime > 0f)
				{
					speed = Mathf.Lerp(speed, (lastPosition - base.transform.position).magnitude / Time.deltaTime, Time.deltaTime);
				}
				Transform transform = target;
				WaypointCircuit.RoutePoint routePoint = circuit.GetRoutePoint(progressDistance + lookAheadForTargetOffset + lookAheadForTargetFactor * speed);
				transform.position = routePoint.position;
				Transform transform2 = target;
				WaypointCircuit.RoutePoint routePoint2 = circuit.GetRoutePoint(progressDistance + lookAheadForSpeedOffset + lookAheadForSpeedFactor * speed);
				transform2.rotation = Quaternion.LookRotation(routePoint2.direction);
				this.progressPoint = circuit.GetRoutePoint(progressDistance);
				WaypointCircuit.RoutePoint progressPoint = this.progressPoint;
				Vector3 vector = progressPoint.position - base.transform.position;
				Vector3 lhs = vector;
				WaypointCircuit.RoutePoint progressPoint2 = this.progressPoint;
				if (Vector3.Dot(lhs, progressPoint2.direction) < 0f)
				{
					progressDistance += vector.magnitude * 0.5f;
				}
				lastPosition = base.transform.position;
			}
			else
			{
				if ((target.position - base.transform.position).magnitude < pointToPointThreshold)
				{
					progressNum = (progressNum + 1) % circuit.Waypoints.Length;
				}
				target.position = circuit.Waypoints[progressNum].position;
				target.rotation = circuit.Waypoints[progressNum].rotation;
				this.progressPoint = circuit.GetRoutePoint(progressDistance);
				WaypointCircuit.RoutePoint progressPoint3 = this.progressPoint;
				Vector3 vector2 = progressPoint3.position - base.transform.position;
				Vector3 lhs2 = vector2;
				WaypointCircuit.RoutePoint progressPoint4 = this.progressPoint;
				if (Vector3.Dot(lhs2, progressPoint4.direction) < 0f)
				{
					progressDistance += vector2.magnitude;
				}
				lastPosition = base.transform.position;
			}
		}

		private void OnDrawGizmos()
		{
			if (Application.isPlaying)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawLine(base.transform.position, target.position);
				Gizmos.DrawWireSphere(circuit.GetRoutePosition(progressDistance), 1f);
				Gizmos.color = Color.yellow;
				Gizmos.DrawLine(target.position, target.position + target.forward);
			}
		}
	}
}
