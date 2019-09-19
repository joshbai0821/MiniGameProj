using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using Object = UnityEngine.Object;

namespace MiniProj
{
    public class Asset : Reference
    {
        private List<Object> m_Requires;
        protected Type m_AssetType;
        protected string m_Name;
        protected string m_Path;
        protected StringBuilder mStrBuilder;

        public string Id
        {
            get { return m_Path+m_Name; }
        }
        public Object m_Asset { get; internal set; }

        private bool CheckRequires
        {
            get { return m_Requires != null; }
        }

        public void Require(Object obj)
        {
            if (m_Requires == null)
                m_Requires = new List<Object>();

            m_Requires.Add(obj);
            Retain();
        }

        // ReSharper disable once IdentifierTypo
        public void Dequire(Object obj)
        {
            if (m_Requires == null)
                return;

            if (m_Requires.Remove(obj))
                Release();
        }

        private void UpdateRequires()
        {
            for (var i = 0; i < m_Requires.Count; i++)
            {
                var item = m_Requires[i];
                if (item != null)
                    continue;
                Release();
                m_Requires.RemoveAt(i);
                i--;
            }

            if (m_Requires.Count == 0)
                m_Requires = null;
        }

        internal virtual void Load()
        {
            mStrBuilder.Length = 0;
            mStrBuilder.Append("Assets/");
            mStrBuilder.Append(m_Path);
            mStrBuilder.Append("/");
            mStrBuilder.Append(m_Name);

            m_Asset = AssetDatabase.LoadAssetAtPath(mStrBuilder.ToString(), m_AssetType);
        }

        internal virtual void Unload()
        {
            if (m_Asset == null)
                return;
            if (!(m_Asset is GameObject))
                Resources.UnloadAsset(m_Asset);

            m_Asset = null;
        }

        public Asset(string path, string name, Type type)
        {
            m_Path = path;
            m_Name = name;
            m_AssetType = type;
            mStrBuilder = new StringBuilder();
        }

        internal bool Update()
        {
            if(CheckRequires)
            {
                UpdateRequires();
            }
            return true;
        }
    }

    public class BundleAsset : Asset
    {
        protected readonly string assetBundleName;
        protected BundleItem m_Bundle;

        public BundleAsset(string path, string name, Type type) :base(path, name, type)
        {
            assetBundleName = path.Replace('/', '_');
        }

        internal override void Load()
        {
            m_Bundle = GameManager.ResManager.LoadAssetBundleSync(assetBundleName);
            m_Asset = m_Bundle.Bundle.LoadAsset(m_Name, m_AssetType);
        }

        internal override void Unload()
        {
            if (m_Bundle != null)
            {
                m_Bundle.Release();
                m_Bundle = null;
            }

            m_Asset = null;
        }
    }
}


