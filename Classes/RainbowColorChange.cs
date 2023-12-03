
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
    private bool changeColor = true;
    public float hue = 0.166666672f;
    public float saturation = 1f;
    public float brightness = 1f;
    public int edgeThickness = 2;

    public void StartChange() => StartCoroutine(ChangeColor());

    public void StopChange() => changeColor = false;

    private Color CalculateRainbowColor(float speed) => new Color((float) (((double) Mathf.Sin(Time.time * speed) + 1.0) / 2.0), (float) (((double) Mathf.Sin((float) ((double) Time.time * (double) speed + 2.0943951606750488)) + 1.0) / 2.0), (float) (((double) Mathf.Sin((float) ((double) Time.time * (double) speed + 4.1887903213500977)) + 1.0) / 2.0));

    private IEnumerator ChangeColor()
    {
      while (changeColor)
      {
        Color rainbowColor = CalculateRainbowColor(MelonPreferences.GetEntryValue<float>("Bluedescriptor", "nameplate-speed"));
                imageToChange.color = rainbowColor;
        yield return  null;
        rainbowColor = new Color();
      }
    }

    public void StartChangeText() => StartCoroutine(ChangeColorText());

    private IEnumerator ChangeColorText()
    {
      while (changeColor)
      {
        Color rainbowColor = CalculateRainbowColor(MelonPreferences.GetEntryValue<float>("Bluedescriptor", "nameplate-speed"));
                textToChange.color = rainbowColor;
        yield return  null;
        rainbowColor = new Color();
      }
    }

    public void StartChangeTextOutline() => StartCoroutine(ChangeColorTextOutline());

    private IEnumerator ChangeColorTextOutline()
    {
      while (changeColor)
      {
        float speed = 5f;
        float r = Mathf.Sin(Time.time * speed);
        float g = Mathf.Sin((float) ((double) Time.time * (double) speed + 2.0943951606750488));
        float b = Mathf.Sin((float) ((double) Time.time * (double) speed + 4.1887903213500977));
        r = (float) (((double) r + 1.0) / 2.0);
        g = (float) (((double) g + 1.0) / 2.0);
        b = (float) (((double) b + 1.0) / 2.0);
        textToChange.outlineColor = new Color(r, g, b);
        yield return  null;
      }
    }



    private bool IsEdgePixel(Texture2D texture, int x, int y) => texture.GetPixel(x, y).a != 0.0 && (x > 0 && texture.GetPixel(x - 1, y).a == 0.0 || x < texture.width - 1 && texture.GetPixel(x + 1, y).a == 0.0 || y > 0 && texture.GetPixel(x, y - 1).a == 0.0 || y < texture.height - 1 && texture.GetPixel(x, y + 1).a == 0.0);
  }
}
