using UnityEngine;

public class JoystickController : MonoBehaviour
{
	public IJoystickListener myInputListener;

	public RectTransform joystickPoint;

	public RectTransform joystickRect;

	public RectTransform joystickSensestiveArea;

	public Canvas canvas;

	private Vector2 touchStartPos = Vector2.zero;

	private Vector2 touchCurrentPos = Vector2.zero;

	private Vector2 delta = Vector2.zero;

	[SerializeField]
	private bool isActive;

	private float k0;

	private float k;

	private int fingerId = -1;

	private Vector2 pos;

	private void Start()
	{
		float scaleFactor = canvas.scaleFactor;
		Vector2 sizeDelta = joystickRect.sizeDelta;
		float num = 1f / (scaleFactor * sizeDelta.x) * canvas.scaleFactor;
		Vector2 sizeDelta2 = joystickRect.sizeDelta;
		k0 = num * sizeDelta2.x;
		joystickRect.gameObject.SetActive(value: false);
	}

	private void Update()
	{
		if (isActive)
		{
			PointerMove();
			k = (touchCurrentPos - touchStartPos).sqrMagnitude * k0;
			k = Mathf.Clamp(k, 0f, 1f);
			myInputListener.SetDelta((touchCurrentPos - touchStartPos).normalized * k);
		}
		else if (!GameController.isMobile)
		{
			delta.x = UnityEngine.Input.GetAxis("Horizontal");
			delta.y = UnityEngine.Input.GetAxis("Vertical");
			myInputListener.SetDelta(delta);
		}
		else
		{
			myInputListener.SetDelta(Vector2.zero);
		}
	}

	public void OnTouchDown()
	{
		if (GameController.isMobile)
		{
			if (fingerId == -1)
			{
				Touch[] touches = Input.touches;
				for (int i = 0; i < touches.Length; i++)
				{
					Touch touch = touches[i];
					if (touch.phase == TouchPhase.Began && RectTransformUtility.RectangleContainsScreenPoint(joystickSensestiveArea, touch.position, canvas.worldCamera))
					{
						touchStartPos = touch.position;
						touchCurrentPos = touchStartPos;
						fingerId = touch.fingerId;
						isActive = true;
						break;
					}
				}
			}
		}
		else
		{
			isActive = true;
			touchStartPos = UnityEngine.Input.mousePosition;
		}
		if (isActive)
		{
			ShowJoystickPanel(b: true);
			RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickSensestiveArea, touchStartPos, canvas.worldCamera, out Vector2 localPoint);
			joystickRect.position = joystickSensestiveArea.transform.TransformPoint(localPoint);
			joystickPoint.position = joystickRect.transform.TransformPoint(Vector2.zero);
		}
	}

	private void ShowJoystickPanel(bool b)
	{
		joystickRect.gameObject.SetActive(b);
	}

	public void OnTouchUp()
	{
		fingerId = -1;
		isActive = false;
		delta = Vector2.zero;
		myInputListener.SetDelta(delta);
		joystickPoint.position = joystickRect.transform.TransformPoint(Vector2.zero);
		ShowJoystickPanel(b: false);
	}

	public void PointerMove()
	{
		if (GameController.isMobile)
		{
			Touch[] touches = Input.touches;
			for (int i = 0; i < touches.Length; i++)
			{
				Touch touch = touches[i];
				if (touch.fingerId == fingerId)
				{
					touchCurrentPos = touch.position;
				}
			}
		}
		else
		{
			touchCurrentPos = UnityEngine.Input.mousePosition;
		}
		RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickRect, touchCurrentPos, canvas.worldCamera, out pos);
		joystickPoint.position = joystickRect.transform.TransformPoint(pos);
	}

	public bool DoesContainPoint(Vector2 point)
	{
		return RectTransformUtility.RectangleContainsScreenPoint(joystickSensestiveArea, point, canvas.worldCamera);
	}

	public void StopActions()
	{
		OnTouchUp();
	}
}
