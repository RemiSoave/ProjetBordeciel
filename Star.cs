using UnityEngine;

public class Star : MonoBehaviour
{
    public int starID;

    public int hip;
    public float magnitude;
    public bool isLocked = true;
    public string constellationCode;

    Color darkRed = new Color(0.5450981f, 0f, 0f, 1f);

    // Change la couleur de l'étoile (ex: rouge sombre quand sélectionnée)
    public void SetHighlight(bool active)
    {
        Renderer r = GetComponent<Renderer>();
        if (r != null)
        {
            r.material.SetColor("_Color", active ? darkRed : Color.white);
        }
    }

    // Lock/Unlock la collider et alpha
    public void Lock(bool value)
    {
        isLocked = value;
        GetComponent<Collider>().enabled = !value;
        GetComponent<Renderer>().material.SetFloat("_Alpha", value ? 0.2f : 1f);
    }
}
