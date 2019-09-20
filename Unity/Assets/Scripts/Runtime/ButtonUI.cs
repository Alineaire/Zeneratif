using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ButtonUI : MonoBehaviour
{
	public new Camera camera;
	public Transform target;

	void Update()
	{
		Vector2 ViewportPosition = camera.WorldToViewportPoint(target.position);
		GetComponent<RectTransform>().anchoredPosition = new Vector2(
			ViewportPosition.x*1024f,
			(ViewportPosition.y-1f)*768f
		);
	}
}
