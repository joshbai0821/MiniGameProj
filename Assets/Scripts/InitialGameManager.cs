using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniProj
{
    public class InitialGameManager : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {
            if(GameManager.GameManagerObj == null)
            {
                Object _obj = Resources.Load("GameManager", typeof(GameObject));
                GameObject _gameObj = Instantiate(_obj) as GameObject;
            }
        }
    }
}

