using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using HDF.PInvoke;
using JetBrains.Annotations;
using UnityEngine;

namespace MoshPlayer.Scripts.FileLoaders {
    public static class UnityH5 {

        static readonly ulong[] MaxDimensions = {10000, 10000, 10000};

        public static int[] LoadIntDataset(string filePath, string datasetName) {
            long[] longArray = LoadDataset<long>(filePath, datasetName);
            int[] integerArray = longArray.Select(item => (int) item).ToArray();
            return integerArray;
        }

        public static float[] LoadFloatDataset(string filePath, string datasetName) {
            double[] doubleArray = LoadDataset<double>(filePath, datasetName);
            float[] floatArray = doubleArray.Select(item => (float) item).ToArray();
            return floatArray;
        }


        public static string[] LoadStringDataset(string filePath, string dataSetName) {
            //With much Help from:  https://stackoverflow.com/questions/23295545/reading-string-array-from-a-hdf5-dataset
            long fileId = H5F.open(filePath, H5F.ACC_RDONLY);
            long datasetId = H5D.open(fileId, dataSetName);
            long spaceID = H5D.get_space(datasetId);
            long dataType = H5D.get_type(datasetId);

            int[] dimensions = GetDimensions(spaceID);

            int stringLength = (int) H5T.get_size(dataType);
            byte[] buffer = new byte[dimensions[0] * stringLength];

            GCHandle gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);

            string longJoinedString;
            try {
                H5D.read(datasetId, dataType, H5S.ALL, H5S.ALL, H5P.DEFAULT, gch.AddrOfPinnedObject());
                longJoinedString = Encoding.ASCII.GetString(buffer);

            }
            finally {
                gch.Free();
                H5D.close(datasetId);
            }

            return longJoinedString.SplitInParts(stringLength).Select(ss => (string) (object) ss).ToArray();
        }

        static int[] ConvertDimensionsToIntegers(ulong[] dims) {
            if (dims == null) throw new ArgumentNullException(nameof(dims));
            int[] dimensions = new int[dims.Length];
            for (int i = 0; i < dims.Length; i++) {
                Debug.Log(dims[i]);
                dimensions[i] = (int) dims[i];
                Debug.Log($"dimensions {i}: {dimensions[i]}");
            }

            return dimensions;
        }


        static IEnumerable<string> SplitInParts(this string theLongString, int partLength) {
            if (theLongString == null)
                throw new ArgumentNullException(nameof(theLongString));
            if (partLength <= 0)
                throw new ArgumentException("Part length has to be positive.", nameof(partLength));

            for (var i = 0; i < theLongString.Length; i += partLength)
                yield return theLongString.Substring(i, Math.Min(partLength, theLongString.Length - i));
        }

        static T[] LoadDataset<T>(string filePath, string datasetName) {

            long fileId = H5F.open(filePath, H5F.ACC_RDONLY);
            
            T[] resultArray;

            try {

                long datasetId = H5D.open(fileId, datasetName);
                if (datasetId == -1)
                    Debug.LogError("Dataset could not be opened. Check filepath exists and is correct");
                long typeId = H5D.get_type(datasetId);
                long spaceID = H5D.get_space(datasetId);

                int[] dimensions = GetDimensions(spaceID);

                resultArray = new T[dimensions[0]];
                GCHandle gch = GCHandle.Alloc(resultArray, GCHandleType.Pinned);

                try {
                    H5D.read(datasetId, typeId, H5S.ALL, H5S.ALL, H5P.DEFAULT, gch.AddrOfPinnedObject());
                }
                finally {
                    gch.Free();
                    H5D.close(datasetId);
                }
            }
            finally {
                H5F.close(fileId);
            }

            return resultArray;

        }

        static int[] GetDimensions(long spaceID) {
            int numberOfDimensions = H5S.get_simple_extent_ndims(spaceID);

            int[] dimensions = new int[0];
            if (numberOfDimensions >= 0) {
                ulong[] dims = new ulong[numberOfDimensions];
                H5S.get_simple_extent_dims(spaceID, dims, MaxDimensions);
                dimensions = ConvertDimensionsToIntegers(dims);
            }

            return dimensions;
        }
    }

}
