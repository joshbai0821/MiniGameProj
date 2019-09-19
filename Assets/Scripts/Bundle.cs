using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace MiniProj
{
    public class BundleItem : Reference
    {
        private string m_Name;
        private StringBuilder m_StringBuilder;
        private string m_Error;
        private AssetBundle m_AssetBundle;
        public readonly List<BundleItem> m_Dependencies = new List<BundleItem>();
        public string Name
        {
            get { return m_Name; }
        }
        public AssetBundle Bundle
        {
            get { return m_AssetBundle; }
        }


        public BundleItem(string name)
        {
            m_Name = name;
            m_StringBuilder = new StringBuilder();
        }

        internal virtual void Load()
        {
            m_StringBuilder.Length = 0;
            m_StringBuilder.Append(ResourceManager.AssetBundlePath);
            m_StringBuilder.Append(m_Name);
            m_AssetBundle = AssetBundle.LoadFromFile(m_StringBuilder.ToString());
            if (m_AssetBundle == null)
                m_Error = m_Name + " LoadFromFile failed.";
        }

        internal virtual void Unload()
        {
            if (m_AssetBundle == null)
                return;
            m_AssetBundle.Unload(true);
            m_AssetBundle = null;
        }

    }
}

