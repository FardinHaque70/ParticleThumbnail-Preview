using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace ParticleThumbnailAndPreview.Editor
{
    internal static class ParticlePreviewTargetGate
    {
        public static bool IsSupportedTarget(UnityObject[] targets)
        {
            if (targets == null || targets.Length != 1)
                return false;

            return IsSupportedTarget(targets[0] as GameObject);
        }

        public static bool IsSupportedTarget(GameObject prefab)
        {
            if (prefab == null)
                return false;

            if (!EditorUtility.IsPersistent(prefab))
                return false;

            if (!PrefabUtility.IsPartOfPrefabAsset(prefab))
                return false;

            return prefab.GetComponent<ParticleSystem>() != null;
        }

        public static bool ShouldSuppressCompetingPreview(UnityObject[] targets)
        {
            return ShouldSuppressCompetingPreview(targets, ParticlePreviewSettings.Active);
        }

        internal static bool ShouldSuppressCompetingPreview(UnityObject[] targets, bool previewActive)
        {
            if (!previewActive)
                return false;

            return IsSupportedTarget(targets) || IsInspectorSourceBackedParticlePrefab(targets);
        }

        public static UnityObject[] TryGetObjectPreviewTargets(object objectPreviewInstance)
        {
            if (objectPreviewInstance == null)
                return null;

            try
            {
                Type type = objectPreviewInstance.GetType();
                while (type != null)
                {
                    FieldInfo targetsField = type.GetField(
                        "m_Targets",
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (targetsField != null)
                        return targetsField.GetValue(objectPreviewInstance) as UnityObject[];

                    type = type.BaseType;
                }
            }
            catch
            {
                // Keep gate safe: return null when Unity internals differ.
            }

            return null;
        }

        private static bool IsInspectorSourceBackedParticlePrefab(UnityObject[] targets)
        {
            if (targets == null || targets.Length != 1)
                return false;

            if (TryResolveSourceParticlePrefab(targets[0], out _))
                return true;

            if (Selection.activeObject is GameObject selectedPrefab && IsSupportedTarget(selectedPrefab))
                return true;

            return false;
        }

        private static bool TryResolveSourceParticlePrefab(UnityObject target, out GameObject prefabAsset)
        {
            prefabAsset = null;
            if (target == null)
                return false;

            GameObject go = target as GameObject;
            if (go == null && target is Component component)
                go = component.gameObject;
            if (go == null)
                return false;

            GameObject source = PrefabUtility.GetCorrespondingObjectFromSource(go);
            if (source == null || !IsSupportedTarget(source))
                return false;

            prefabAsset = source;
            return true;
        }
    }
}
