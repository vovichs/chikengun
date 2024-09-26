using UnityEngine;

public static class ExtensionsMethods
{
	public static void SetPositionX(this Transform t, float newX)
	{
		Vector3 position = t.position;
		float y = position.y;
		Vector3 position2 = t.position;
		t.position = new Vector3(newX, y, position2.z);
	}

	public static void SetPositionY(this Transform t, float val)
	{
		Vector3 position = t.position;
		float x = position.x;
		Vector3 position2 = t.position;
		t.position = new Vector3(x, val, position2.z);
	}

	public static void SetPositionZ(this Transform t, float newZ)
	{
		Vector3 position = t.position;
		float x = position.x;
		Vector3 position2 = t.position;
		t.position = new Vector3(x, position2.y, newZ);
	}

	public static void SetLocalPositionX(this Transform t, float val)
	{
		Vector3 localPosition = t.localPosition;
		float y = localPosition.y;
		Vector3 localPosition2 = t.localPosition;
		t.localPosition = new Vector3(val, y, localPosition2.z);
	}

	public static void SetLocalPositionY(this Transform t, float val)
	{
		Vector3 localPosition = t.localPosition;
		float x = localPosition.x;
		Vector3 localPosition2 = t.localPosition;
		t.localPosition = new Vector3(x, val, localPosition2.z);
	}

	public static void SetLocalPositionZ(this Transform t, float val)
	{
		Vector3 localPosition = t.localPosition;
		float x = localPosition.x;
		Vector3 localPosition2 = t.localPosition;
		t.localPosition = new Vector3(x, localPosition2.y, val);
	}

	public static void SetLocalEulerX(this Transform t, float newX)
	{
		Vector3 localEulerAngles = t.localEulerAngles;
		float y = localEulerAngles.y;
		Vector3 localEulerAngles2 = t.localEulerAngles;
		t.localEulerAngles = new Vector3(newX, y, localEulerAngles2.z);
	}

	public static void SetLocalEulerY(this Transform t, float newY)
	{
		Vector3 localEulerAngles = t.localEulerAngles;
		float x = localEulerAngles.x;
		Vector3 localEulerAngles2 = t.localEulerAngles;
		t.localEulerAngles = new Vector3(x, newY, localEulerAngles2.z);
	}

	public static void SetLocalEulerZ(this Transform t, float newZ)
	{
		Vector3 localEulerAngles = t.localEulerAngles;
		float x = localEulerAngles.x;
		Vector3 localEulerAngles2 = t.localEulerAngles;
		t.localEulerAngles = new Vector3(x, localEulerAngles2.y, newZ);
	}

	public static void SetEulerX(this Transform t, float val)
	{
		Vector3 eulerAngles = t.eulerAngles;
		float y = eulerAngles.y;
		Vector3 eulerAngles2 = t.eulerAngles;
		t.eulerAngles = new Vector3(val, y, eulerAngles2.z);
	}

	public static void SetEulerY(this Transform t, float val)
	{
		Vector3 eulerAngles = t.eulerAngles;
		float x = eulerAngles.x;
		Vector3 eulerAngles2 = t.eulerAngles;
		t.eulerAngles = new Vector3(x, val, eulerAngles2.z);
	}

	public static void SetEulerZ(this Transform t, float val)
	{
		Vector3 eulerAngles = t.eulerAngles;
		float x = eulerAngles.x;
		Vector3 eulerAngles2 = t.eulerAngles;
		t.eulerAngles = new Vector3(x, eulerAngles2.y, val);
	}

	public static void SetLocalScaleX(this Transform t, float newX)
	{
		Vector3 localScale = t.localScale;
		float y = localScale.y;
		Vector3 localScale2 = t.localScale;
		t.localScale = new Vector3(newX, y, localScale2.z);
	}

	public static void SetLocalScaleY(this Transform t, float newY)
	{
		Vector3 localScale = t.localScale;
		float x = localScale.x;
		Vector3 localScale2 = t.localScale;
		t.localScale = new Vector3(x, newY, localScale2.z);
	}

	public static void SetLocalScaleZ(this Transform t, float newZ)
	{
		Vector3 localScale = t.localScale;
		float x = localScale.x;
		Vector3 localScale2 = t.localScale;
		t.localScale = new Vector3(x, localScale2.y, newZ);
	}
}
