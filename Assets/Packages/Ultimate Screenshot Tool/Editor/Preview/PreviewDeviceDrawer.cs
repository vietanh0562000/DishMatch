using UnityEngine;
using UnityEditor;

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool
{
    [ExecuteAlways]
    [CustomPropertyDrawer(typeof(PreviewDevice))]
    public class PreviewDeviceDrawer : PropertyDrawer
    {
        static bool packageNotFound = false;
        static string devicesFolder = "";

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.FindPropertyRelative("active").boolValue)
                return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("active"), label, true) + EditorGUIUtility.standardVerticalSpacing;

            float totalHeight = 0;
            PreviewDevice.Type type = (PreviewDevice.Type)property.FindPropertyRelative("type").intValue;
            foreach (SerializedProperty childProperty in property.GetChildren())
            {
                // Always skip editor only property.
                if (childProperty.name == "editedObject") continue;
                // Internal variable.
                if (childProperty.name == "deviceFrameFilePath") continue;
                // Always skip as active and name are on the same line.
                if (childProperty.name == "active") continue;
                // Always skip as width and height are on the same line.
                if (childProperty.name == "height") continue;
                // Only 3 lines are required for offsets: label, top/bottom, left/right
                if (childProperty.name == "safeAreaRightOffset") continue;

                // Skip device frame rotation if a screenshot type.
                if (type == PreviewDevice.Type.Screenshot && childProperty.name == "sizingType") continue;

                // Skip device frame and frame rotation if not texture type.
                if (type != PreviewDevice.Type.TextureFrame && childProperty.name == "frame") continue;
                if (type != PreviewDevice.Type.TextureFrame && childProperty.name == "frameRotation") continue;

                // Skip device file path if not device type.
                if (type != PreviewDevice.Type.DeviceFrame && childProperty.name == "deviceFilePath") continue;
                if (type != PreviewDevice.Type.DeviceFrame && childProperty.name == "orientation") continue;

                totalHeight += EditorGUI.GetPropertyHeight(childProperty, label, true) + EditorGUIUtility.standardVerticalSpacing;
            }

            return totalHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float height = 0;
            float labelWidth = 0;
            float fieldWidth = 0;
            float padding = 0;
            float positionX = 0;
            PreviewDevice.Type type = (PreviewDevice.Type)property.FindPropertyRelative("type").intValue;

            EditorGUI.BeginProperty(position, label, property);
            Rect rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            if (label.text.Equals("Reorderable List Preview Device"))
            {
                float activeWidth = position.width * 0.08f;
                rect.width = activeWidth;
                CustomEditorGUI.PropertyField(rect, property.FindPropertyRelative("active"), new GUIContent(""));
                rect.x += activeWidth;
                rect.width = position.width - activeWidth;

                CustomEditorGUI.PropertyField(rect, property.FindPropertyRelative("deviceName"), new GUIContent(""));
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }
            else
            {
                float activeWidth = position.width * 0.30f;
                labelWidth = position.width * 0.2f;
                padding = position.width * 0.02f;
                positionX = position.x;

                rect = new Rect(positionX, position.y + height, labelWidth, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(rect, new GUIContent("Active"));
                positionX += rect.width;

                rect = new Rect(positionX, position.y + height, activeWidth - labelWidth, EditorGUIUtility.singleLineHeight);
                CustomEditorGUI.PropertyField(rect, property.FindPropertyRelative("active"), new GUIContent(""));
                positionX += rect.width;

                positionX += padding;
                rect = new Rect(positionX, position.y + height, labelWidth, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(rect, new GUIContent("Name"));
                positionX += rect.width;

                float remainingWidth = position.width - positionX + position.x;
                rect = new Rect(positionX, position.y + height, remainingWidth, EditorGUIUtility.singleLineHeight);
                CustomEditorGUI.PropertyField(rect, property.FindPropertyRelative("deviceName"), new GUIContent(""));
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            if (!property.FindPropertyRelative("active").boolValue)
            {
                EditorGUI.EndProperty();
                return;
            }

            EditorGUI.BeginChangeCheck();
            rect = new Rect(position.x, position.y + height, position.width, EditorGUIUtility.singleLineHeight);
            CustomEditorGUI.PropertyField(rect, property.FindPropertyRelative("type"));
            height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            if (EditorGUI.EndChangeCheck())
            {
                if(property.FindPropertyRelative("type").intValue == (int)PreviewDevice.Type.Screenshot)
                    property.FindPropertyRelative("sizingType").intValue = (int)PreviewDevice.SizingType.SetScreenshotToSize;
                Reload(property);
            }

            if (type == PreviewDevice.Type.DeviceFrame)
            {
                EditorGUI.BeginChangeCheck();
                rect = new Rect(position.x, position.y + height, position.width, EditorGUIUtility.singleLineHeight);
                CustomEditorGUI.PropertyField(rect, property.FindPropertyRelative("orientation"));
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                if (EditorGUI.EndChangeCheck())
                {
                    ScreenOrientation newOrientation = (ScreenOrientation)property.FindPropertyRelative("orientation").intValue;
                    if ((int)newOrientation == 0 || newOrientation == ScreenOrientation.AutoRotation)
                    {
                        property.FindPropertyRelative("orientation").intValue = (int)ScreenOrientation.Portrait;
                        Debug.LogError(newOrientation + " is not a valid orientation.");
                    }


                    Reload(property);
                }
            }

            if (type != PreviewDevice.Type.Screenshot)
            {
                rect = new Rect(position.x, position.y + height, position.width, EditorGUIUtility.singleLineHeight);
                CustomEditorGUI.PropertyField(rect, property.FindPropertyRelative("sizingType"));
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            if (type == PreviewDevice.Type.TextureFrame)
            {
                string frameRotationTooltip = "Amount to rotate the frame before adding it. Useful as a single texture can be used for all orientations.\n\nFor example, import a portrait texture and apply a 90 degree rotation for landscape images.";
                rect = new Rect(position.x, position.y + height, position.width, EditorGUIUtility.singleLineHeight);
                CustomEditorGUI.PropertyField(rect, property.FindPropertyRelative("frameRotation"), new GUIContent("Frame Rotation", frameRotationTooltip));
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            } else if (type == PreviewDevice.Type.DeviceFrame)
            {
                rect = new Rect(position.x, position.y + height, position.width, EditorGUIUtility.singleLineHeight);

                LoadDevicesFolderIfNecessary();

                EditorGUI.BeginChangeCheck();
                SavePathHelpers.OpenFilePropertyWithBrowseButton(rect, property.FindPropertyRelative("deviceFilePath"), null, "", devicesFolder, "device");
                if (EditorGUI.EndChangeCheck())
                {
                    if (!string.IsNullOrEmpty(property.FindPropertyRelative("deviceFilePath").stringValue))
                        Reload(property);
                }

                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            bool originalGUIEnabled = GUI.enabled;
            GUI.enabled = originalGUIEnabled && type != PreviewDevice.Type.DeviceFrame;

            labelWidth = position.width * 0.15f;
            fieldWidth = position.width * 0.34f;
            padding = position.width * 0.02f;
            positionX = position.x;

            rect = new Rect(positionX, position.y + height, labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(rect, new GUIContent("Width"));
            positionX += rect.width;
            rect = new Rect(positionX, position.y + height, fieldWidth, EditorGUIUtility.singleLineHeight);
            CustomEditorGUI.PropertyField(rect, property.FindPropertyRelative("width"), new GUIContent(""));
            positionX += rect.width;

            positionX += padding;
            rect = new Rect(positionX, position.y + height, labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(rect, new GUIContent("Height"));
            positionX += rect.width;

            rect = new Rect(positionX, position.y + height, fieldWidth, EditorGUIUtility.singleLineHeight);
            CustomEditorGUI.PropertyField(rect, property.FindPropertyRelative("height"), new GUIContent(""));
            positionX += rect.width;

            height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            if (type == PreviewDevice.Type.TextureFrame)
            {
                rect = new Rect(position.x, position.y + height, position.width, EditorGUIUtility.singleLineHeight);
                CustomEditorGUI.PropertyField(rect, property.FindPropertyRelative("frame"));
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            rect = new Rect(position.x, position.y + height, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(rect, new GUIContent("Safe Area Offset"));
            height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            labelWidth = position.width * 0.15f;
            fieldWidth = position.width * 0.34f;
            padding = position.width * 0.02f;
            positionX = position.x;

            rect = new Rect(positionX, position.y + height, labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(rect, new GUIContent("Top"));
            positionX += rect.width;
            rect = new Rect(positionX, position.y + height, fieldWidth, EditorGUIUtility.singleLineHeight);
            CustomEditorGUI.PropertyField(rect, property.FindPropertyRelative("safeAreaTopOffset"), new GUIContent(""));
            positionX += rect.width;

            positionX += padding;
            rect = new Rect(positionX, position.y + height, labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(rect, new GUIContent("Bottom"));
            positionX += rect.width;

            rect = new Rect(positionX, position.y + height, fieldWidth, EditorGUIUtility.singleLineHeight);
            CustomEditorGUI.PropertyField(rect, property.FindPropertyRelative("safeAreaBottomOffset"), new GUIContent(""));
            positionX += rect.width;
            height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            labelWidth = position.width * 0.15f;
            fieldWidth = position.width * 0.34f;
            padding = position.width * 0.02f;
            positionX = position.x;

            rect = new Rect(positionX, position.y + height, labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(rect, new GUIContent("Left"));
            positionX += rect.width;
            rect = new Rect(positionX, position.y + height, fieldWidth, EditorGUIUtility.singleLineHeight);
            CustomEditorGUI.PropertyField(rect, property.FindPropertyRelative("safeAreaLeftOffset"), new GUIContent(""));
            positionX += rect.width;

            positionX += padding;
            rect = new Rect(positionX, position.y + height, labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(rect, new GUIContent("Right"));
            positionX += rect.width;

            rect = new Rect(positionX, position.y + height, fieldWidth, EditorGUIUtility.singleLineHeight);
            CustomEditorGUI.PropertyField(rect, property.FindPropertyRelative("safeAreaRightOffset"), new GUIContent(""));
            positionX += rect.width;

            GUI.enabled = originalGUIEnabled;

            EditorGUI.EndProperty();
        }

        public void Reload(SerializedProperty property)
        {
            property.serializedObject.ApplyModifiedProperties();

            PreviewDevice previewDevice = property.GetTarget<PreviewDevice>();
            previewDevice.ReloadFromDeviceIfValid();

            property.serializedObject.Update(); 
            property.serializedObject.ApplyModifiedProperties();
        }

        public static void LoadDevicesFolderIfNecessary()
        {
            if (!string.IsNullOrEmpty(devicesFolder) || packageNotFound) return;

            string assetsFolder = Application.dataPath;
            string packageCacheDirectoryPath = System.IO.Path.Combine(assetsFolder, "..", "Library", "PackageCache");
            System.IO.DirectoryInfo packageCacheDirectory = new System.IO.DirectoryInfo(packageCacheDirectoryPath);
            System.IO.DirectoryInfo[] potentialPackageDirectories = packageCacheDirectory.GetDirectories("com.unity.device-simulator@*");
            if(potentialPackageDirectories.Length <=0)
            {
                packageNotFound = true;
                Debug.LogError("Device Simulator Package Not Found. Please import Device Simulator from the Package Manager.\n\nDocumentation:\n\nhttps://docs.unity3d.com/Packages/com.unity.device-simulator@3.0/manual/index.html");
                Debug.LogWarning("Restart the Editor once the package is downloaded.");
                Debug.LogWarning("If this error occurs while the package exists, please contact: jacob@tangledrealitystudios.com");
                return;
            } else if(potentialPackageDirectories.Length > 1)
            {
                packageNotFound = true;
                Debug.LogError("Multiple Device Simulator Packages Found. This should not happen please contact: jacob@tangledrealitystudios.com");
                return;
            }
            devicesFolder = System.IO.Path.Combine(potentialPackageDirectories[0].ToString(), "Editor", "SimulatorResources", "DeviceAssets");
        }
    }
}