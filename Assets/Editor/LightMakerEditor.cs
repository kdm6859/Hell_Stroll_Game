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


        //Light�׷� �߰�(AŰ)
        if (currentEvent.type == EventType.KeyDown && currentEvent.keyCode == KeyCode.A)
        {
            component.LightGroups.Add(new LightGroup());
            component.LightGroups[component.LightGroups.Count - 1].Group = new GameObject("Group" + (component.LightGroups.Count - 1));
            component.LightGroups[component.LightGroups.Count - 1].Group.transform.parent = transform;
            Debug.Log("LightGroups �߰�\nLightGroups : " + component.LightGroups.Count);

            Undo.RegisterCompleteObjectUndo(component.gameObject, "undo");
        }
        //Light�׷� ���� ����(CŰ)
        else if (currentEvent.type == EventType.KeyDown && currentEvent.keyCode == KeyCode.C)
        {
            component.LightGroupIndex++;
            if (component.LightGroupIndex >= component.LightGroups.Count)
            {
                component.LightGroupIndex = 0;
            }
            Debug.Log("Light�׷� �ε��� ����\n�ε��� : " + component.LightGroupIndex);

            Undo.RegisterCompleteObjectUndo(component.gameObject, "undo");
            //currentEvent.Use();
        }
        //LightŸ�� ����(VŰ)
        else if (currentEvent.type == EventType.KeyDown && currentEvent.keyCode == KeyCode.V)
        {
            component.Light_Type_Num++;
            if (component.Light_Type_Num >= 3)
            {
                component.Light_Type_Num = 0;
            }
            Debug.Log("LightŸ�� �ε��� ����\n�ε��� : " + component.Light_Type_Num);

            Undo.RegisterCompleteObjectUndo(component.gameObject, "undo");
            //currentEvent.Use();
        }
        //Light�׷� Light �߰�(ctrl + left click)
        else if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0 && currentEvent.control)//&&currentEvent.clickCount > 0 && currentEvent.isMouse)
        {
            //�����Ϳ��� ���콺 �������� ���ϴ� ��
            //
            //var mousePosition = currentEvent.mousePosition * EditorGUIUtility.pixelsPerPoint;
            //mousePosition.y = Camera.current.pixelHeight - mousePosition.y;
            //Ray ray = Camera.current.ScreenPointToRay(mousePosition);
            // �Ǵ� 
            Ray ray = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition);
            //Physics.Raycast(ray, out hit, Mathf.Infinity, mask);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log("Ŭ�� : " + hit.point);
                //GameObject point = Instantiate(component.pointPrefab, hit.point, Quaternion.identity);
                //point.transform.parent = transform;

                //component.points.Add(point);
                Debug.Log("�׷� ����" + component.LightGroups.Count);
                if (component.LightGroups.Count == 0)
                {
                    Debug.Log("ù ����");
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
        //Light�׷� ����(Shift + del)
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

        //DeleteŰ�� ������ �� ������Ʈ�� �������� �ʰ� �Ѵ�.
        if (currentEvent.keyCode == KeyCode.Delete)
            currentEvent.Use();

        //Ŭ�� �� �ٸ� ������Ʈ�� ���õǴ� ���� ���´�.
        Selection.activeObject = component;



        //���� ���õ� enemyMoveArea�� ����Ʈ���� �ڵ��� ǥ��
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
        //SnapSettings ��  ��ġ�� ������
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

        //X��
        Handles.color = Handles.xAxisColor;
        position =
            Handles.Slider(position, transform.right, size, Handles.ArrowHandleCap, snap.x);

        //Y��
        Handles.color = Handles.yAxisColor;
        position =
            Handles.Slider(position, transform.up, size, Handles.ArrowHandleCap, snap.y);

        //Z��
        Handles.color = Handles.zAxisColor;
        position =
            Handles.Slider(position, transform.forward, size, Handles.ArrowHandleCap, snap.z);



        return position;
    }

}
