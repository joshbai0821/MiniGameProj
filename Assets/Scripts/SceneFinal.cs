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
            if (!m_isOn && other.gameObject.tag == "Player")
            {
                SceneModule _sceneModule = (SceneModule)GameManager.GameManagerObj.GetComponent<GameManager>().GetModuleByName("SceneModule");
                _sceneModule.ArriveSceneFinal();
                m_isOn = true;
            }
        }
    }
}

