using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapData : MonoBehaviour {
    [SerializeField]
    private int m_data;
    public int Data
    {
        get { return m_data; }
    }
}
