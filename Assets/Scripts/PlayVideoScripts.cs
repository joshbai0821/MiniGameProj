using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayVideoScripts : MonoBehaviour {

#if UNITY_EDITOR
    public MovieTexture m_movTexture;
#endif
    // Use this for initialization
    void Start () {
#if UNITY_EDITOR
        transform.GetComponent<MeshRenderer>().material.mainTexture = m_movTexture;
        m_movTexture.loop = false;
        m_movTexture.Play();
#endif

    }

}
