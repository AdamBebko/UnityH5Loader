using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using HDF.PInvoke;
using JetBrains.Annotations;

namespace UnityH5Loader {
    public static class H5Loader {

        static readonly ulong[] MaxDimensions = {10000, 10000, 10000};

        /// <summary>=
        /// Loads a 1D int dataset from an H5 file. The dataset must be 'i8' encoded dataset. In python: create it using numpy array with dtype='i8'
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="datasetName"></param>
        /// <returns></returns>
        [PublicAPI]
        public static int[] LoadIntDataset(string filePath, string datasetName) {
            long[] longArray = LoadDataset<long>(filePath, datasetName);
            int[] integerArray = longArray.Select(item => (int) item).ToArray();
            return integerArray;
        }

        /// <summary>
        /// Loads a 1d float dataset from an H5 file. The dataset must be 'float' encoded dataset. In python: create it using numpy array with dtype='float'
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="datasetName"></param>
        /// <returns></returns>
        [PublicAPI]
        public static float[] LoadFloatDataset(string filePath, string datasetName) {
            double[] doubleArray = LoadDataset<double>(filePath, datasetName);
            float[] floatArray = doubleArray.Select(item => (float) item).ToArray();
            return floatArray;
        }

        /// <summary>
        /// Loads a 1d string dataset from an H5 file. The dataset must be 'string' encoded dataset. In python: create it using numpy array with dtype='S'
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="dataSetName"></param>
        /// <returns></returns>
        [PublicAPI]
        public static string[] LoadStringDataset(string filePath, string dataSetName) {
            //With much Help from:  https://stackoverflow.com/questions/23295545/reading-string-array-from-a-hdf5-dataset
            long fileId = H5F.open(filePath, H5F.ACC_RDONLY);
            
            string longJoinedString;
            int stringLength;
            try {
                
                long datasetId = H5D.open(fileId, dataSetName);
                long spaceID = H5D.get_space(datasetId);
                long dataType = H5D.get_type(datasetId);

                int[] dimensions = GetDatasetDimensions(spaceID);

                stringLength = (int) H5T.get_size(dataType);
                byte[] buffer = new byte[dimensions[0] * stringLength];

                GCHandle gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);

                
                try {
                    H5D.read(datasetId, dataType, H5S.ALL, H5S.ALL, H5P.DEFAULT, gch.AddrOfPinnedObject());
                    longJoinedString = Encoding.ASCII.GetString(buffer);
                }
                finally {
                    gch.Free();
                    H5D.close(dataType);
                    H5D.close(spaceID);
                    H5D.close(datasetId);
                }
            }
            finally {
                H5F.close(fileId);
            }

            return longJoinedString.SplitInParts(stringLength).Select(ss => (string) (object) ss).ToArray();
        }
        
        static IEnumerable<string> SplitInParts(this string theLongString, int partLength) {
            if (theLongString == null)
                throw new ArgumentNullException(nameof(theLongString));
            if (partLength <= 0)
                throw new ArgumentException("Part length has to be positive.", nameof(partLength));

            for (var i = 0; i < theLongString.Length; i += partLength)
                yield return theLongString.Substring(i, Math.Min(partLength, theLongString.Length - i));
        }

        /// <summary>
        /// Loads a 2d integer dataset from an H5 file. The dataset must be an 'i8' encoded dataset. In python: create it using numpy array with dtype='i8'
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="datasetName"></param>
        /// <returns></returns>
        [PublicAPI]
        public static int[,] Load2dIntDataset(string filePath, string datasetName) {
            long[,] doubleArray = Load2DDataset<long>(filePath, datasetName);
            int[,] intArray = new int[doubleArray.GetLength(0),doubleArray.GetLength(1)];
            for (int i = 0; i < doubleArray.GetLength(0); i++) {
                for (int j = 0; j < doubleArray.GetLength(1); j++) {
                    intArray[i, j] = (int)doubleArray[i, j];
                }
            }
            return intArray;
        }
        
        /// <summary>
        /// Loads a 2d float dataset from an H5 file. The dataset must be 'float' encoded dataset. In python: create it using numpy array with dtype='float'
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="datasetName"></param>
        /// <returns></returns>
        public static float[,] Load2dFloatDataset(string filePath, string datasetName) {
            double[,] doubleArray = Load2DDataset<double>(filePath, datasetName);
            float[,] floatArray = new float[doubleArray.GetLength(0),doubleArray.GetLength(1)];
            for (int i = 0; i < doubleArray.GetLength(0); i++) {
                for (int j = 0; j < doubleArray.GetLength(1); j++) {
                    floatArray[i, j] = (float)doubleArray[i, j];
                }
            }
            return floatArray;
        }

        

        /// <summary>
        /// WARNING: ADVANCED USE ONLY!! Loads a 1D generic dataset from an H5 file.
        /// The generic loaders only loads data in non-Unity friendly types, such as bytes, uints, longs etc...
        /// You'll have to know the correct cast to retrieve usable data.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="datasetName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        [PublicAPI]
        public static T[] LoadDataset<T>(string filePath, string datasetName) {
            
            if (!File.Exists(filePath)) throw new FileNotFoundException($"Loading dataset {datasetName} from file that doesn't exist {filePath}");
            long fileId = H5F.open(filePath, H5F.ACC_RDONLY);
            
            T[] resultArray;

            try {

                long datasetId = H5D.open(fileId, datasetName);
                if (datasetId == -1) throw new ArgumentException($"Dataset could not be opened. Check filepath exists and is correct. FilePath = {filePath}");
                long typeId = H5D.get_type(datasetId);
                long spaceID = H5D.get_space(datasetId);

                int[] dimensions = GetDatasetDimensions(spaceID);

                resultArray = new T[dimensions[0]];
                GCHandle gch = GCHandle.Alloc(resultArray, GCHandleType.Pinned);

                try {
                    H5D.read(datasetId, typeId, H5S.ALL, H5S.ALL, H5P.DEFAULT, gch.AddrOfPinnedObject());
                }
                finally {
                    gch.Free();
                    H5D.close(typeId);
                    H5D.close(spaceID);
                    H5D.close(datasetId);
                }
            }
            finally {
                H5F.close(fileId);
            }

            return resultArray;

        }

        /// <summary>
        /// WARNING: ADVANCED USE ONLY!! Loads a 2D generic dataset from an H5 file.
        /// The generic loaders only loads data in non-Unity friendly types, such as bytes, uints, longs etc...
        /// You'll have to know the correct cast to retrieve usable data.
        /// 
        /// Created With help from https://github.com/LiorBanai/HDF5-CSharp/blob/master/HDF5-CSharp/Hdf5Dataset.cs
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="datasetName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        static T[,] Load2DDataset<T>(string filePath, string datasetName) {
            
            if (!File.Exists(filePath)) throw new FileNotFoundException($"Loading dataset {datasetName} from file that doesn't exist {filePath}");
            long fileId = H5F.open(filePath, H5F.ACC_RDONLY);
            
            T[,] resultArray = new T[2,2];
            try {
                
                ulong[] start = {0, 0};
                ulong[] count = {0, 0};

                long datasetId = H5D.open(fileId, datasetName);
                var datatype = H5D.get_type(datasetId);
                var spaceId = H5D.get_space(datasetId);
                int rank = H5S.get_simple_extent_ndims(spaceId);
                ulong[] maxDims = new ulong[rank];
                ulong[] dims = new ulong[rank];
                H5S.get_simple_extent_dims(spaceId, dims, maxDims);
                
                count[0] = dims[0];
                count[1] = dims[1];

                // Define file hyperslab. 
                long status = H5S.select_hyperslab(spaceId, H5S.seloper_t.SET, start, null, count, null);
                
                // Define the memory dataspace.
                resultArray = new T[dims[0], dims[1]];
                var memId = H5S.create_simple(rank, dims, null);

                // Define memory hyperslab. 
                status = H5S.select_hyperslab(memId, H5S.seloper_t.SET, start, null,
                    count, null);

                // Read data from hyperslab in the file into the hyperslab in 
                // memory and display.             
                GCHandle handle = GCHandle.Alloc(resultArray, GCHandleType.Pinned);

                try {
                    H5D.read(datasetId, datatype, memId, spaceId,
                        H5P.DEFAULT, handle.AddrOfPinnedObject());
                }
                finally {
                    handle.Free();
                    H5S.close(status);
                    H5S.close(memId);
                    H5S.close(spaceId);
                    H5D.close(datatype);
                    H5D.close(datasetId);
                }
            }
            finally {
                H5F.close(fileId);
            }
            return resultArray;

        }


        static int[] GetDatasetDimensions(long spaceID) {
            int numberOfDimensions = H5S.get_simple_extent_ndims(spaceID);

            int[] dimensions = new int[0];
            if (numberOfDimensions >= 0) {
                ulong[] dims = new ulong[numberOfDimensions];
                H5S.get_simple_extent_dims(spaceID, dims, MaxDimensions);
                dimensions = ConvertDimensionsToIntegers(dims);
            }

            return dimensions;
        }


        static int[] ConvertDimensionsToIntegers(ulong[] dims) {
            if (dims == null) throw new ArgumentNullException(nameof(dims));
            int[] dimensions = new int[dims.Length];
            for (int i = 0; i < dims.Length; i++) {
                dimensions[i] = (int) dims[i];
            }

            return dimensions;
        }
    }

}
