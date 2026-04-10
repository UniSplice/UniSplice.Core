using System.IO;
using System.Reflection;
using UnityEngine;

namespace UniSplice
{
    public class UniSpliceBehaviour : MonoBehaviour
    {
        private Texture2D _splashTexture;

        internal static string loadingText = "UniSplice";
        
        private void Awake()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream("UniSplice.Splash.png"))
            {
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);

                var tempPath = Path.Combine(Application.temporaryCachePath, "UniSplice_Splash.png");
                File.WriteAllBytes(tempPath, bytes);

                var www = new WWW("file://" + tempPath);
                _splashTexture = www.texture;

                File.Delete(tempPath);
            }
        }

        public void OnGUI()
        {
            if (_splashTexture != null)
            {
                // Draw Stretched image first for the background color (maybe later just have another texture that is blank, or draw the color manually)
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), _splashTexture);
                
                float screenWidth = Screen.width;
                float screenHeight = Screen.height;

                float textureWidth = _splashTexture.width;
                float textureHeight = _splashTexture.height;

                // Scale based on height (100% vertical fit)
                float scale = screenHeight / textureHeight;

                float scaledWidth = textureWidth * scale;
                float scaledHeight = screenHeight; // exact fit vertically

                // Center horizontally
                float x = (screenWidth - scaledWidth) * 0.5f;
                float y = 0f;

                GUI.DrawTexture(
                    new Rect(x, y, scaledWidth, scaledHeight),
                    _splashTexture,
                    ScaleMode.StretchToFill,
                    true
                );
            }

            var style = new GUIStyle(GUI.skin.label)
            {
                fontSize = 40,
                alignment = TextAnchor.LowerCenter
            };
            
            GUI.Label(new Rect(0, Screen.height - 120, Screen.width, 100), $"Loading - {loadingText}...", style);
        }

        private void OnDestroy()
        {
            if (_splashTexture != null)
            {
                Destroy(_splashTexture);
            }
        }
    }
}