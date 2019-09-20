using UnityEngine;

[ExecuteInEditMode]
public class ButtonUI : MonoBehaviour
{
	public new Camera camera;
	public Transform target;

	private void Update()
	{
		var rectTransform = GetComponent<RectTransform>();
		var canvasRectTransform = transform.GetComponentInParent<Canvas>().GetComponent<RectTransform>();

		Vector2 viewportPosition = camera.WorldToViewportPoint(target.position);
		var screenPosition = new Vector2(
			canvasRectTransform.sizeDelta.x * (viewportPosition.x),
			canvasRectTransform.sizeDelta.y * (viewportPosition.y - 1f)
		 );

		rectTransform.anchoredPosition = screenPosition;
	}
}
