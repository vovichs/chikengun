using UnityEngine;
using UnityEngine.UI;

public class GameFinishedGUI : MonoBehaviour
{
	private enum GUIState
	{
		undef,
		otherCarsDemonstr,
		countDown,
		playing,
		won,
		lost,
		interrupted
	}

	public RawImage steeringWheel;

	public RawImage steeringRect;

	private int W = Screen.width;

	private int H = Screen.height;

	private Vector2 steeringWheelSize;

	private Vector3 steeringWheelPos;

	private Vector3 SteeringWheelRot;

	private Vector2 steeringWheelPointerPos;

	private bool steeringWheelActive;

	private float steeringAngle;

	private Vector3 steerRectPos;

	private Vector2 steerRectSize;

	public GameController gameController;

	private void Start()
	{
		gameController = GameObject.Find("GameController").GetComponent<GameController>();
		initVars();
		steeringWheel.transform.position = steeringWheelPos;
		steeringWheel.transform.rotation = Quaternion.Euler(SteeringWheelRot);
		steeringWheel.rectTransform.sizeDelta = steeringWheelSize;
		steeringWheelPointerPos = Vector2.zero;
		steeringWheelActive = false;
		steeringRect.rectTransform.position = steerRectPos;
		steeringRect.rectTransform.sizeDelta = steerRectSize;
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Vector3 mousePosition = UnityEngine.Input.mousePosition;
			if (mousePosition.x > steerRectPos.x - steerRectSize.x / 2f)
			{
				Vector3 mousePosition2 = UnityEngine.Input.mousePosition;
				if (mousePosition2.x < steerRectPos.x + steerRectSize.x / 2f)
				{
					Vector3 mousePosition3 = UnityEngine.Input.mousePosition;
					if (mousePosition3.y > steerRectPos.y - steerRectSize.y / 2f)
					{
						Vector3 mousePosition4 = UnityEngine.Input.mousePosition;
						if (mousePosition4.y < steerRectPos.y + steerRectSize.y / 2f)
						{
							steeringWheelPointerPos = UnityEngine.Input.mousePosition;
							steeringWheelActive = true;
						}
					}
				}
			}
		}
		if (steeringWheelActive)
		{
			steeringWheelPointerPos = UnityEngine.Input.mousePosition;
			steeringAngle = (steeringWheelPointerPos.x - steerRectPos.x) / (steerRectSize.x * 0.5f);
			if (Mathf.Abs(steeringAngle) > 1f)
			{
				steeringAngle = Mathf.Abs(steeringAngle) * Mathf.Sign(steeringAngle);
			}
			SteeringWheelRot.z = steeringAngle;
			steeringWheel.transform.rotation = Quaternion.Euler(SteeringWheelRot);
		}
		if (Input.GetMouseButtonUp(0))
		{
			steeringWheelActive = false;
			SteeringWheelRot.z = 0f;
			steeringAngle = 0f;
		}
	}

	private void initVars()
	{
		steeringWheelSize = new Vector2((float)W * 0.2f, (float)W * 0.2f);
		steeringWheelPos = new Vector3((float)W * 0.052f + steeringWheelSize.x / 2f, steeringWheelSize.y / 2f, 0f);
		SteeringWheelRot = new Vector3(0f, 0f, 0f);
		steerRectSize = new Vector3((float)W * 0.3f, (float)H * 0.15f);
		steerRectPos = new Vector3((float)W * 0.2f, (float)H * 0.2f);
	}
}
