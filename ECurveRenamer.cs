using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;
using System.Linq;

public class ECurveRenamer : EditorWindow
{
    string _oldName = "";
    string  _newName = "";

    bool _all = true;
    bool _setDirty = true;
    bool _scaleToggle = true;
    bool _rotationToggle = true;
    bool _positionToggle = true;

    readonly bool[] _scaleAxes = new bool[] { true, true, true };
    readonly bool[] _rotationAxes = new bool[] { true, true, true };
    readonly bool[] _positionAxes = new bool[] { true, true, true };


    List<AnimationClip> _clipsWithModifiedCurves = new List<AnimationClip>();

    [MenuItem("Escripts/ERenamer Animation Curves")]
    public static void ShowWindow()
    {
        GetWindow<ECurveRenamer>("ECurveRenamer");
    }

    void OnGUI()
    {
        GUILayout.Space(10);

        _oldName = EditorGUILayout.TextField("Old Name", _oldName);
        _newName = EditorGUILayout.TextField("New Name", _newName);

        GUILayout.Space(10);

        _all = EditorGUILayout.Toggle("All", _all);
        
        _setDirty = EditorGUILayout.Toggle("Save Assets DataBase", _setDirty);

        if (!_all)
        {
            GUILayout.Space(10);
            GUILayout.Label("Individual Curves Selection", EditorStyles.boldLabel);

            _scaleToggle = EditorGUILayout.Toggle("Scale", _scaleToggle);
            if (_scaleToggle)
            {
                GUILayout.BeginHorizontal();
                _scaleAxes[0] = EditorGUILayout.ToggleLeft("X", _scaleAxes[0], GUILayout.Width(50));
                _scaleAxes[1] = EditorGUILayout.ToggleLeft("Y", _scaleAxes[1], GUILayout.Width(50));
                _scaleAxes[2] = EditorGUILayout.ToggleLeft("Z", _scaleAxes[2], GUILayout.Width(50));
                GUILayout.EndHorizontal();
            }

            _rotationToggle = EditorGUILayout.Toggle("Rotation", _rotationToggle);
            if (_rotationToggle)
            {
                GUILayout.BeginHorizontal();
                _rotationAxes[0] = EditorGUILayout.ToggleLeft("X", _rotationAxes[0], GUILayout.Width(50));
                _rotationAxes[1] = EditorGUILayout.ToggleLeft("Y", _rotationAxes[1], GUILayout.Width(50));
                _rotationAxes[2] = EditorGUILayout.ToggleLeft("Z", _rotationAxes[2], GUILayout.Width(50));
                GUILayout.EndHorizontal();
            }

            _positionToggle = EditorGUILayout.Toggle("Position", _positionToggle);
            if (_positionToggle)
            {
                GUILayout.BeginHorizontal();
                _positionAxes[0] = EditorGUILayout.ToggleLeft("X", _positionAxes[0], GUILayout.Width(50));
                _positionAxes[1] = EditorGUILayout.ToggleLeft("Y", _positionAxes[1], GUILayout.Width(50));
                _positionAxes[2] = EditorGUILayout.ToggleLeft("Z", _positionAxes[2], GUILayout.Width(50));
                GUILayout.EndHorizontal();
            }    
            
            GUILayout.Space(10);
        }
        
        if (GUILayout.Button("Rename Curves"))
        {
            RenameSelectedCurves();
        }

        if (GUILayout.Button("Undo Rename"))
        {
            UndoRename();
        }
    }

    void RenameSelectedCurves()
    {
        var clipsToModify = new List<AnimationClip>();
        foreach (Object obj in Selection.objects)
        {
            if (obj is AnimationClip clip)
            {
                var curveBindings = AnimationUtility.GetCurveBindings(clip).Where(IsCurveSelected).ToArray();
                var curvesToUpdate = new List<EditorCurveBinding>();
                var newCurves = new List<AnimationCurve>();

                foreach (EditorCurveBinding curveBinding in curveBindings)
                {
                    if (curveBinding.path.EndsWith(_oldName) || curveBinding.path.EndsWith("/" + _oldName))
                    {
                        AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, curveBinding);
                        string newPath = curveBinding.path.Replace(_oldName, _newName);
                        EditorCurveBinding newCurveBinding = new EditorCurveBinding
                        {
                            path = newPath,
                            propertyName = curveBinding.propertyName,
                            type = curveBinding.type
                        };
                        curvesToUpdate.Add(curveBinding);
                        newCurves.Add(curve);
                    }
                }

                if (curvesToUpdate.Count > 0)
                {
                    Undo.RecordObject(clip, "Rename Animation Curves");
                    for (int i = 0; i < curvesToUpdate.Count; i++)
                    {
                        AnimationUtility.SetEditorCurve(clip, curvesToUpdate[i], null);
                        AnimationUtility.SetEditorCurve(clip, new EditorCurveBinding
                        {
                            path = curvesToUpdate[i].path.Replace(_oldName, _newName),
                            propertyName = curvesToUpdate[i].propertyName,
                            type = curvesToUpdate[i].type
                        }, newCurves[i]);
                    }

                    clipsToModify.Add(clip);
                }
            }
        }

        if (_setDirty && clipsToModify.Count > 0)
        {
            foreach (var modifiedClip in clipsToModify)
            {
                EditorUtility.SetDirty(modifiedClip);
            }

            AssetDatabase.SaveAssets();
        }

        _clipsWithModifiedCurves = clipsToModify;
    }

    
    bool IsCurveSelected(EditorCurveBinding binding)
    {
        if (_all) return true;
        
        if (binding.type != typeof(Transform)) return false;

        bool isSelected = false;
        string property = binding.propertyName.ToLower();

        if (_scaleToggle && property.Contains("scale"))
        {
            isSelected |= _scaleAxes[0] && property.Contains(".x");
            isSelected |= _scaleAxes[1] && property.Contains(".y");
            isSelected |= _scaleAxes[2] && property.Contains(".z");
        }
        if (_rotationToggle && property.Contains("rotation"))
        {
            isSelected |= _rotationAxes[0] && property.Contains(".x");
            isSelected |= _rotationAxes[1] && property.Contains(".y");
            isSelected |= _rotationAxes[2] && property.Contains(".z");
        }
        if (_positionToggle && property.Contains("position"))
        {
            isSelected |= _positionAxes[0] && property.Contains(".x");
            isSelected |= _positionAxes[1] && property.Contains(".y");
            isSelected |= _positionAxes[2] && property.Contains(".z");
        }

        return isSelected;
    }

    void UndoRename()
    {
        foreach (AnimationClip clip in _clipsWithModifiedCurves)
        {
            Undo.PerformUndo();
        }
        _clipsWithModifiedCurves.Clear();
    }
}
