using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniProj
{
    public class BridgeSwitch : MonoBehaviour
    {
        private bool m_isopen = false;
        private Vector2 switchpos;
        public Animation m_anim;
        public GameObject m_plane1;
        public GameObject m_plane2;
        private bool m_isOn = false;

        void Start()
        {
            switchpos = new Vector2(transform.parent.GetComponent<MapData>().Pos.m_row, transform.parent.GetComponent<MapData>().Pos.m_col);
        }


        private void OnTriggerEnter(Collider other)
        {

            SceneModule _sceneModule = (SceneModule)GameManager.GameManagerObj.GetComponent<GameManager>().GetModuleByName("SceneModule");
            if (!m_isopen && other.tag == "Player")
            {
                if (other.GetComponent<Player>().Pos.m_row == switchpos.x && other.GetComponent<Player>().Pos.m_col == switchpos.y)
                {
                    m_isopen = true;
                    GetComponent<AudioSource>().Play();
                    m_anim.Play();

                    if (!m_isOn && other.tag == "Player")
                    {
                        if (m_plane1 != null)
                        {
                            m_plane1.GetComponent<Animation>().Play();
                            m_plane1.GetComponent<BoxCollider>().enabled = true;
                            MapData _mapData1 = m_plane1.GetComponent<MapData>();
                            int _row1 = _mapData1.Pos.m_row;
                            int _col1 = _mapData1.Pos.m_col;
                            _mapData1.Data = MapDataType.PINGDI;
                            _sceneModule.m_mapData[_row1][_col1] = MapDataType.PINGDI;
                        }
                        if (m_plane2 != null)
                        {
                            m_plane2.GetComponent<Animation>().Play();
                            m_plane2.GetComponent<BoxCollider>().enabled = true;
                            MapData _mapData2 = m_plane2.GetComponent<MapData>();
                            int _row2 = _mapData2.Pos.m_row;
                            int _col2 = _mapData2.Pos.m_col;
                            _mapData2.Data = MapDataType.PINGDI;
                            _sceneModule.m_mapData[_row2][_col2] = MapDataType.PINGDI;

                        }

                    }

                }

            }
        }
    }
}

