using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Alla.Utils
{
    public class MaterialAndShaderRendererUtil
    {
        // Method to render a texture from a shader
        public static Texture2D Renderer(Shader shader, int textureWidth, int textureHeight)
        {
            // Create a temporary material using the provided shader
            Material tmpMaterial = new Material(shader);
            return Renderer(tmpMaterial, textureWidth, textureHeight); // Call the material rendering method
        }

        // Method to render a texture from a material
        public static Texture2D Renderer(Material material, int textureWidth, int textureHeight)
        {
            // Create a camera object to render the texture
            GameObject cameraObject = new GameObject("QuadRenderCamera");
            Camera renderCamera = cameraObject.AddComponent<Camera>();
            renderCamera.orthographic = true; // Set the camera to orthographic mode
            renderCamera.backgroundColor = Color.black; // Set the background color to black
            renderCamera.clearFlags = CameraClearFlags.SolidColor; // Clear the background to a solid color

            // Position the camera out of view
            renderCamera.transform.position = new Vector3(-99999, -99999, -99999);
            renderCamera.transform.rotation = Quaternion.identity;

            // Build a quad mesh for rendering
            Mesh quad = BuildQuadMesh();
            Bounds quadBounds = quad.bounds;
            float quadHeight = quadBounds.size.y;
            renderCamera.orthographicSize = quadHeight / 2; // Adjust camera size to fit the quad

            // Set up a render texture to capture the rendering
            RenderTexture renderTexture = new RenderTexture(textureWidth, textureHeight, 24);
            renderCamera.targetTexture = renderTexture;

            // Draw the mesh with the material applied
            Graphics.DrawMesh(quad, renderCamera.transform.position + renderCamera.transform.forward * (renderCamera.nearClipPlane + 0.01f), Quaternion.identity, material, 0, renderCamera);

            // Render the texture and save it as a Texture2D
            renderCamera.Render();
            Texture2D texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGB24, false);
            RenderTexture.active = renderTexture;
            texture.ReadPixels(new Rect(0, 0, textureWidth, textureHeight), 0, 0);
            texture.Apply(); // Apply the changes to the texture

            // Clean up resources
            RenderTexture.active = null;
            renderCamera.targetTexture = null;
            Object.DestroyImmediate(renderTexture); // Destroy the render texture
            Object.DestroyImmediate(cameraObject); // Destroy the camera object

            return texture;
        }

        // Method to build a quad mesh
        public static Mesh BuildQuadMesh()
        {
            Mesh mesh = new Mesh();

            // Define the vertices of the quad
            Vector3[] vertices = new Vector3[4]
            {
                new Vector3(-0.5f, -0.5f, 0), // Bottom-left
                new Vector3(0.5f, -0.5f, 0),  // Bottom-right
                new Vector3(-0.5f, 0.5f, 0),  // Top-left
                new Vector3(0.5f, 0.5f, 0)    // Top-right
            };

            // Define the UV coordinates for the quad
            Vector2[] uv = new Vector2[4]
            {
                new Vector2(0, 0), // Bottom-left
                new Vector2(1, 0), // Bottom-right
                new Vector2(0, 1), // Top-left
                new Vector2(1, 1)  // Top-right
            };

            // Define the triangles that make up the quad
            int[] triangles = new int[6]
            {
                0, 2, 1, // First triangle
                1, 2, 3  // Second triangle
            };

            // Assign the vertices, UVs, and triangles to the mesh
            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;

            mesh.RecalculateNormals(); // Recalculate the normals for lighting calculations

            return mesh;
        }

        // Method to save the rendered texture to a PNG file
        public static bool SaveTexture(Texture2D texture, string filename, string outputPath)
        {
            // Encode the texture to PNG format
            byte[] bytes = texture.EncodeToPNG();

            // Construct the full file path
            string filePath = System.IO.Path.Combine(outputPath, filename);

            // Write the PNG file to the specified path
            System.IO.File.WriteAllBytes(filePath, bytes);

            // Refresh the AssetDatabase to show the new file in the Unity Editor
            AssetDatabase.Refresh();

            Debug.Log($"Quad rendered and saved to {filePath}"); // Log the file path for reference
            return true; // Return success
        }
    }
}
