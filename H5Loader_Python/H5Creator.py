import h5py
import numpy as np

with h5py.File("testfile.hdf5", "w") as f:
    intArray = np.array([0, 1, 2, 3, 4, 5, 6, 7, 8, 9], dtype='i8')
    print(intArray)
    intDataset = f.create_dataset("integers", data=intArray)
    print(intDataset)
    floatArray = intArray/3.0;
    print(floatArray)
    floatDataset = f.create_dataset("floats", data=floatArray)
    print(floatDataset)

    stringArray = np.array(['string1', 'string2', 'string3', 'string4', 'string5'], dtype='S')
    print(stringArray)
    stringDataset = f.create_dataset("strings", data=stringArray)
    print(stringDataset)

    twoDArray = np.array([[1, 2, 3], [4, 5, 6]])
    print(twoDArray)
    twoDDataset = f.create_dataset("twoD", data=twoDArray)
    print(twoDDataset)