using UnityEngine;

public class ToggleGameObject : MonoBehaviour
{
	public KeyCode keyCode;
	public new GameObject gameObject;

	private void Update()
	{
		if (Input.GetKeyDown(keyCode))
		{
			gameObject.SetActive(!gameObject.activeSelf);
		}
	}
}
