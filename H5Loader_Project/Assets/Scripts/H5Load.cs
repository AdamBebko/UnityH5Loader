using System.IO;
using MoshPlayer.Scripts.FileLoaders;
using UnityEngine;

namespace Assets.Scripts {
    public class H5Load : MonoBehaviour {
        // Start is called before the first frame update
        void Start() {
            var filePath = Path.Combine(Application.dataPath, "testfile.hdf5");

            int[] ints = UnityH5.LoadIntDataset(filePath, "integers");
            foreach (int i in ints) {
                Debug.Log(i);
            }
            
            float[] floats = UnityH5.LoadFloatDataset(filePath, "floats");
            foreach (float i in floats) {
                Debug.Log(i);
            }

            string[] strings = UnityH5.LoadStringDataset(filePath, "strings");
            foreach (string i in strings) {
                Debug.Log(i);
            }

        }
        
      
    }
}
