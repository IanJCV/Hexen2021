using DAE.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "DAE/Persistent Board Model")]
public class BoardModel : ScriptableObject
{
    public BidirectionalDictionary<Hex, HexCoords> hexes = new BidirectionalDictionary<Hex, HexCoords>();
}