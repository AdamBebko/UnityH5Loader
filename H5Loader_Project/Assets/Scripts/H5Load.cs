using System.IO;
using System.Runtime.InteropServices;
using HDF.PInvoke;
using UnityEngine;

namespace Assets.Scripts {
    public class H5Load : MonoBehaviour {
        // Start is called before the first frame update
        void Start() {
            var filePath = Path.Combine(Application.dataPath, "testfile.hdf5");
            long fileId = H5F.open(filePath, H5F.ACC_RDONLY);
            Debug.Log(fileId);
        }
        
        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
