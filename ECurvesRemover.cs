using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Animations;

public class ECurvesRemover : EditorWindow
{
    string _curvePath = "";
    bool _includeChildren = false;
    bool _deletePositionCurves = true;
    bool _deleteRotationCurves = true;
    bool _deleteScaleCurves = true;
    bool _cutCurveAndChangeHierarchy = false;
    bool _saveAssets = true;
    
    [MenuItem("Escripts/ECurves Remover")]
    public static void ShowWindow()
    {
        GetWindow<ECurvesRemover>("ECurvesRemover");
    }

    void OnGUI()
    {
        GUILayout.Label("Curve Settings", EditorStyles.boldLabel);
        GUILayout.Space(2);
        GUILayout.Label("Specify the Curve Path for deletion or hierarchy change.", EditorStyles.miniLabel);
        GUILayout.Space(10);
        _curvePath = EditorGUILayout.TextField("Curve Path", _curvePath);
        GUILayout.Space(10);

        _includeChildren = EditorGUILayout.Toggle("Include Children", _includeChildren);
        _cutCurveAndChangeHierarchy = EditorGUILayout.Toggle("Cut Curve/Change Hierarchy", _cutCurveAndChangeHierarchy);
        
        GUILayout.Space(10);
        _saveAssets = EditorGUILayout.Toggle("Save Assets", _saveAssets);
        GUILayout.Space(10);

        if (_includeChildren && _cutCurveAndChangeHierarchy)
        {
            _cutCurveAndChangeHierarchy = false;
        }

        GUILayout.Space(10);

        _deletePositionCurves = EditorGUILayout.Toggle("Delete Position Curves", _deletePositionCurves);
        _deleteRotationCurves = EditorGUILayout.Toggle("Delete Rotation Curves", _deleteRotationCurves);
        _deleteScaleCurves = EditorGUILayout.Toggle("Delete Scale Curves", _deleteScaleCurves);

        if (GUILayout.Button("Delete Curves"))
        {
            DeleteCurveFromSelectedClips(_curvePath, _includeChildren);
        }
    }

    void DeleteCurveFromSelectedClips(string path, bool includeChildren)
    {
        foreach (Object obj in Selection.objects)
        {
            if (obj is AnimationClip)
            {
                AnimationClip clip = (AnimationClip)obj;
                var bindings = AnimationUtility.GetCurveBindings(clip);
                var pathsToDelete = new List<EditorCurveBinding>();
                var curvesToRebind = new Dictionary<EditorCurveBinding, AnimationCurve>();

                foreach (var binding in bindings)
                {
                    bool isChild = IsChildOfPath(binding.path, path);
                    bool shouldModifyHierarchy = _cutCurveAndChangeHierarchy && (binding.path == path || isChild);
                    bool shouldDelete = binding.path == path || (includeChildren && isChild);

                    if (shouldModifyHierarchy)
                    {
                        var modifiedPath = ModifyPath(binding.path, path);
                        var curve = AnimationUtility.GetEditorCurve(clip, binding);
                        var newBinding = new EditorCurveBinding { path = modifiedPath, propertyName = binding.propertyName, type = binding.type };
                        curvesToRebind[newBinding] = curve;
                    }
                    else if (shouldDelete && ShouldDeleteCurve(binding))
                    {
                        pathsToDelete.Add(binding);
                    }
                }

                ProcessCurves(clip, pathsToDelete, curvesToRebind);
            }
        }
    }

    bool ShouldDeleteCurve(EditorCurveBinding binding)
    {
        return (_deletePositionCurves && binding.propertyName.Contains("m_LocalPosition")) ||
               (_deleteRotationCurves && binding.propertyName.Contains("m_LocalRotation")) ||
               (_deleteScaleCurves && binding.propertyName.Contains("m_LocalScale"));
    }

    void ProcessCurves(AnimationClip clip, List<EditorCurveBinding> pathsToDelete, Dictionary<EditorCurveBinding, AnimationCurve> curvesToRebind)
    {
        bool changesMade = false;

        foreach (var binding in pathsToDelete)
        {
            AnimationUtility.SetEditorCurve(clip, binding, null);
            changesMade = true;
        }

        foreach (var kvp in curvesToRebind)
        {
            AnimationUtility.SetEditorCurve(clip, kvp.Key, kvp.Value);
            changesMade = true;
        }

        if (changesMade && _saveAssets)
        {
            AssetDatabase.SaveAssets();
            Debug.Log("Assets saved.");
        }
    }

    string ModifyPath(string originalPath, string segmentToRemove)
    {
        if (!originalPath.Contains(segmentToRemove)) return originalPath;

        var pattern = segmentToRemove + (segmentToRemove.EndsWith("/") ? "" : "/");
        var modifiedPath = originalPath.Replace(pattern, "");
        return modifiedPath;
    }
    
    bool IsChildOfPath(string childPath, string parentPath)
    {
        return childPath.StartsWith(parentPath + "/", System.StringComparison.Ordinal);
    }
}