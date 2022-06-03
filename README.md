# H5UnityLoader
A simple library for loading H5 files in Unity. Basically a simple wrapper for PInvoke. Includes a unity-friendly version of PInvoke

Directly supports loading:

* 1D arrays of: `float`, `int`, `string`
* 2D arrays of: `float`, `int`
* Advanced: Also supports generic methods to load any datatype, but these generic types do not play well with unity, and must usually be cast manually to unity-friendly types. They are typically types `byte`, `ulong`, `double`, etc.

Tested on macos and windows 64 bit. Should also work on linux but untested.

If h5 is from python, it's easier (especially for strings) if datasets are created from numpy arrays with the dtype set to `'i8'` (int), `'float'`, or `'S'` (string).

## Installation
 
#### Unity 2020.1 and later (recommended):
 
1. Edit > Project settings > Package manager
2. Add a new scoped registry fill in the following information:
    ```text
    name: AdamBebko
    url: https://registry.npmjs.org
    scope: com.adambebko.unityh5loader
    ```
3. Go to Window > Package Manager > open the "My Registries tab"
4. Click on the package and hit install.
 
You can use this to update the package easily in the future.
 
#### Unity 2019 (not recommended):
 
From git url in package manager: 
 ```text
https://github.com/AdamBebko/UnityH5Loader.git#X.Y.Z
```

Where x.y.z is the most recent release as seen on the [GitHub](https://github.com/AdamBebko/UnityH5Loader) page.

It is much harder to get updates using this method.

#### As old .unitypackage (not recommended at all):
 
go to the [GitHub](https://github.com/BioMotionLab/UnityPDFDisplay) page and download the contents of the "upm" branch as a zip. Drag unzipped folder into assets folder.

## Use:

reference the H5Loader assembly and namespace.

There are public static methods for each kind of supported datatype. For example:

```c#
using UnityH5Loader;

float[] loadedFloatArray = H5Loader.LoadFloatDataset(filePath, "floatdatasetname");
int[,] loadedInt2DArray = H5Loader.Load2dIntDataset(filePath, "Dtwointdatasetname");
```

See included sample (accessible in package manager window) for more examples.

# FAQ

### I'm getting an error in the console that is blank

Are you on MacOS? yeah..... you're out of luck. Quit unity and reopen it. This seems to happen a ton. As far as I can tell it's a bug in the PInvoke DLLs. I've tried to fix this to no avail. It only seems to happen with the mac DLL in the project. 

Are you on windows? try deleting the mac DLLS from the package.

### I'm getting a stack overflow when I read the files!

The tool I made is VERY finicky about data type and data structures in the H5 file. Read the above readme carefuly and make sure you data is in one of those specifc formats. Otherwise it simply won't work. 

Another possibility is that you've loaded your huge data, and then are looping through it. Don't do any loops over huge datasets, especially not using Debug.Log. To check if your data has loaded correctly, I use something like this:

```csharp
int[,] loadedInt2DArray = H5Loader.Load2dIntDataset(fullPath, "data");
Debug.Log($"loaded size: {loadedInt2DArray.GetLength(0)}, {loadedInt2DArray.GetLength(1)}");
```
