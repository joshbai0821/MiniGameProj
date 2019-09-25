using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniProj
{
    public class BridgeSwitch : MonoBehaviour
    {

        public GameObject m_plane1;
        public GameObject m_plane2;
        private bool m_isOn = false;

        private void OnTriggerEnter(Collider other)
        {
            if (!m_isOn && other.gameObject.tag == "Player")
            {
                m_plane1.SetActive(true);
                m_plane2.SetActive(true);
                int _row1 = m_plane1.GetComponent<MapData>().Pos.m_row;
                int _col1 = m_plane1.GetComponent<MapData>().Pos.m_col;
                int _row2 = m_plane2.GetComponent<MapData>().Pos.m_row;
                int _col2 = m_plane2.GetComponent<MapData>().Pos.m_col;
                SceneModule _sceneModule = (SceneModule)GameManager.GameManagerObj.GetComponent<GameManager>().GetModuleByName("SceneModule");
                _sceneModule.Data[_row1][_col1] = MapDataType.PINGDI;
                _sceneModule.Data[_row2][_col2] = MapDataType.PINGDI;
            }

        }
    }
}

