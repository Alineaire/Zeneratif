using UnityEngine;
using UnityEngine.UI;

namespace CanvasTheme
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Canvas))]
	[RequireComponent(typeof(Image))]
	public class CanvasThemeAssigner : MonoBehaviour
	{
		public CanvasTheme theme;

		private Material material;

		private void Update()
		{
			if (theme == null)
			{
				Debug.LogWarning("Canvas theme is not assigned.");
				return;
			}

			var image = GetComponent<Image>();
			image.color = theme.canvasBackgroundColor;
			material = image.material;

			BrowseChildren(transform);
		}

		private void ApplyText(Text text, TextTheme theme)
		{
			text.font = theme.font;
			text.fontStyle = theme.fontStyle;
			text.fontSize = theme.fontSize;
			text.lineSpacing = theme.lineSpacing;
			text.supportRichText = theme.richText;
			text.alignment = theme.alignment;
			text.alignByGeometry = theme.alignByGeometry;
			text.horizontalOverflow = theme.horizontalOverflow;
			text.verticalOverflow = theme.verticalOverflow;
			text.resizeTextForBestFit = theme.bestFit;
			text.color = theme.color;
		}

		private void BrowseChildren(Transform parent)
		{
			Graphic graphic;

			Button button;
			Dropdown dropdown;
			Image image;
			InputField inputField;
			Text text;
			Toggle toggle;

			foreach (Transform child in parent)
			{
				if (button = child.GetComponent<Button>())
				{
					button.colors = theme.buttonColors;

					var texts = child.GetComponentsInChildren<Text>();
					if (texts.Length == 1)
					{
						ApplyText(texts[0], theme.buttonText);
					}
				}

				else if (dropdown = child.GetComponent<Dropdown>())
				{
					dropdown.colors = theme.dropdownColors;
				}

				else if (inputField = child.GetComponent<InputField>())
				{
					inputField.colors = theme.inputFieldColors;

					var texts = child.GetComponentsInChildren<Text>();
					if (texts.Length == 2)
					{
						ApplyText(texts[0], theme.inputFieldPlaceholder);
						ApplyText(texts[1], theme.inputFieldText);
					}
				}

				else if (child.GetComponent<ScrollRect>())
				{
					if (image = child.GetComponent<Image>())
					{
						image.color = theme.scrollRectBackgroundColor;
					}
				}

				else if (text = child.GetComponent<Text>())
				{
					ApplyText(text, theme.text);
				}

				else if (toggle = child.GetComponent<Toggle>())
				{
					toggle.colors = theme.toggleColors;
				}

				else
				{
					if (graphic = child.GetComponent<Graphic>())
					{
						graphic.material = material;
					}

					BrowseChildren(child);
				}
			}
		}
	}
}
