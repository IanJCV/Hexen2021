using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BoardGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject boardParent = default;

    [SerializeField]
    private GameObject hexPrefab = default;

    [SerializeField]
    private int size = 0;

    [SerializeField]
    private float offset = 0f;

    private float hexRadius
    {
        get
        {
            return hexPrefab.GetComponentInChildren<MeshRenderer>().bounds.extents.z;
        }
    }

    public List<GameObject> tiles = new List<GameObject>();


    [ContextMenu("Create Board")]
    private void CreateBoard()
    {

        for (int q = -size; q <= size; q++)
        {
            int r1 = Mathf.Max(-size, -q - size);
            int r2 = Mathf.Min(size, -q + size);

            for (int r = r1; r <= r2; r++)
            {
                var s = -q - r;

                GameObject go = Instantiate(hexPrefab, HexToWorld(new HexCoords(q, r, s)), Quaternion.identity, boardParent.transform);
                go.name = $"Tile ^{q}^{r}^{s}";
                go.GetComponent<Hex>().coords = new HexCoords(q, r, s);
                tiles.Add(go);
            }
        }

        string json = JsonUtility.ToJson(new BoardInfo(size, offset, hexRadius));
        File.WriteAllText(Application.dataPath + "/boardinfo.json", json);
        Debug.Log(Application.dataPath + "/boardinfo.json");
    }

    [ContextMenu("ClearBoard")]
    private void ClearBoard()
    {
        if (tiles.Count == 0)
            return;

        foreach (var t in tiles)
        {
            DestroyImmediate(t);
        }
        tiles.Clear();
    }


    public Vector3 HexToWorld(HexCoords hex)
    {
        Vector3 pos = Vector3.zero;
        pos.x = (hexRadius * Mathf.Sqrt(3.0f) + offset) * (hex.q + hex.r / 2.0f);
        pos.z = (hexRadius * 3.0f + offset) / 2.0f * hex.r;

        return pos;
    }

    public HexCoords WorldToHex(Vector3 pos)
    {
        var q = (Mathf.Sqrt(3f) / 3f * pos.x - 1f / 3f * pos.z) / (hexRadius + offset);
        var r = (2f / 3f * pos.z) / (hexRadius - offset);

        q = (float)pos.z / ((hexRadius + offset) * 0.75f);
        r = ((float)pos.x + (0.433f * q)) / 0.866f;

        Debug.DrawRay(HexToWorld(new HexCoords((int)q, (int)r)), Vector3.up, Color.green, 120f);
        return Round((q, r, -q - r));
    }

    public static HexCoords Round((float q, float r, float s) frac)
    {
        int q = Mathf.RoundToInt(frac.q);
        int r = Mathf.RoundToInt(frac.r);
        int s = Mathf.RoundToInt(frac.s);

        var q_diff = Mathf.Abs(q - frac.q);
        var r_diff = Mathf.Abs(r - frac.r);
        var s_diff = Mathf.Abs(s - frac.s);

        if (q_diff > r_diff && q_diff > s_diff)
            q = -r - s;
        else if (r_diff > s_diff)
            r = -q - s;
        else
            s = -q - r;

        return new HexCoords(q, r, s);
    }

    [ContextMenu("Test Maths")]
    public void Test()
    {
        var hex = new HexCoords(3, -3, 0);
        var worldPos = HexToWorld(hex);
        var hexPos = WorldToHex(worldPos);
        var worldPos2 = HexToWorld(hexPos);
        if (worldPos2 == worldPos || Vector3.Distance(worldPos2, worldPos) <= 0.3f)
        {
            Debug.Log("test passed");
        }
        Debug.Log($"{hexRadius} {offset}");
        Debug.Log($"hex before conversion = {hex.q} {hex.r} {hex.s}, worldpos = {worldPos}, hex after conversion back = {hexPos.q} {hexPos.r} {hexPos.s}, hex to world = {worldPos2}");
    }
}

[System.Serializable]
public class BoardInfo
{
    public int boardSize;
    public float offset;
    public float hexRadius;

    public BoardInfo(int boardSize, float offset, float hexRadius)
    {
        this.boardSize = boardSize;
        this.offset = offset;
        this.hexRadius = hexRadius;
    }
}
