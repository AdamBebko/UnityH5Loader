import h5py
import numpy as np

with h5py.File("testfile.hdf5", "w") as f:
    intArray = np.array([0, 1, 2, 3, 4, 5, 6, 7, 8, 9], dtype='i8')
    print(intArray)

    floatArray = [0.0, .1, .2, .3, .4, .5, .6, .7, .8, .9]
    print(floatArray)

    stringArray = np.array(['string1', 'string2', 'string3', 'string4', 'string5'], dtype='S')
    print(stringArray)

    twoDArray = np.array([[1, 2, 3], [4, 5, 6]])
    print(twoDArray)

    intDataset = f.create_dataset("integers", data=intArray)
    floatDataset = f.create_dataset("floats", data=floatArray)
    stringDataset = f.create_dataset("strings", data=stringArray)
    twoDDataset = f.create_dataset("twoD", data=twoDArray)

    print(f.keys())

