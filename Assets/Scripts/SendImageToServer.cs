using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace PDIProject
{
    public class SendImageToServer : MonoBehaviour
    {
        public static SendImageToServer Instance;
        public PrefabManager prefabManager;

        private void Awake()
        {
            Instance = this;
        }

        public void SendImage(string filePath)
        {
            StartCoroutine(UploadImage(filePath));
        }

        public IEnumerator UploadImage(string filePath)
        {
            byte[] imageData = System.IO.File.ReadAllBytes(filePath);

            WWWForm form = new WWWForm();
            form.AddBinaryData("file", imageData, "image.png", "image/png");

            using (UnityWebRequest www = UnityWebRequest.Post("http://127.0.0.1:5000/procesar", form))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    string jsonResponse = www.downloadHandler.text;
                    Debug.Log($"Respuesta JSON cruda del servidor: {jsonResponse}");

                    try
                    {
                        // Parsear la respuesta JSON
                        JObject response = JObject.Parse(jsonResponse);
                        string className = (string)response["class"];
                        string segmentedImageUrl = (string)response["segmented_image_url"];

                        Debug.Log($"Clase recibida del servidor: {className}");
                        Debug.Log($"URL de imagen segmentada: {segmentedImageUrl}");

                        if (!string.IsNullOrEmpty(className))
                        {
                            // Instanciar el prefab
                            GameObject instantiatedPrefab = prefabManager.InstantiatePrefabByName(className);

                            // Descargar y aplicar el material al segundo hijo del prefab instanciado
                            if (!string.IsNullOrEmpty(segmentedImageUrl))
                            {
                                StartCoroutine(DownloadAndApplyMaterial(segmentedImageUrl, instantiatedPrefab));
                            }
                        }
                        else
                        {
                            Debug.LogError("Clase desconocida recibida del servidor.");
                        }
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"Error procesando la respuesta JSON: {e.Message}");
                    }
                }
                else
                {
                    Debug.LogError($"Error al enviar imagen: {www.responseCode} - {www.error}");
                }
            }
        }

        private IEnumerator DownloadAndApplyMaterial(string imageUrl, GameObject prefabInstance)
        {
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    // Crear la textura descargada
                    Texture2D downloadedTexture = DownloadHandlerTexture.GetContent(request);

                    // Crear un nuevo material con la textura descargada
                    Material newMaterial = new Material(Shader.Find("Standard"));
                    newMaterial.mainTexture = downloadedTexture;

                    // Buscar el segundo hijo del prefab
                    if (prefabInstance.transform.childCount >= 2)
                    {
                        Transform secondChild = prefabInstance.transform.GetChild(1);
                        Renderer childRenderer = secondChild.GetComponent<Renderer>();

                        if (childRenderer != null)
                        {
                            // Aplicar el material al segundo hijo
                            childRenderer.material = newMaterial;
                            Debug.Log("Material aplicado correctamente al segundo hijo del prefab.");
                        }
                        else
                        {
                            Debug.LogError("El segundo hijo no tiene un componente Renderer para aplicar el material.");
                        }
                    }
                    else
                    {
                        Debug.LogError("El prefab no tiene suficientes hijos para encontrar el segundo hijo.");
                    }
                }
                else
                {
                    Debug.LogError($"Error al descargar la imagen segmentada: {request.error}");
                }
            }
        }
    }
}
