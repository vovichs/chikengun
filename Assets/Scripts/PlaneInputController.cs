using UnityEngine;

public class PlaneInputController : MonoBehaviour, IJoystickListener
{
	public JoystickController myJoystick;

	public Vector2 joystickDelta = Vector2.zero;

	public GameObject shootBtn;

	public PlaneController myHeli;

	private void OnEnable()
	{
		myHeli = (GameController.instance.OurPlayer.myCar as PlaneController);
		myJoystick.myInputListener = this;
		shootBtn.SetActive(myHeli.Shootable());
	}

	public void SetDelta(Vector2 delta)
	{
		joystickDelta = delta;
		myHeli.SetDelta(delta);
	}

	public void ShootBtnDown()
	{
		myHeli.StartShoot(0f);
	}

	public void ShootBtnUp()
	{
		myHeli.StopShooting();
	}

	public void OnEngineUpBtnDown()
	{
		myHeli.SetAccel(-1f);
	}

	public void OnEngineUpBtnUp()
	{
		myHeli.SetAccel(0f);
	}

	public void OnEngineDownBtnDown()
	{
		myHeli.SetAccel(1f);
	}

	public void OnEngineDownBtnUp()
	{
		myHeli.SetAccel(0f);
	}
}
