using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class LayoutController : EditorWindow
{
    [MenuItem("Window/Custom/project window OnOff %SPACE")]
    public static void ToggleProjectWindow()
    {
        var projectWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.ProjectBrowser");
        
        if (projectWindowType != null)
        {
            EditorWindow window;

            if (Resources.FindObjectsOfTypeAll(projectWindowType).Length <= 0)
            {
                //Debug.Log("Open");
                //ProjectBrowser 없으면 윈도우 생성 후 반환
                window = EditorWindow.GetWindow(projectWindowType);
                //ShowWindow();
            }
            else
            {
                //Debug.Log("Close");
                //ProjectBrowser 있으면 윈도우 반환
                window = EditorWindow.GetWindow(projectWindowType);
                window.Close();
            }
            

            //if (window != null)
            //{
            //    // 창이 이미 열려있고 포커스 되어 있다면, 창을 닫습니다.
            //    if (window.hasFocus)
            //    {
            //        Debug.Log("Close");
            //        window.Close();
            //    }
            //    else
            //    {
            //        Debug.Log("Open");
            //        window.Show();
            //    }

            //    //window.Show();
            //}
        }
        else
        {
            Debug.LogError("Unable to find ProjectBrowser type.");
        }
    }


    string selectedPath = "";
    string assetType = "";
    string selectedObjectPath = "";

    [MenuItem("Tools/Asset Type Checker")]
    private static void ShowWindow()
    {
        var window = GetWindow<LayoutController>();
        window.titleContent = new GUIContent("Asset Type Checker");
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Selected Object Path:", EditorStyles.boldLabel);
        GUILayout.TextField(selectedPath);

        GUILayout.Label("Asset Type:", EditorStyles.boldLabel);
        GUILayout.TextField(assetType);


        //GUILayout.Label("Assets in Assets/ExampleFolder:", EditorStyles.boldLabel);

        //var guids = AssetDatabase.FindAssets("", new[] { "Assets" });
        //foreach (var guid in guids)
        //{
        //    var path = AssetDatabase.GUIDToAssetPath(guid);
        //    var assetName = System.IO.Path.GetFileName(path);

        //    if (GUILayout.Button(assetName))
        //    {
        //        var asset = AssetDatabase.LoadAssetAtPath<Object>(path);
        //        EditorGUIUtility.PingObject(asset);
        //    }
        //}
    }

    private void OnSelectionChange()
    {
        if (Selection.activeObject != null)
        {
            Debug.Log("선택");
            selectedPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            DetermineAssetType(selectedPath);
            Repaint();
        }
    }

    void DetermineAssetType(string path)
    {
        if (Directory.Exists(path))
        {
            assetType = "Folder";
        }
        else if (File.Exists(path))
        {
            assetType = "File";
        }
        else
        {
            assetType = "Unknown";
        }
    }

}