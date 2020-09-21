import h5py
import numpy

with h5py.File("testfile.hdf5", "r") as f:
    print(f.keys())
    print(f["integers"].value)
    print(f["floats"].value)
    print(f["strings"].value)
    print(f["twoD"].value)