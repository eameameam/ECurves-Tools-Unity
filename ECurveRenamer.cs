using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

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
    List<AnimationClip> _clipsToRename = new List<AnimationClip>();

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
        
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical();
        
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Animation Clips", EditorStyles.boldLabel);
        if (GUILayout.Button("+", GUILayout.Width(20)))
        {
            _clipsToRename.Add(null);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginVertical();
        for (int i = 0; i < _clipsToRename.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            _clipsToRename[i] = (AnimationClip)EditorGUILayout.ObjectField(_clipsToRename[i], typeof(AnimationClip), false);

            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                _clipsToRename.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
    
        EditorGUILayout.EndVertical();

        GUILayout.Space(10);
        Event evt = Event.current;
        Rect dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.Width(100));
        GUI.Box(dropArea, "Drag\nTo Fill\nThe List of Clips");

        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!dropArea.Contains(evt.mousePosition))
                    break;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    _clipsToRename.Clear();
                    foreach (Object draggedObject in DragAndDrop.objectReferences)
                    {
                        AnimationClip clip = draggedObject as AnimationClip;
                        if (clip != null)
                        {
                            _clipsToRename.Add(clip);
                        }
                    }
                }
                break;
        }
        EditorGUILayout.EndHorizontal();
    }

    void RenameSelectedCurves()
    {
        ECurveRenamerUtility.RenameCurves(_clipsToRename, _oldName, _newName, _setDirty, _all, _scaleAxes, _rotationAxes, _positionAxes);
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
