using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LightMaker))]
public class LightMakerEditor : Editor
{
    private void OnSceneGUI()
    {
        var component = target as LightMaker;
        var transform = component.transform;

        Event currentEvent = Event.current;

        //Tools.current = Tool.Move;

        EditorGUI.BeginChangeCheck();


        //Light그룹 추가(A키)
        if (currentEvent.type == EventType.KeyDown && currentEvent.keyCode == KeyCode.A)
        {
            component.LightGroups.Add(new LightGroup());
            component.LightGroups[component.LightGroups.Count - 1].Group = new GameObject("Group" + (component.LightGroups.Count - 1));
            component.LightGroups[component.LightGroups.Count - 1].Group.transform.parent = transform;
            Debug.Log("LightGroups 추가\nLightGroups : " + component.LightGroups.Count);

            Undo.RegisterCompleteObjectUndo(component.gameObject, "undo");
        }
        //Light그룹 선택 변경(C키)
        else if (currentEvent.type == EventType.KeyDown && currentEvent.keyCode == KeyCode.C)
        {
            component.LightGroupIndex++;
            if (component.LightGroupIndex >= component.LightGroups.Count)
            {
                component.LightGroupIndex = 0;
            }
            Debug.Log("Light그룹 인덱스 변경\n인덱스 : " + component.LightGroupIndex);

            Undo.RegisterCompleteObjectUndo(component.gameObject, "undo");
            //currentEvent.Use();
        }
        //Light타입 변경(V키)
        else if (currentEvent.type == EventType.KeyDown && currentEvent.keyCode == KeyCode.V)
        {
            component.Light_Type_Num++;
            if (component.Light_Type_Num >= 3)
            {
                component.Light_Type_Num = 0;
            }
            Debug.Log("Light타입 인덱스 변경\n인덱스 : " + component.Light_Type_Num);

            Undo.RegisterCompleteObjectUndo(component.gameObject, "undo");
            //currentEvent.Use();
        }
        //Light그룹 Light 추가(ctrl + left click)
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
                Debug.Log("그룹 갯수" + component.LightGroups.Count);
                if (component.LightGroups.Count == 0)
                {
                    Debug.Log("첫 생성");
                    component.LightGroups.Add(new LightGroup());
                    component.LightGroups[component.LightGroups.Count - 1].Group = new GameObject("Group" + (component.LightGroups.Count - 1));
                    component.LightGroups[component.LightGroups.Count - 1].Group.transform.parent = transform;
                    component.LightGroupIndex = 0;
                }
                GameObject newLightObj = Instantiate(component.LightPrefabs[component.Light_Type_Num], hit.point, Quaternion.identity);
                component.LightGroups[component.LightGroupIndex].Lights.Add(newLightObj);
                newLightObj.transform.parent = component.LightGroups[component.LightGroupIndex].Group.transform;

                Light newLight = newLightObj.GetComponent<Light>();
                newLight.type = component.Light_Type;
                newLight.lightmapBakeType = component.Light_Mode;
                newLight.innerSpotAngle = component.Light_InnerSpotAngle;
                newLight.spotAngle = component.Light_SpotAngle;
                newLight.color = component.Light_Color;
                newLight.intensity = component.Light_Intensity;
                newLight.bounceIntensity = component.Light_BounceIntensity;
                newLight.range = component.Light_Range;
                newLight.shadows = component.Light_Shadows;
                newLight.shadowRadius = component.Light_ShadowRadius;
            }

            Undo.RegisterCompleteObjectUndo(component.gameObject, "undo");
        }
        //Light그룹 삭제(Shift + del)
        else if (currentEvent.type == EventType.KeyDown && currentEvent.keyCode == KeyCode.Delete && currentEvent.shift)
        {
            if (component.LightGroups.Count > 0)
            {
                DestroyImmediate(component.LightGroups[component.LightGroupIndex].Group);
                //Destroy(component.LightGroups[component.LightGroupIndex].Group);
                component.LightGroups.RemoveAt(component.LightGroupIndex);
                if (component.LightGroupIndex == component.LightGroups.Count && component.LightGroups.Count != 0)
                {
                    Debug.Log(component.LightGroupIndex + " " + component.LightGroups.Count);
                    component.LightGroupIndex--; 
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



        //현재 선택된 enemyMoveArea의 포인트들의 핸들을 표시
        if (component.LightGroups.Count > 0)
        {
            for (int i = 0; i < component.LightGroups[component.LightGroupIndex].Lights.Count; i++)
            {
                //component.pointPositions[i] = Handles.PositionHandle(component.pointPositions[i], Quaternion.identity);
                component.LightGroups[component.LightGroupIndex].Lights[i].transform.position = PositionHandle(component.LightGroups[component.LightGroupIndex].Lights[i].transform.position);
            }

        }


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
        var component = target as LightMaker;
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
