using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

namespace NavMeshEx
{
[CustomEditor(typeof(NavMeshObject))]
public class NavMeshObjectEditor : Editor
{
    private NavMeshObject navMeshObject
    {
        get
        {
            return target as NavMeshObject;
        }
    }

    private Tool last;

    void OnEnable()
    {
        Undo.undoRedoPerformed += undoRedoPerformed;
    }

    void OnDisable()
    {
        Undo.undoRedoPerformed -= undoRedoPerformed;
    }

    void undoRedoPerformed()
    {
        navMeshObject.UpdateMesh();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Current Vertices: " + navMeshObject.vertices.Count);

        EditorGUILayout.LabelField("Current Triangles: " + navMeshObject.triangles.Count / 3);


        if (navMeshObject.editable)
            GUI.color = Color.yellow;

        if (navMeshObject.editable && GUILayout.Button("Edit Mode: On") || !navMeshObject.editable && GUILayout.Button("Edit Mode: Off"))
        {
            Undo.RecordObject(navMeshObject, "editable");

            navMeshObject.editable = !navMeshObject.editable;

            if (navMeshObject.editable)
            {
                last = Tools.current;
                Tools.current = Tool.None;
            }
            else
                Tools.current = last;
        }


        GUI.color = Color.white;

        if (GUILayout.Button("Bake NavMesh"))
        {
            GameObjectUtility.SetStaticEditorFlags(navMeshObject.gameObject, StaticEditorFlags.NavigationStatic);
            UnityEditor.AI.NavMeshBuilder.BuildNavMesh();
        }

        if (GUILayout.Button("Save NavMeshObject Mesh"))
        {
            string path = "Assets/NavMeshEx/Assets/";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            AssetDatabase.CreateAsset(Instantiate(navMeshObject.mesh), path + navMeshObject.gameObject.name + ".asset");
            AssetDatabase.SaveAssets();
        }
    }

    public void OnSceneGUI()
    {
        List<Vector3> points = navMeshObject.TransformPoints();

        if (navMeshObject.editable)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));//编辑时独占模式

            Event e = Event.current;

            List<int> selecteds = navMeshObject.selecteds;

            for (int i = 0; i < selecteds.Count; i++)
            {
                int selected = selecteds[i];

                Handles.color = navMeshObject.selectedColor;

                Vector3 position = points[selected];

                Handles.DrawSolidDisc(position, Vector3.up, HandleUtility.GetHandleSize(position) * 0.1f);

                Vector3 delta = Handles.PositionHandle(position, Quaternion.identity) - position;

                GUIUtility.GetControlID(FocusType.Passive);
                if (e.type == EventType.used)
                {
                    Undo.RecordObject(navMeshObject, "PositionHandleAt");

                    navMeshObject.PositionHandleAt(selected, delta);

                    EditorUtility.SetDirty(navMeshObject);
                    break;
                }
            }

            if (e.type == EventType.MouseDown && e.button == 1)
            {
                Undo.RecordObject(navMeshObject, "selecteds");
                selecteds.Clear();
            }

            if (e.type == EventType.MouseDown && e.button == 0)
            {
                int index = FindClosest(points, Event.current.mousePosition);
                if (index != -1)
                {
                    Undo.RecordObject(navMeshObject, "selecteds");

                    if (e.control)
                    {
                        selecteds.Add(index);

                        if (selecteds.Count == 3)
                        {
                            navMeshObject.AddTriangle(new int[] { selecteds[0], selecteds[1], selecteds[2] });
                            EditorUtility.SetDirty(navMeshObject);

                            selecteds.Clear();
                        }
                    }
                    else
                    {
                        selecteds.Clear();
                        selecteds.Add(index);
                    }
                }
                else
                {
                    Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

                    RaycastHit hitInfo;
                    if (Physics.Raycast(worldRay, out hitInfo))
                    {
                        Undo.RecordObject(navMeshObject, "AddPoint");

                        Vector3 point = hitInfo.point;
                        point.y += 0.001f;

                        navMeshObject.AddPoint(point);

                        points.Add(point);

                        if (e.control)
                        {
                            selecteds.Add(points.Count - 1);

                            if (selecteds.Count == 3)
                            {
                                navMeshObject.AddTriangle(new int[] { selecteds[0], selecteds[1], selecteds[2] });
                                EditorUtility.SetDirty(navMeshObject);

                                selecteds.Clear();
                            }
                        }
                        else
                        {
                            if (points.Count >= 3)
                            {
                                switch (selecteds.Count)
                                {
                                case 0:
                                    navMeshObject.AddTriangle(new int[] { 0, points.Count - 2, points.Count - 1 });
                                    break;

                                case 1:
                                    if (selecteds[0] == 0 || selecteds[0] == points.Count - 2)
                                        navMeshObject.AddTriangle(new int[] { 0, points.Count - 2, points.Count - 1 });
                                    else
                                        navMeshObject.AddTriangle(new int[] { selecteds[0], points.Count - 2, points.Count - 1 });
                                    break;

                                case 2:
                                    navMeshObject.AddTriangle(new int[] { selecteds[0], selecteds[1], points.Count - 1 });
                                    break;
                                }

                                selecteds.Clear();
                                selecteds.Add(points.Count - 1);
                            }
                        }

                        EditorUtility.SetDirty(navMeshObject);
                    }
                }
            }

            if (selecteds.Count == 1)
            {
                int selected = selecteds[0];

                if (e.keyCode == KeyCode.Backspace)
                {
                    Undo.RecordObject(navMeshObject, "RemovePointAt");

                    navMeshObject.RemovePointAt(selected);
                    points.RemoveAt(selected);

                    selecteds.Clear();

                    EditorUtility.SetDirty(navMeshObject);
                }
            }
        }

        Handles.color = navMeshObject.outlineColor;

        List<int> triangles = navMeshObject.triangles;
        for (int i = 0; i < triangles.Count / 3; i++)
            Handles.DrawPolyLine(points[triangles[i * 3]], points[triangles[i * 3 + 1]], points[triangles[i * 3 + 2]]);

        Handles.color = navMeshObject.normalColor;

        for (int i = 0; i < points.Count; i++)
            Handles.CircleCap(0, points[i], Quaternion.LookRotation(Vector3.up), HandleUtility.GetHandleSize(points[i]) * 0.1f);

        HandleUtility.Repaint();
    }

    private int FindClosest(List<Vector3> points, Vector2 mousePosition)
    {
        List<int> closest = new List<int>();

        for (int i = 0; i < points.Count; i++)
        {
            Vector2 position = HandleUtility.WorldToGUIPoint(points[i]);
            if (Vector2.Distance(position, mousePosition) < 10)//因为之前绘制时大小为HandleUtility.GetHandleSize(points[i]) * 0.1f
                closest.Add(i);
        }

        if (closest.Count == 0)
            return -1;
        else if (closest.Count == 1)
            return closest[0];
        else
        {
            //there are more than a few vertices near the mouse position,
            //here only the closest vertex to the camera should matter
            Vector3 cameraPosition = Camera.current.transform.position;
            float nearDist = float.MaxValue;
            int near = -1;

            //loop over all vertices and get the one near to the camera
            for (int i = 0; i < closest.Count; i++)
            {
                float dist = Vector3.Distance(points[closest[i]], cameraPosition);
                if (dist < nearDist)
                {
                    nearDist = dist;
                    near = closest[i];
                }
            }

            return near;
        }
    }
}
}