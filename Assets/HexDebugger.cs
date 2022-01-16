using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HexDebugger : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI t;
    private void Awake()
    {
        string[] coordDelim = gameObject.name.Split('^');
        t.text = $"{new HexCoords(int.Parse(coordDelim[1]), int.Parse(coordDelim[2]), int.Parse(coordDelim[3]))}";
    }
}
