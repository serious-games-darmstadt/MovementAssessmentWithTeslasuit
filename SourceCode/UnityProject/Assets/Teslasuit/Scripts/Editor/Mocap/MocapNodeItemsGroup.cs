
using System;
using System.Collections.Generic;
using System.Linq;
using TeslasuitAPI.Utils;
using UnityEditor;

namespace TeslasuitAPI
{
    public class MocapNodeItemsGroup
    {
        private const string ShowContentKeyPrefix = "TSMN_Group_ShowContent_";

        private static readonly string GroupIsNotFullWarningText = "This bones group is not full. Please configure avatar (in ModelImportSettings) by these bones: ";

        private MocapBone _bonesMask;

        private IEnumerable<MocapNodeItem> mocapNodeItems;

        public event Action<MocapNodeItem> ItemSelected = delegate { };

        public bool IsFullyConfigured { get; private set; }

        public string Name { get; set; }
        
        public bool Enabled { get { return _enabled; } set { _enabled = value; } }
        private bool _enabled = false;

        private bool ShowContent { get { return _showContent; }  set { _showContent = value; EditorPrefs.SetBool(ShowContentKeyPrefix + Name, _showContent); } }
        private bool _showContent = false;

        public MocapNodeItemsGroup(MocapBone mask, MocapNodeItem[] mocapNodeItems, string name)
        {
            
            this.Name = name;

            this.ShowContent = EditorPrefs.GetBool(ShowContentKeyPrefix + Name, false);

            this._bonesMask = mask;
            this.mocapNodeItems = mocapNodeItems.Where((item) => _bonesMask.Contains(item.MocapBoneIndex));

            foreach(var item in this.mocapNodeItems)
            {
                item.Selected += ItemSelected;
                _enabled |= item.Enabled;
            }
                

            CheckForAvailability();
            Initialize();
        }

        private void CheckForAvailability()
        {
            IsFullyConfigured = true;
            
            foreach (var nodeItem in _bonesMask.GetEnumerator())
            {
                if (mocapNodeItems.FirstOrDefault((item) => item.MocapBoneIndex == nodeItem) == null)
                    IsFullyConfigured = false;
            }
        }

        private string NotAvailableBones()
        {
            string bonesList = "\n";

            foreach (var nodeItem in _bonesMask.GetEnumerator())
            {
                if (mocapNodeItems.FirstOrDefault((item) => item.MocapBoneIndex == nodeItem) == null)
                    bonesList += nodeItem.ToString() + "\n";
            }
            return bonesList;
        }

        private void Initialize()
        {
            foreach(var nodeItem in mocapNodeItems)
            {
                _enabled |= nodeItem.Enabled;
            }
        }

        public bool OnSceneGUI()
        {
            bool updated = false;
            foreach (var nodeItem in mocapNodeItems)
                updated |= nodeItem.OnSceneGUI();

            return updated;
        }

        public bool OnInspectorGUI()
        {
            bool updated = false;
 
            bool enabled_changed = _enabled;
            updated |= EditorGUIExtensions.BeginSettingsBox(Name, ref _enabled, ref _showContent);
            ShowContent = _showContent;
            enabled_changed = enabled_changed != _enabled;

            if (ShowContent && !IsFullyConfigured)
            {
                EditorGUIExtensions.WarningBox(GroupIsNotFullWarningText + NotAvailableBones());
            }
            
            foreach (var nodeItem in mocapNodeItems)
            {
                if (enabled_changed)
                    nodeItem.Enabled = _enabled;
                if (ShowContent)
                     updated |= nodeItem.OnInspectorGUI();
            }

            EditorGUIExtensions.EndSettingsBox();
            return updated;
        }

        public void UnselectExcept(MocapNodeItem except)
        {
            foreach (var nodeItem in mocapNodeItems)
            {
                if(nodeItem != except)
                {
                    nodeItem.SetSelected(false);
                }
            }
        }

    } 
}
