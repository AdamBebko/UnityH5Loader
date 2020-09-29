using System.IO;
using UnityEditor;
using UnityEngine;
using UnityH5Loader;

namespace SampleLoad {
    public class H5SampleLoader : MonoBehaviour {
        // Start is called before the first frame update

        [SerializeField] Object h5File = default;
        
        void Start() {
            string projectFolder = Path.GetDirectoryName(Application.dataPath);
            if (projectFolder == null) throw new FileNotFoundException("Couldn't retrieve project folder");
            string filePath = Path.Combine(projectFolder, AssetDatabase.GetAssetPath(h5File));
            if (!File.Exists(filePath)) throw new FileNotFoundException($"File not found {filePath}");
            Debug.Log($"Loading file: {filePath}");

            Debug.Log("Loading ints dataset");
            int[] ints = H5Loader.LoadIntDataset(filePath, "integers");
            foreach (int i in ints) {
                Debug.Log($"ints: {i}");
            }
            
            
            Debug.Log("Loading floats dataset");
            float[] floats = H5Loader.LoadFloatDataset(filePath, "floats");
            foreach (float i in floats) {
                Debug.Log($"floats: {i}");
            }

            Debug.Log("Loading strings dataset");
            string[] strings = H5Loader.LoadStringDataset(filePath, "strings");
            foreach (string i in strings) {
                Debug.Log($"strings: {i}");
            }

            Debug.Log("Loading 2d ints dataset");
            int[,] int2d = H5Loader.Load2dIntDataset(filePath, "twoD");
            for (int i = 0; i < int2d.GetLength(0); i++) {
                for (int j = 0; j < int2d.GetLength(1); j++) {
                    Debug.Log($"twoD int: {i}, {j} = {int2d[i,j]}");
                }
            }
            
            Debug.Log("Loading 2d int dataset as floats");
            float[,] float2d = H5Loader.Load2dFloatDataset(filePath, "twoD");
            for (int i = 0; i < int2d.GetLength(0); i++) {
                for (int j = 0; j < int2d.GetLength(1); j++) {
                    Debug.Log($"twoD float: {i}, {j} = {int2d[i,j]}");
                }
            }
            

        }
        
      
    }
}
