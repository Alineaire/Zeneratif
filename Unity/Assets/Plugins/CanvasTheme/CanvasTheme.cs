using System;
using UnityEngine;
using UnityEngine.UI;

namespace CanvasTheme
{
	[Serializable]
	public class TextTheme
	{
		public Font font;
		public FontStyle fontStyle;
		public int fontSize;
		public float lineSpacing;
		public bool richText;
		public TextAnchor alignment;
		public bool alignByGeometry;
		public HorizontalWrapMode horizontalOverflow;
		public VerticalWrapMode verticalOverflow;
		public bool bestFit;
		public Color color;
	}

	[CreateAssetMenu]
	public class CanvasTheme : ScriptableObject
	{
		[Header("Button")]
		public ColorBlock buttonColors;
		public TextTheme buttonText;

		[Header("Canvas")]
		public Color canvasBackgroundColor;

		[Header("Dropdown")]
		public ColorBlock dropdownColors;

		[Header("Input Field")]
		public ColorBlock inputFieldColors;
		public TextTheme inputFieldPlaceholder;
		public TextTheme inputFieldText;

		[Header("Scroll Rect")]
		public Color scrollRectBackgroundColor;

		[Header("Text")]
		public TextTheme text;

		[Header("Toggle")]
		public ColorBlock toggleColors;
	}
}
