using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class StarGenerator : MonoBehaviour
{
    public TextAsset csvFile;
    public GameObject starPrefab;
    public float sphereRadius = 50f;
    public float maxMagnitude = 5.5f;

    public Dictionary<int, GameObject> starsByID = new Dictionary<int, GameObject>();
    public Dictionary<int, GameObject> starsByHIP = new Dictionary<int, GameObject>();

    public List<StarInfo> starsList = new List<StarInfo>();

    public static StarGenerator Instance;

    public System.Action OnStarsReady; // événement pour signaler la fin

    void Awake() => Instance = this;

    void Start()
    {
        GenerateStarsFromCSV();
        OnStarsReady?.Invoke(); // signal que toutes les étoiles sont créées
    }

    void GenerateStarsFromCSV()
{
    string[] lines = csvFile.text.Split('\n');

    for (int i = 1; i < lines.Length; i++) // skip header
    {
        string line = lines[i].Trim();
        if (string.IsNullOrWhiteSpace(line)) continue;

        string[] cols = line.Split(',');
        if (cols.Length < 22) continue; // sécurité : la colonne "con" existe ?

        if (!int.TryParse(cols[1], out int hip)) continue;
        if (!float.TryParse(cols[7], NumberStyles.Float, CultureInfo.InvariantCulture, out float raHours)) continue;
        if (!float.TryParse(cols[8], NumberStyles.Float, CultureInfo.InvariantCulture, out float dec)) continue;
        if (!float.TryParse(cols[13], NumberStyles.Float, CultureInfo.InvariantCulture, out float mag)) continue;

        // récupérer le code de constellation depuis la colonne 'con'
        string constellationCode = cols[29].Trim(); // 0-indexed => 21e colonne

        if (mag > maxMagnitude &&
            hip != 12387 && hip != 10826 && hip != 8645 &&
            hip != 55203 && hip != 102831 && hip != 102989 && hip != 55219)
            continue;

        CreateStar(i, hip, raHours, dec, mag, constellationCode);
    }

    Debug.Log($"🌌 Stars loaded: {starsByHIP.Count} HIP indexed");
}

void CreateStar(int id, int hip, float raHours, float decDeg, float mag, string constellationCode)
{
    float raDeg = raHours * 15f;
    Vector3 dir = RaDecToDirection(raDeg, decDeg);
    Vector3 pos = dir * sphereRadius;

    GameObject starGO = Instantiate(starPrefab, pos, Quaternion.identity, transform);

    starsByID[id] = starGO;
    starsByHIP[hip] = starGO;

    Star starComp = starGO.GetComponent<Star>();
    if (starComp == null)
        starComp = starGO.AddComponent<Star>();

    starComp.starID = id;
    starComp.hip = hip;
    starComp.magnitude = mag;
    starComp.constellationCode = constellationCode; // ✅ assigné ici

    starsList.Add(new StarInfo { hip = hip, ra = raDeg, dec = decDeg, go = starGO });

    float size = Mathf.Lerp(1.2f, 0.2f, Mathf.InverseLerp(-1.5f, 6f, mag));
    starGO.transform.localScale = Vector3.one * size;
}


    Vector3 RaDecToDirection(float raDeg, float decDeg)
    {
        float ra = raDeg * Mathf.Deg2Rad;
        float dec = decDeg * Mathf.Deg2Rad;

        return new Vector3(
            Mathf.Cos(dec) * Mathf.Cos(ra),
            Mathf.Sin(dec),
            Mathf.Cos(dec) * Mathf.Sin(ra)
        ).normalized;
    }
    
}

// Classe helper
public class StarInfo
{
    public int hip;
    public float ra;
    public float dec;
    public GameObject go;
}
