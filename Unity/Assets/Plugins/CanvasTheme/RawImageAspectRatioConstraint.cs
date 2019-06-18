using UnityEngine;
using UnityEngine.UI;

namespace CanvasTheme
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(RawImage))]
	[RequireComponent(typeof(LayoutElement))]
	public class RawImageAspectRatioConstraint : MonoBehaviour
	{
		public enum Mode
		{
			ChangeHeight,
			ChangeWidth,
		}

		[SerializeField]
		private Mode mode = Mode.ChangeHeight;

		private void Update()
		{
			var texture = GetComponent<RawImage>().mainTexture;
			var layoutElement = GetComponent<LayoutElement>();

			switch (mode)
			{
				case Mode.ChangeHeight:
					layoutElement.preferredHeight = (layoutElement.preferredWidth * texture.height) / texture.width;
					break;

				case Mode.ChangeWidth:
					layoutElement.preferredWidth = (layoutElement.preferredHeight * texture.width) / texture.height;
					break;
			}
		}
	}
}
