using System.Collections.Generic;
using UnityEngine;

public class ConstellationRenderer : MonoBehaviour
{
    public TextAsset constellationCsv;

    private Dictionary<string, List<GameObject>> constellationLines = new Dictionary<string, List<GameObject>>();

    public StarGenerator starGenerator;
    public Material lineMaterial;
    public float lineWidth = 0.05f;

    // Les constellations sélectionnées via ScriptableObjects
    public List<ConstellationData> constellationsData;
    private HashSet<string> allowedCodes;

    void Start()
    {
        Debug.Log("ConstellationRenderer est bien actif");

        if (constellationCsv == null)
        {
            Debug.LogError("constellationCsv non assigné");
            return;
        }

        if (starGenerator == null)
        {
            Debug.LogError("starGenerator non assigné");
            return;
        }

        // Construire le HashSet des codes autorisés
        allowedCodes = new HashSet<string>();
        if (constellationsData != null)
        {
            foreach (var c in constellationsData)
            {
                if (c != null && !string.IsNullOrEmpty(c.code))
                    allowedCodes.Add(c.code);
            }
        }

        Debug.Log($"{allowedCodes.Count} constellations autorisées à dessiner");

        // S'abonner à l'événement
        starGenerator.OnStarsReady += DrawConstellationsFromCSV;

        // Si les étoiles sont déjà prêtes (StarGenerator Start a déjà tourné)
        if (starGenerator.starsByHIP.Count > 0)
        {
            Debug.Log("Etoiles créées, appel direct de DrawConstellationsFromCSV");
            DrawConstellationsFromCSV();
        }
    }

    void DrawConstellationsFromCSV()
    {
        Debug.Log("Début de DrawConstellationsFromCSV");

        string[] lines = constellationCsv.text.Split('\n');
        int drawnLines = 0;

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] cols = line.Split(',');
            if (cols.Length < 4) continue;

            string raw = cols[0].Trim();
            string code = raw.Split(' ')[^1]; // dernier mot = And, Ori, etc.

            if (!allowedCodes.Contains(code))
                continue;

            if (!int.TryParse(cols[2], out int hip1)) continue;
            if (!int.TryParse(cols[3], out int hip2)) continue;

            if (!starGenerator.starsByHIP.TryGetValue(hip1, out GameObject star1))
            {
                Debug.LogWarning($"HIP manquant: {hip1}");
                continue;
            }

            if (!starGenerator.starsByHIP.TryGetValue(hip2, out GameObject star2))
            {
                Debug.LogWarning($"HIP manquant: {hip2}");
                continue;
            }

            // Assigner le code de constellation aux étoiles
            AssignConstellation(star1, code);
            AssignConstellation(star2, code);

            // Dessiner la ligne
            DrawLineBetweenStars(star1, star2, hip1, hip2, code);
            drawnLines++;
        }

        Debug.Log($"{drawnLines} lignes des constellations dessinées selon les ScriptableObjects");
    }

    void AssignConstellation(GameObject starGO, string code)
    {
        Star s = starGO.GetComponent<Star>();
        if (s == null)
        {
            Debug.LogError($"Composant manquant sur {starGO.name}");
            return;
        }

        s.constellationCode = code;
        // Debug pour vérifier
        Debug.Log($"{starGO.name} assignée à la constellation {code}");
    }

    void DrawLineBetweenStars(GameObject star1, GameObject star2, int hip1, int hip2, string code)
{
    GameObject lineGO = new GameObject($"Line_{code}_{hip1}_{hip2}");
    lineGO.transform.parent = transform;

    LineRenderer lr = lineGO.AddComponent<LineRenderer>();
    lr.positionCount = 2;
    lr.SetPosition(0, star1.transform.position);
    lr.SetPosition(1, star2.transform.position);

    if (lineMaterial != null)
        lr.material = new Material(lineMaterial);

    lr.startWidth = lineWidth;
    lr.endWidth = lineWidth;

    // couleur
    Star s1 = star1.GetComponent<Star>();
    Star s2 = star2.GetComponent<Star>();
    float avgMag = (s1 != null && s2 != null) ? (s1.magnitude + s2.magnitude) / 2f : 3f;
    lr.material.color = Color.Lerp(Color.white, new Color(0.5f, 0.7f, 1f), Mathf.InverseLerp(-1.5f, 6f, avgMag));

    // IMPORTANT : cacher par défaut
    lineGO.SetActive(false);

    // stocker par constellation
    if (!constellationLines.ContainsKey(code))
        constellationLines[code] = new List<GameObject>();

    constellationLines[code].Add(lineGO);
}


    public void HighlightConstellation(string code)
{
    Debug.Log($"Highlight constellation {code}");

    foreach (var kvp in starGenerator.starsByHIP)
    {
        Star s = kvp.Value.GetComponent<Star>();
        if (s == null) continue;

        bool match = s.constellationCode == code;
        s.SetHighlight(match);
    }
}

public void ShowAllConstellations()
{
    Debug.Log("constellations toutes affichées");

    foreach (var kvp in constellationLines)
        foreach (var go in kvp.Value)
            go.SetActive(true);
}
public void ShowConstellation(string code)
{
    Debug.Log($"Constellation {code} affichée");

    // cacher tout
    foreach (var kvp in constellationLines)
        foreach (var go in kvp.Value)
            go.SetActive(false);

    // montrer celle demandée
    if (!constellationLines.ContainsKey(code))
    {
        Debug.LogWarning($"Pas de lignes pour la constellation {code}");
        return;
    }

    foreach (var go in constellationLines[code])
        go.SetActive(true);
}

}
