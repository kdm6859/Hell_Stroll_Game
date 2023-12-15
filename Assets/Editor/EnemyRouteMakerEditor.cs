//using System.Collections;
//using System.Collections.Generic;
//using System.ComponentModel;
//using Unity.Burst.CompilerServices;
//using Unity.VisualScripting;
using UnityEditor;
//using UnityEditor.PackageManager.UI;
using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.UIElements;
//using static System.Runtime.CompilerServices.RuntimeHelpers;

[CustomEditor(typeof(EnemyRouteMaker))]
public class EnemyRouteMakerEditor : Editor
{
    //public LayerMask mask;

    //public List<GameObject> points { get; set; } = new List<GameObject>();

    private void OnSceneGUI()
    {
        //Tools.current = Tool.None;
        var component = target as EnemyRouteMaker;
        var transform = component.transform;

        Event currentEvent = Event.current;

        Tools.current = Tool.Move;

        EditorGUI.BeginChangeCheck();

        



        //Ray ray;
        //RaycastHit hit;

        //if (currentEvent.type != EventType.MouseDown || currentEvent.button != 0)
        //{
        //    Selection.activeObject = component;
        //    return;

        //}
        //Debug.Log("a??");

        //Enemy 이동영역 추가(A키)
        if (currentEvent.type == EventType.KeyDown && currentEvent.keyCode == KeyCode.A)
        {
            component.EnemyMoveArea.Add(new MovePoints());
            Debug.Log("enemyMoveArea 추가\nenemyMoveArea : " + component.EnemyMoveArea.Count);

            Undo.RegisterCompleteObjectUndo(component.gameObject, "undo");
        }
        //Enemy 이동영역 선택 변경(C키)
        else if (currentEvent.type == EventType.KeyDown && currentEvent.keyCode == KeyCode.C)
        {
            component.EnemyMoveAreaIndex++;
            if (component.EnemyMoveAreaIndex >= component.EnemyMoveArea.Count)
            {
                component.EnemyMoveAreaIndex = 0;
            }
            Debug.Log("인덱스 변경\n인덱스 : " + component.EnemyMoveAreaIndex);

            Undo.RegisterCompleteObjectUndo(component.gameObject, "undo");
            //currentEvent.Use();
        }
        //Enemy 이동영역 포인트 추가(ctrl + left click)
        else if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0 && currentEvent.control)//&&currentEvent.clickCount > 0 && currentEvent.isMouse)
        {
            //에디터에서 마우스 포지션을 구하는 법
            //
            //var mousePosition = currentEvent.mousePosition * EditorGUIUtility.pixelsPerPoint;
            //mousePosition.y = Camera.current.pixelHeight - mousePosition.y;
            //Ray ray = Camera.current.ScreenPointToRay(mousePosition);
            // 또는 
            Ray ray = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition);
            //Physics.Raycast(ray, out hit, Mathf.Infinity, mask);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log("클릭 : " + hit.point);
                //GameObject point = Instantiate(component.pointPrefab, hit.point, Quaternion.identity);
                //point.transform.parent = transform;

                //component.points.Add(point);
                if (component.EnemyMoveArea.Count == 0)
                {

                    component.EnemyMoveArea.Add(new MovePoints());
                    component.EnemyMoveAreaIndex = 0;
                }
                component.EnemyMoveArea[component.EnemyMoveAreaIndex].PointPositions.Add(hit.point);
            }

            Undo.RegisterCompleteObjectUndo(component.gameObject, "undo");
        }
        //Enemy 이동영역 삭제(Shift + del)
        else if (currentEvent.type == EventType.KeyDown && currentEvent.keyCode == KeyCode.Delete && currentEvent.shift)
        {
            if (component.EnemyMoveArea.Count > 0)
            {
                component.EnemyMoveArea.RemoveAt(component.EnemyMoveAreaIndex);
                if (component.EnemyMoveAreaIndex == component.EnemyMoveArea.Count && component.EnemyMoveArea.Count != 0)
                {
                    Debug.Log(component.EnemyMoveAreaIndex + " " + component.EnemyMoveArea.Count);
                    component.EnemyMoveAreaIndex--;
                }
            }
        }

        if (currentEvent.type == EventType.KeyDown)
        {
            //Debug.Log("SetDirty");
            EditorUtility.SetDirty(component);
        }

        //Delete키를 눌렀을 때 오브젝트가 삭제되지 않게 한다.
        if (currentEvent.keyCode == KeyCode.Delete)
            currentEvent.Use();

        //클릭 시 다른 오브젝트가 선택되는 것을 막는다.
        Selection.activeObject = component;

        //
        //자식 오브젝트 찾아서 points.Add(point); 해줘야함
        //

        //현재 선택된 enemyMoveArea의 포인트들의 핸들을 표시
        if (component.EnemyMoveArea.Count > 0)
        {
            for (int i = 0; i < component.EnemyMoveArea[component.EnemyMoveAreaIndex].PointPositions.Count; i++)
            {
                //component.pointPositions[i] = Handles.PositionHandle(component.pointPositions[i], Quaternion.identity);
                component.EnemyMoveArea[component.EnemyMoveAreaIndex].PointPositions[i] = PositionHandle(component.EnemyMoveArea[component.EnemyMoveAreaIndex].PointPositions[i]);
            }

        }

        //Scene view에 메뉴얼 표시
        Handles.BeginGUI();
        var oldbgcolor = GUI.backgroundColor;
        GUI.backgroundColor = Color.cyan;     // 배경 색 지정
        GUI.color = Color.cyan;
        GUIStyle guiBoxStyle = new GUIStyle(GUI.skin.box);
        guiBoxStyle.fontSize = component.manualFontSize;
        guiBoxStyle.alignment = TextAnchor.UpperLeft;
        float guiBoxWidth = 19.1536f;
        float guiBoxHeight = 9.4227f; //텍스트 1줄 : 1.3461
        GUI.Box(new Rect(43, 0, guiBoxWidth * guiBoxStyle.fontSize, guiBoxHeight * guiBoxStyle.fontSize),
            "<Manual>\nEnemyMoveArea Add : A\nEnemyMoveArea Change : C\nEnemyMoveArea Remove : shift + del\nMovePoint Add : ctrl + left click\nArea Count : "
            + component.EnemyMoveArea.Count + "\nCurrent Area Index : " + component.EnemyMoveAreaIndex, guiBoxStyle);
        //GUI.backgroundColor = oldbgcolor;
        Handles.EndGUI();





        //Tools.current = Tool.None;
        //transform.position = PositionHandle(transform);



        //currentEvent.Use();

        //mouse.transform.position = hit.point;

        //Debug.Log(Input.mousePosition + ", " + mouse.transform.position);


        //if (Tools.current == Tool.Move)
        //{
        //    transform.rotation =
        //        Handles.RotationHandle(transform.rotation, transform.position);
        //}
        //else if (Tools.current == Tool.Rotate)
        //{
        //    transform.position =
        //        Handles.PositionHandle(transform.position, transform.rotation);
        //}




    }


    Vector3 snap;
    void OnEnable()
    {
        //SnapSettings 의  수치를 얻어오기
        var snapX = EditorPrefs.GetFloat("MoveSnapX", 1f);
        var snapY = EditorPrefs.GetFloat("MoveSnapY", 1f);
        var snapZ = EditorPrefs.GetFloat("MoveSnapZ", 1f);
        snap = new Vector3(snapX, snapY, snapZ);
    }

    Vector3 PositionHandle(Vector3 position)
    {
        var component = target as EnemyRouteMaker;
        var transform = component.transform;

        //var position = transform.position;
        //var size = 10;
        var size = HandleUtility.GetHandleSize(position) * 0.4f;

        //X축
        Handles.color = Handles.xAxisColor;
        position =
            Handles.Slider(position, transform.right, size, Handles.ArrowHandleCap, snap.x);

        //Y축
        Handles.color = Handles.yAxisColor;
        position =
            Handles.Slider(position, transform.up, size, Handles.ArrowHandleCap, snap.y);

        //Z축
        Handles.color = Handles.zAxisColor;
        position =
            Handles.Slider(position, transform.forward, size, Handles.ArrowHandleCap, snap.z);



        return position;
    }
}
