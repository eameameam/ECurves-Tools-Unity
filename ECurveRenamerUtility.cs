using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class ECurveRenamerUtility
{
    public static void RenameCurves(List<AnimationClip> clipsToRename, string oldName, string newName, bool setDirty, bool all, bool[] scaleAxes, bool[] rotationAxes, bool[] positionAxes)
    {
        foreach (AnimationClip clip in clipsToRename)
        {
            if (clip == null) continue;

            var curveBindings = AnimationUtility.GetCurveBindings(clip);
            bool clipModified = false;

            foreach (var binding in curveBindings)
            {
                if (!IsCurveSelected(binding, all, scaleAxes, rotationAxes, positionAxes)) continue;
                if (!binding.path.Contains(oldName)) continue;

                AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);
                string newPath = binding.path.Replace(oldName, newName ?? "");
                EditorCurveBinding newBinding = binding;
                newBinding.path = newPath;

                AnimationUtility.SetEditorCurve(clip, binding, null);
                AnimationUtility.SetEditorCurve(clip, newBinding, curve);
                clipModified = true;
            }

            if (clipModified && setDirty)
            {
                EditorUtility.SetDirty(clip);
            }
        }

        if (setDirty)
        {
            AssetDatabase.SaveAssets();
        }
    }

    private static bool IsCurveSelected(EditorCurveBinding binding, bool all, bool[] scaleAxes, bool[] rotationAxes, bool[] positionAxes)
    {
        if (all) return true;
        if (binding.type != typeof(Transform)) return false;

        string property = binding.propertyName.ToLower();
        return (property.Contains("scale") && MatchAxes(scaleAxes, property)) ||
               (property.Contains("rotation") && MatchAxes(rotationAxes, property)) ||
               (property.Contains("position") && MatchAxes(positionAxes, property));
    }

    private static bool MatchAxes(bool[] axes, string property)
    {
        return (axes[0] && property.Contains(".x")) || (axes[1] && property.Contains(".y")) || (axes[2] && property.Contains(".z"));
    }
}
