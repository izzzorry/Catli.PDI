using UnityEngine;

namespace PDIProject
{
    public class PrefabManager : MonoBehaviour
    {
        public GameObject colibriPrefab;
        public GameObject gatoPrefab;
        public GameObject mariposaPrefab;

        public GameObject InstantiatePrefabByName(string className)
        {
            GameObject prefabToInstantiate = null;

            switch (className)
            {
                case "Colibrí":
                    prefabToInstantiate = colibriPrefab;
                    break;
                case "Gato":
                    prefabToInstantiate = gatoPrefab;
                    break;
                case "Mariposa":
                    prefabToInstantiate = mariposaPrefab;
                    break;
                default:
                    Debug.LogError($"Clase desconocida: {className}");
                    return null; // Retorna null si no se encuentra el prefab
            }

            if (prefabToInstantiate != null)
            {
                GameObject instantiatedPrefab = Instantiate(prefabToInstantiate, prefabToInstantiate.transform.position, Quaternion.identity);
                Debug.Log($"Prefab instanciado para clase: {className}");
                return instantiatedPrefab; // Retorna el prefab instanciado
            }

            return null; // Retorna null en caso de algún error inesperado
        }
    }
}
