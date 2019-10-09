using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniProj
{
    public class SceneFinal : MonoBehaviour
    {
        private bool m_isOn = false;
        private void OnTriggerEnter(Collider other)
        {

            if (!m_isOn && other.tag == "Player")
            {
                SceneModule _sceneModule = (SceneModule)GameManager.GameManagerObj.GetComponent<GameManager>().GetModuleByName("SceneModule");
                if (GameManager.SceneConfigId == 1 && other.name == "xiangyu(Clone)")
                {
                    _sceneModule.ArriveSceneFinal();
                    m_isOn = true;
                }
                else if (GameManager.SceneConfigId == 2 && other.name == "yuji(Clone)")
                {
                    _sceneModule.ArriveSceneFinal();
                    m_isOn = true;
                }
                else if (GameManager.SceneConfigId == 3 && other.name == "yuji(Clone)")
                {
                    _sceneModule.ArriveSceneFinal();
                    m_isOn = true;
                }
            }
        }
    }
}

