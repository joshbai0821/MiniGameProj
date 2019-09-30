using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayVideoScripts : MonoBehaviour {

    public MovieTexture m_movTexture;
    // Use this for initialization
    void Start () {
        transform.GetComponent<MeshRenderer>().material.mainTexture = m_movTexture;
        m_movTexture.loop = false;
        m_movTexture.Play();
    }

}
