using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace MiniProj
{
    public class BundleItem : Reference
    {
        private string m_name;
        private StringBuilder m_stringBuilder;
        private string m_error;
        private AssetBundle m_assetBundle;
        public readonly List<BundleItem> m_dependencies = new List<BundleItem>();
        public string Name
        {
            get { return m_name; }
        }
        public AssetBundle Bundle
        {
            get { return m_assetBundle; }
        }

        public string Error
        {
            get { return m_error; }
        }


        public BundleItem(string name)
        {
            m_name = name;
            m_stringBuilder = new StringBuilder();
        }

        internal virtual void Load()
        {
            string _filePath = System.IO.Path.Combine(ResourceManager.AssetBundlePath, m_name);
            m_assetBundle = AssetBundle.LoadFromFile(_filePath);
            if (m_assetBundle == null)
                m_error = m_name + " LoadFromFile failed.";
        }

        internal virtual void Unload()
        {
            if (m_assetBundle == null)
                return;
            m_assetBundle.Unload(true);
            m_assetBundle = null;
        }

    }
}

