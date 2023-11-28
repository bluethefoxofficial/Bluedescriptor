using MelonLoader;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Bluedescriptor_Rewritten.Classes
{
    public class RainbowColorChange : MonoBehaviour
    {
        public Image imageToChange;
        public TMP_Text textToChange;
        bool changeColor = true;

        public void StartChange() => StartCoroutine(ChangeColor());

        public void StopChange() => changeColor = false;
        private Color CalculateRainbowColor(float speed)
        {
            var r = Mathf.Sin(Time.time * speed);
            var g = Mathf.Sin(Time.time * speed + 2 * Mathf.PI / 3); // phase shift
            var b = Mathf.Sin(Time.time * speed + 4 * Mathf.PI / 3); // phase shift

            return new Color((r + 1) / 2, (g + 1) / 2, (b + 1) / 2);
        }
        IEnumerator ChangeColor()
        {
            while (changeColor)
            {
                var rainbowColor = CalculateRainbowColor(MelonPreferences.GetEntryValue<float>("Bluedescriptor", "nameplate-speed"));
                imageToChange.color = rainbowColor;
                yield return null;
            }
        }

        // text color change
        public void StartChangeText() => StartCoroutine(ChangeColorText());

        IEnumerator ChangeColorText()
        {
            while (changeColor)
            {
                var rainbowColor = CalculateRainbowColor(MelonPreferences.GetEntryValue<float>("Bluedescriptor", "nameplate-speed"));
                textToChange.color = rainbowColor;
                yield return null;
            }
        }
        public void StartChangeTextOutline() => StartCoroutine(ChangeColorTextOutline());

        IEnumerator ChangeColorTextOutline()
        {
            while (changeColor)
            {
                var speed = 5.0f;
                var r = Mathf.Sin(Time.time * speed);
                var g = Mathf.Sin(Time.time * speed + 2 * Mathf.PI / 3); // phase shift
                var b = Mathf.Sin(Time.time * speed + 4 * Mathf.PI / 3); // phase shift

                r = (r + 1) / 2;
                g = (g + 1) / 2;
                b = (b + 1) / 2;

                textToChange.outlineColor = new Color(r, g, b);
                yield return null;
            }
        }

        public float hue = 1 / 6f; // Hue for yellow
        public float saturation = 1f; // Saturation for yellow
        public float brightness = 1f; // Brightness for yellow
        public int edgeThickness = 2; // Thickness of the edge in pixels

        public IEnumerator ChangeHueOnEdgesCoroutine(Texture2D tex)
        {
            var mainTexture = imageToChange.material.mainTexture as Texture2D;

            // Create a new texture to store the result
            var resultTexture = new Texture2D(mainTexture.width, mainTexture.height);

            for (int x = 0; x < mainTexture.width; x++)
            {
                for (int y = 0; y < mainTexture.height; y++)
                {
                    var originalColor = mainTexture.GetPixel(x, y);

                    // Check if the current pixel is black
                    if (originalColor == Color.black)
                    {
                        // Set the color to yellow
                        resultTexture.SetPixel(x, y, Color.HSVToRGB(hue, saturation, brightness));
                    }
                    else if (IsEdgePixel(mainTexture, x, y))
                    {
                        // Apply the hue change to a band of pixels around the edge
                        for (int i = -edgeThickness; i <= edgeThickness; i++)
                        {
                            for (int j = -edgeThickness; j <= edgeThickness; j++)
                            {
                                var newX = x + i;
                                var newY = y + j;

                                if (newX >= 0 && newX < mainTexture.width && newY >= 0 && newY < mainTexture.height)
                                {
                                    var edgeColor = mainTexture.GetPixel(newX, newY);
                                    Color.RGBToHSV(edgeColor, out float originalHue, out float S, out float V);
                                    originalHue += hue;
                                    originalHue = originalHue % 1;
                                    var newColor = Color.HSVToRGB(originalHue, S, V);
                                    resultTexture.SetPixel(newX, newY, newColor);
                                }
                            }
                        }
                    }
                    else
                    {
                        // Keep the original color for non-edge pixels
                        resultTexture.SetPixel(x, y, originalColor);
                    }
                }

                // Yield return after processing each row of pixels
                yield return null;
            }

            resultTexture.Apply();
            imageToChange.material.mainTexture = resultTexture;
        }

        bool IsEdgePixel(Texture2D texture, int x, int y)
        {
            var currentPixel = texture.GetPixel(x, y);

            // If the current pixel is transparent, it's not an edge
            if (currentPixel.a == 0) return false;

            // Check the surrounding pixels
            if (x > 0 && texture.GetPixel(x - 1, y).a == 0) return true;
            if (x < texture.width - 1 && texture.GetPixel(x + 1, y).a == 0) return true;
            if (y > 0 && texture.GetPixel(x, y - 1).a == 0) return true;
            if (y < texture.height - 1 && texture.GetPixel(x, y + 1).a == 0) return true;

            return false;
        }
    }
}