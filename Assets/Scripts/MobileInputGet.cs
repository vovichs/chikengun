using UnityEngine;

[RequireComponent(typeof(VehicleParent))]
[DisallowMultipleComponent]
[AddComponentMenu("RVP/C#/Input/Mobile Input Getter", 2)]
public class MobileInputGet : MonoBehaviour
{
	private VehicleParent vp;

	private MobileInput setter;

	public float steerFactor = 1f;

	public float flipFactor = 1f;

	public bool useAccelerometer = true;

	[Tooltip("Multiplier for input addition based on rate of change of input")]
	public float deltaFactor = 10f;

	private Vector3 accelerationPrev;

	private Vector3 accelerationDelta;

	private void Start()
	{
		vp = GetComponent<VehicleParent>();
		setter = UnityEngine.Object.FindObjectOfType<MobileInput>();
	}

	private void FixedUpdate()
	{
		if ((bool)setter)
		{
			accelerationDelta = Input.acceleration - accelerationPrev;
			accelerationPrev = Input.acceleration;
			vp.SetAccel(setter.accel);
			vp.SetBrake(setter.brake);
			vp.SetEbrake(setter.ebrake);
			vp.SetBoost(setter.boost);
			if (useAccelerometer)
			{
				VehicleParent vehicleParent = vp;
				Vector3 acceleration = Input.acceleration;
				vehicleParent.SetSteer((acceleration.x + accelerationDelta.x * deltaFactor) * steerFactor);
				VehicleParent vehicleParent2 = vp;
				Vector3 acceleration2 = Input.acceleration;
				vehicleParent2.SetYaw(acceleration2.x * flipFactor);
				VehicleParent vehicleParent3 = vp;
				Vector3 acceleration3 = Input.acceleration;
				vehicleParent3.SetPitch((0f - acceleration3.z) * flipFactor);
			}
			else
			{
				vp.SetSteer(setter.steer);
			}
		}
	}
}
