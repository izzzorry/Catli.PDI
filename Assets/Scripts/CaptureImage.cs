using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace PDIProject
{
    public class CaptureDeviceImage : MonoBehaviour
    {
        public RawImage cameraPreview;  // UI para mostrar la cámara
        public string imageName = "capturedImage.png";

        private WebCamTexture webCamTexture;

        void Start()
        {
            // Inicializar la cámara del dispositivo
            StartDeviceCamera();
        }

        public void StartDeviceCamera()
        {
            // Verificar si hay cámaras disponibles
            if (WebCamTexture.devices.Length > 0)
            {
                WebCamDevice device = WebCamTexture.devices[0];  // Usar la primera cámara disponible
                webCamTexture = new WebCamTexture(device.name);

                // Asignar la textura de la cámara al RawImage para mostrar el feed
                cameraPreview.texture = webCamTexture;
                cameraPreview.material.mainTexture = webCamTexture;

                webCamTexture.Play();  // Iniciar la cámara
                Debug.Log("Cámara del dispositivo iniciada.");
            }
            else
            {
                Debug.LogError("No se detectaron cámaras en el dispositivo.");
            }
        }

        public void CaptureAndSaveImage()
        {
            if (webCamTexture != null && webCamTexture.isPlaying)
            {
                // Capturar el frame actual de la cámara
                Texture2D capturedImage = new Texture2D(webCamTexture.width, webCamTexture.height);
                capturedImage.SetPixels(webCamTexture.GetPixels());
                capturedImage.Apply();

                // Guardar la imagen en almacenamiento persistente
                byte[] imageBytes = capturedImage.EncodeToPNG();
                string filePath = Path.Combine(Application.persistentDataPath, imageName);
                File.WriteAllBytes(filePath, imageBytes);

                Debug.Log($"Imagen capturada y guardada en: {filePath}");

                // Enviar la imagen al servidor
                SendImageToServer.Instance.SendImage(filePath);
            }
            else
            {
                Debug.LogError("La cámara no está activa o no está disponible.");
            }
        }

        public void StopDeviceCamera()
        {
            if (webCamTexture != null && webCamTexture.isPlaying)
            {
                webCamTexture.Stop();  // Detener la cámara
                Debug.Log("Cámara del dispositivo detenida.");
            }
        }
    }
}
