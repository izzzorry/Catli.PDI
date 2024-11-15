using UnityEngine;
using System.IO;

namespace PDIProject
{
    public class UploadImage : MonoBehaviour
    {
        public void OpenFileBrowser()
        {
            // Configurar filtros para tipos de archivo
            SimpleFileBrowser.FileBrowser.SetFilters(true, new string[] { ".png", ".jpg", ".jpeg" });

            // Mostrar el explorador de archivos
            SimpleFileBrowser.FileBrowser.ShowLoadDialog(
                (paths) =>
                {
                    string selectedPath = paths[0]; // Obtener la primera ruta seleccionada
                    Debug.Log($"Imagen cargada desde: {selectedPath}");

                    // Enviar la imagen al servidor
                    SendImageToServer.Instance.SendImage(selectedPath);
                },
                () =>
                {
                    Debug.Log("Explorador cancelado");
                },
                SimpleFileBrowser.FileBrowser.PickMode.Files, // Seleccionar archivos
                false, // No se permite seleccionar múltiples archivos
                null, // Carpeta inicial (opcional)
                null, // Título del explorador (opcional)
                "Seleccionar imagen" // Texto del botón de confirmación
            );
        }
    }
}
