namespace NatureManufacture.RAM
{
    using NatureManufacture.RAM.Editor;
    using UnityEditor;
    using UnityEditor.Overlays;
    using UnityEditor.Toolbars;
    using UnityEngine;

    [Overlay(typeof(SceneView), "NM Spline Tools")]
    [NmIcon("logoRAMNoText")]
    public class SplinesOverlay : ToolbarOverlay
    {
        SplinesOverlay() : base(AddRiverButton.ID, AddLakeButton.ID, AddFenceButton.ID)
        {
        }


        [EditorToolbarElement(ID, typeof(SceneView))]
        private class AddRiverButton : EditorToolbarButton
        {
            public const string ID = "NM Spline Tools/Add River";

            public AddRiverButton()
            {
                this.text = "Add River";
                this.icon = AssetDatabase.LoadAssetAtPath<Texture2D>(NmIconAttribute.GetRelativeIconPath("riverRAM"));
                this.tooltip = "Add river to scene";
                this.clicked += OnClick;
            }


            void OnClick()
            {
                RamSplineEditor.CreateSpline();
            }
        }

        [EditorToolbarElement(ID, typeof(SceneView))]
        private class AddLakeButton : EditorToolbarButton
        {
            public const string ID = "NM Spline Tools/Add Lake";

            public AddLakeButton()
            {
                this.text = "Add Lake";
                this.icon = AssetDatabase.LoadAssetAtPath<Texture2D>(NmIconAttribute.GetRelativeIconPath("lakeRAM"));
                this.tooltip = "Add lake to scene";
                this.clicked += OnClick;
            }

            void OnClick()
            {
                LakePolygonEditor.CreateLakePolygon();
            }
        }


        [EditorToolbarElement(ID, typeof(SceneView))]
        private class AddFenceButton : EditorToolbarButton
        {
            public const string ID = "NM Spline Tools/Add Fence";

            public AddFenceButton()
            {
                this.text = "Add Fence";
                this.icon = AssetDatabase.LoadAssetAtPath<Texture2D>(NmIconAttribute.GetRelativeIconPath("fenceRAM"));
                this.tooltip = "Add fence to scene";
                this.clicked += OnClick;
            }

            void OnClick()
            {
                FenceGeneratorEditor.CreateFenceGenerator();
            }
        }
    }
}