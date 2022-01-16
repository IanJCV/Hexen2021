using UnityEngine;

namespace DAE.Commons
{

    internal class DontDestroyOnLoad : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}