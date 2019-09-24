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
        private List<Object> m_requires;
        protected Type m_assetType;
        protected string m_name;
        protected string m_path;
        protected StringBuilder m_strBuilder;

        public string Id
        {
            get { return m_path+m_name; }
        }
        public Object m_asset { get; internal set; }

        private bool CheckRequires
        {
            get { return m_requires != null; }
        }

        public void Require(Object obj)
        {
            if (m_requires == null)
                m_requires = new List<Object>();

            m_requires.Add(obj);
            Retain();
        }

        // ReSharper disable once IdentifierTypo
        public void Dequire(Object obj)
        {
            if (m_requires == null)
                return;

            if (m_requires.Remove(obj))
                Release();
        }

        private void UpdateRequires()
        {
            for (var _i = 0; _i < m_requires.Count; _i++)
            {
                var _item = m_requires[_i];
                if (_item != null)
                    continue;
                Release();
                m_requires.RemoveAt(_i);
                _i--;
            }

            if (m_requires.Count == 0)
                m_requires = null;
        }

        internal virtual void Load()
        {
            m_strBuilder.Length = 0;
            m_strBuilder.Append("Assets/");
            m_strBuilder.Append(m_path);
            m_strBuilder.Append("/");
            m_strBuilder.Append(m_name);
            m_strBuilder.Append(".prefab");

            m_asset = AssetDatabase.LoadAssetAtPath(m_strBuilder.ToString(), m_assetType);
        }

        internal virtual void Unload()
        {
            if (m_asset == null)
                return;
            if (!(m_asset is GameObject))
                Resources.UnloadAsset(m_asset);

            m_asset = null;
        }

        public Asset(string path, string name, Type type)
        {
            m_path = path;
            m_name = name;
            m_assetType = type;
            m_strBuilder = new StringBuilder();
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
        protected readonly string m_assetBundleName;
        protected BundleItem m_bundle;

        public BundleAsset(string path, string name, Type type) :base(path, name, type)
        {
            m_assetBundleName = path.Replace('/', '_');
        }

        internal override void Load()
        {
            m_bundle = GameManager.ResManager.LoadAssetBundleSync(m_assetBundleName);
            m_asset = m_bundle.Bundle.LoadAsset(m_name, m_assetType);
        }

        internal override void Unload()
        {
            if (m_bundle != null)
            {
                m_bundle.Release();
                m_bundle = null;
            }

            m_asset = null;
        }
    }
}


