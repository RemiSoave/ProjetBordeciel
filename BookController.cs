using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ConstellationBook : MonoBehaviour
{
    public ConstellationData[] allConstellations;
    private int index = 0;

    // LEFT PAGE
    public Image leftImage;
    public Text leftTitle;

    // RIGHT PAGE
    public Text rightDescription;

    // Renderer pour highlight
    public ConstellationRenderer constellationRenderer;

    

    void Start()
    {
        ShowPage();
    }

    void ShowPage()
    {
        var c = allConstellations[index];

        leftImage.sprite = c.image;
        leftTitle.text = c.displayName;
        rightDescription.text = c.description;
    }

    public void NextPage()
    {
        index++;
        if (index >= allConstellations.Length) index = 0;
        ShowPage();
    }

    public void PrevPage()
    {
        index--;
        if (index < 0) index = allConstellations.Length - 1;
        ShowPage();
    }

    public void HighlightCurrent()
    {
        string code = allConstellations[index].code;
        if (code == "All")
    {
        constellationRenderer.ShowAllConstellations();
        return;
    }
        constellationRenderer.ShowConstellation(code);

        constellationRenderer.HighlightConstellation(code);
    }
}
