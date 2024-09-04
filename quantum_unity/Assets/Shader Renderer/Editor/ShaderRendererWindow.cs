using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using Alla.Utils;
using System;

namespace Alla.Rendering
{
    public class ShaderRendererWindow : EditorWindow
    {
        // List to hold shaders that will be processed
        private List<Shader> shaders = new List<Shader>();

        // List to hold materials that will be processed
        private List<Material> materials = new List<Material>();

        // Width of the output texture
        public int textureWidth = 1024;

        // Height of the output texture
        public int textureHeight = 1024;

        // Path where the rendered textures will be saved
        public string outputPath = "Assets";

        // Flags to manage rendering states
        private bool rendered;
        private bool wrongPath;
        private bool dirtyScene;
        private string error;
        // Scroll position for the GUI
        private Vector2 scrollPos;

        // List to store generated textures
        private List<Texture> textureList = new List<Texture>();

        // Create a menu item to show the ShaderRendererWindow
        [MenuItem("Tools/Material and Shader Renderer")]
        public static void ShowWindow()
        {
            // Show the ShaderRendererWindow with a custom title
            GetWindow<ShaderRendererWindow>("Alla : Material and Shader Renderer");
        }

        // Draw the custom editor GUI
        private void OnGUI()
        {
            // Begin scroll view for the GUI elements
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(Screen.width), GUILayout.Height(EditorGUILayout.GetControlRect().y));

            // Define custom styles for buttons and labels
            GUIStyle renderButtonStyle = new GUIStyle(GUI.skin.button)
            {
                normal = { background = MakeTex(2, 2, new Color(0f, 0.7f, 0)), textColor = Color.white },
                fixedHeight = 40,
                fontSize = 22,
                fontStyle = FontStyle.Bold
            };

            GUIStyle errorStyle = new GUIStyle(GUI.skin.label)
            {
                normal = { textColor = Color.red },
                fontSize = 12,
                fontStyle = FontStyle.Bold
            };
            GUIStyle warningStyle = new GUIStyle(GUI.skin.label)
            {
                normal = { textColor = new Color(0.7f,0.5f,0) },
                fontSize = 12,
                fontStyle = FontStyle.Bold
            };
            // Create a custom GUIStyle for the label
            GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 18 // Set the desired font size
            };

            // Use the custom style in the label
            GUILayout.Label("Shader and Material Renderer", headerStyle);
            GUILayout.Label("by Alla", EditorStyles.boldLabel);

            // Display the header for shader and material assignment
            GUILayout.Label("Shader and Material Assignment", EditorStyles.boldLabel);
            GUILayout.Space(10);
            int removeShaderIndex = -1;

            // Display the list of shaders
            GUILayout.Label("Shaders", EditorStyles.label);
            for (int i = 0; i < shaders.Count; i++)
            {
                GUILayout.BeginHorizontal(); // Begin a new horizontal group for each shader
                shaders[i] = (Shader)EditorGUILayout.ObjectField($"Custom Shader {i + 1}", shaders[i], typeof(Shader), false);
                if (GUILayout.Button("Remove", GUILayout.Width(70)))
                {
                    removeShaderIndex = i; // Mark the shader to be removed after the loop
                }
                GUILayout.EndHorizontal(); // End the horizontal group
            }

            // If a shader is marked for removal, remove it after the loop
            if (removeShaderIndex >= 0)
            {
                shaders.RemoveAt(removeShaderIndex);
            }
            // Button to add a new shader to the list
            if (GUILayout.Button("Add new Shader"))
            {
                shaders.Add(null); // Add a null shader as a placeholder
            }

            // Variable to store the index of the material to be removed
            int removeMaterialIndex = -1;

            // Display the list of materials
            GUILayout.Label("Materials", EditorStyles.label);
            for (int i = 0; i < materials.Count; i++)
            {
                GUILayout.BeginHorizontal(); // Begin a new horizontal group for each material
                materials[i] = (Material)EditorGUILayout.ObjectField($"Material {i + 1}", materials[i], typeof(Material), false);
                if (GUILayout.Button("Remove", GUILayout.Width(70)))
                {
                    removeMaterialIndex = i; // Mark the material to be removed after the loop
                }
                GUILayout.EndHorizontal(); // End the horizontal group
            }

            // If a material is marked for removal, remove it after the loop
            if (removeMaterialIndex >= 0)
            {
                materials.RemoveAt(removeMaterialIndex);
            }
           

            // Button to add a new material to the list
            if (GUILayout.Button("Add new Material"))
            {
                materials.Add(null); // Add a null material as a placeholder
            }

            GUILayout.Space(10);

            // Display settings for texture output (width and height)
            GUILayout.Label("Output Settings", EditorStyles.boldLabel);
            textureWidth = EditorGUILayout.IntField("Texture Width", textureWidth);
            textureHeight = EditorGUILayout.IntField("Texture Height", textureHeight);

            // Folder selection for the output path
            GUILayout.BeginHorizontal();
            GUILayout.Label("Output Path", EditorStyles.label);
            outputPath = EditorGUILayout.TextField(outputPath);

            // Button to select the folder for saving textures
            if (GUILayout.Button("Select Folder"))
            {
                string selectedPath = EditorUtility.OpenFolderPanel("Select Output Folder", outputPath, "");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    if (selectedPath.StartsWith(Application.dataPath))
                    {
                        outputPath = "Assets" + selectedPath.Substring(Application.dataPath.Length);
                        wrongPath = false;
                    }
                    else
                    {
                        wrongPath = true;
                        Debug.LogWarning("Selected path is outside the project. Please select a folder within the Assets directory.");
                    }
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            // Display warning if the selected path is outside the project directory
            if (wrongPath)
                GUILayout.Label("Selected path is outside the project. Please select a folder within the Assets directory.", errorStyle);

            GUILayout.Space(10);
            if (dirtyScene)
                GUILayout.Label(error, errorStyle);

            GUILayout.Label("Please Select the Empty Scene \"Render Scene\" for fastest render time", warningStyle);
            // Button to apply shaders to materials and render textures
            if (GUILayout.Button("Render Textures", renderButtonStyle) && (shaders.Count > 0 || materials.Count > 0))
            {
                rendered = false;
                if (!System.IO.File.Exists(outputPath))
                {
                    GUILayout.Label("Selected path doesn't exist!!", errorStyle);
                }
               
                // Create a temporary scene for rendering
                Scene originalScene = SceneManager.GetActiveScene();

                Scene tempScene;
                try
                {
                    tempScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
                }
                catch (Exception e)
                {
                    dirtyScene = true;
                    error=e.Message;
                    EditorGUILayout.EndScrollView();
                    return;
                }
                dirtyScene = false;
                // Disable all lights in all scenes to avoid interference
                List<Light> originalLights = new List<Light>();
                foreach (Light light in Light.FindObjectsOfType<Light>())
                {
                    if (!light.enabled) continue;
                    originalLights.Add(light);
                    light.enabled = false;
                }

                // Set up the temporary scene with its own directional light
                GameObject lightObject = new GameObject("Directional Light");
                Light tempLight = lightObject.AddComponent<Light>();
                tempLight.type = LightType.Directional;
                tempLight.color = Color.white;
                tempLight.intensity = 1;
                SceneManager.MoveGameObjectToScene(lightObject, tempScene);

                // Set the temporary scene as the active scene for rendering
                SceneManager.SetActiveScene(tempScene);

                // Clear the list of generated textures before rendering
                textureList.Clear();

                // Render textures for each shader
                foreach (Shader shader in shaders)
                {
                    if (shader)
                    {
                        Texture2D texture = MaterialAndShaderRendererUtil.Renderer(shader, textureWidth, textureHeight);
                        MaterialAndShaderRendererUtil.SaveTexture(texture, shader.name.Replace("/", "").Replace("\\", "") + " Render.png", outputPath);
                        textureList.Add(texture);
                    }
                }

                // Render textures for each material
                foreach (Material material in materials)
                {
                    if (material)
                    {
                        Texture2D texture = MaterialAndShaderRendererUtil.Renderer(material, textureWidth, textureHeight);
                        MaterialAndShaderRendererUtil.SaveTexture(texture, material.name.Replace("/", "").Replace("\\", "") + " Render.png", outputPath);
                        textureList.Add(texture);
                    }
                }

                // Close the temporary scene and revert to the original scene
                EditorSceneManager.CloseScene(tempScene, true);
                SceneManager.SetActiveScene(originalScene);

                // Re-enable the original lights that were disabled
                foreach (Light light in originalLights)
                {
                    light.enabled = true;
                }

                rendered = true;
            }

            // Display a message if rendering is completed
            if (rendered)
            {
                GUILayout.Space(20);
                GUILayout.Label("Shader and Material rendered.");
                GUILayout.Space(20);
                GUILayout.Label("Outputs:");

                int maxTexturesPerRow = Mathf.FloorToInt(Screen.width / 150); // Calculate the maximum number of textures per row
                int currentTexture = 0; // Counter to keep track of the current texture

                GUILayout.BeginVertical(); // Start a vertical layout group

                while (currentTexture < textureList.Count)
                {
                    GUILayout.BeginHorizontal(); // Start a new row

                    for (int i = 0; i < maxTexturesPerRow && currentTexture < textureList.Count; i++)
                    {
                        // Display the texture in the grid
                        GUILayout.Box(textureList[currentTexture], GUILayout.Width(150), GUILayout.Height(150));
                        currentTexture++; // Move to the next texture
                    }

                    GUILayout.EndHorizontal(); // End the current row
                }

                GUILayout.EndVertical(); // End the vertical layout group
            }

            // End scroll view for GUI elements
            EditorGUILayout.EndScrollView();
        }

        // Utility method to create a solid-colored texture for GUI backgrounds
        private Texture2D MakeTex(int width, int height, Color color)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
            {
                pix[i] = color;
            }
            Texture2D tex = new Texture2D(width, height);
            tex.SetPixels(pix);
            tex.Apply();
            return tex;
        }
    }
}