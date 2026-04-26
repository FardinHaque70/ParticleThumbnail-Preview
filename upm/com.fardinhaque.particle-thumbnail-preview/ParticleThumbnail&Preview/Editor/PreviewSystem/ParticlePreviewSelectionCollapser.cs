using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace ParticleThumbnailAndPreview.Editor
{
    [InitializeOnLoad]
    internal static class ParticlePreviewSelectionCollapser
    {
        #region State
        private static string _pendingPath;
        #endregion

        static ParticlePreviewSelectionCollapser()
        {
            Selection.selectionChanged += ScheduleCollapseForSelection;
            ParticlePreviewSettings.SettingsChanged += ScheduleCollapseForSelection;
            ScheduleCollapseForSelection();
        }

        #region Selection Hook
        // Best-effort workaround: collapse prefab component foldouts when our
        // particle preview target is selected so hidden inspector fields do not
        // compete with the preview panel's hover/cursor behavior.
        private static void ScheduleCollapseForSelection()
        {
            EditorApplication.delayCall -= CollapsePendingSelection;
            _pendingPath = null;

            if (!ParticlePreviewSettings.Active)
                return;

            if (!ParticlePreviewTargetGate.IsSupportedTarget(Selection.objects))
                return;

            GameObject selectedPrefab = Selection.activeObject as GameObject;
            if (selectedPrefab == null)
                return;

            string path = AssetDatabase.GetAssetPath(selectedPrefab);
            if (string.IsNullOrEmpty(path))
                return;

            _pendingPath = path;
            EditorApplication.delayCall += CollapsePendingSelection;
        }

        private static void CollapsePendingSelection()
        {
            EditorApplication.delayCall -= CollapsePendingSelection;

            if (string.IsNullOrEmpty(_pendingPath))
                return;

            if (!ParticlePreviewSettings.Active || !ParticlePreviewTargetGate.IsSupportedTarget(Selection.objects))
                return;

            GameObject selectedPrefab = Selection.activeObject as GameObject;
            if (selectedPrefab == null)
                return;

            string selectedPath = AssetDatabase.GetAssetPath(selectedPrefab);
            if (!string.Equals(selectedPath, _pendingPath, System.StringComparison.Ordinal))
                return;

            CollapsePrefabInspectorState(selectedPrefab);
        }
        #endregion

        #region Collapse Helpers
        private static void CollapsePrefabInspectorState(GameObject prefabRoot)
        {
            if (prefabRoot == null)
                return;

            TrySetExpanded(prefabRoot, expanded: false);

            Component[] components = prefabRoot.GetComponentsInChildren<Component>(true);
            for (int i = 0; i < components.Length; i++)
            {
                Component component = components[i];
                if (component == null)
                    continue;

                TrySetExpanded(component, expanded: false);
            }
        }

        private static void TrySetExpanded(Object obj, bool expanded)
        {
            if (obj == null)
                return;

            try
            {
                InternalEditorUtility.SetIsInspectorExpanded(obj, expanded);
            }
            catch
            {
                // Keep this as a best-effort UI-only workaround.
            }
        }
        #endregion
    }
}
