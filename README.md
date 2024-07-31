PointCloud-Visualization-Tool
======

![Image](https://github.com/LixiangZhao98/asset/blob/master/Project/PointCloud-Visualization-Tool/pic/teaser.png "Image")
Kernel density estimation algrithm for point cloud visualization in [Unity3D](https://unity3d.com/get-unity/download "Unity download").\
Based on the density field, the repo support to reconstruct the 3D geometric shape by Marching Cube algorithm and implement color encoding.
Any pull requests and issues are welcome. If you find it useful, could you please leave a star here? Thanks in advance.

### Projects built based on this repo
[MeTACAST](https://github.com/LixiangZhao98/MeTACAST "MeTACAST")

# Features
- pointcloud data reader in .bin(xyz coordinates in binary format) and .ply
- Kernal Density Estimation(KDE) of the point cloud density
- Iso-surface construction with Marching-Cube algrithm based on point cloud density
- Color-coded based on point cloud density
- Halo Visualization for point cloud

# Install the project and Play the demos

## Install the project
- Download Unity Hub. Please refer to sec.1-4 in [tutorial](https://github.com/LixiangZhao98/asset/blob/master/Tutorial/Unity_Setup_General.pdf) if you are a new Unity user.
- Clone the repo with git lfs installed or download the archive [https://github.com/LixiangZhao98/PointCloud-Visualization-Tool/archive/refs/heads/master.zip](https://github.com/LixiangZhao98/PointCloud-Visualization-Tool/archive/refs/heads/master.zip "archive") and open the project using Unity (versions equal to/higher than 2019 have been tested). Please refer to sec.6 in [tutorial](https://github.com/LixiangZhao98/asset/blob/master/Tutorial/Unity_Setup_General.pdf) if you don't know how to open an existing project.


## Demo1: Read data, visualize data, calculate density field
- Demo in `Assets/PointCloud-Visualization-Tool/Scenes/PointCloudVisualization.unity`
- To switch datasets, click the gameobject `script/RunTime` in Hierarchy and change `datasets` in the inspector window. 
- To enable the density calculation, click the gameobject `script/RunTime` in Hierarchy and set `CalculateDensity` as true in the inspector window before running the game. 
- To generate iso-surface and change threshold, click the gameobject `script/RunTime` in Hierarchy and adjust `MCGPUThreshold` in the inspector window. Then you can see the iso-surface enclosing the region with density higher than `MCGPUThreshold` just as follows.
![Image](https://github.com/LixiangZhao98/asset/blob/master/Project/PointCloud-Visualization-Tool/pic/marchingcube.png "Image")

## Demo2: Color mapping
- Color mapping based on density from low (blue) to high (red)
- Demo in `Assets/PointCloud-Visualization-Tool/Scenes/ColorMapping.unity`
![Image](https://github.com/LixiangZhao98/asset/blob/master/Project/PointCloud-Visualization-Tool/pic/FieldColor.png "Image")

## Demo3: Halo visualization
- Halo visualization published in [10.1109/TVCG.2009.138](https://ieeexplore.ieee.org/document/5290742 "Depth-Dependent Halos")
- Demo in `Assets/PointCloud-Visualization-Tool/Scenes/Halo.unity` 
- The .ply files can be downloaded from [https://graphics.stanford.edu/data/3Dscanrep/](https://graphics.stanford.edu/data/3Dscanrep/)
![Image](https://github.com/LixiangZhao98/asset/blob/master/Project/PointCloud-Visualization-Tool/halos.png "Image")


# Scripting

The following are all in `Assets/PointCloud-Visualization-Tool/Scenes/MyPointCloud.unity`.
  
## Load data from binary files
- Add `RenderDataRunTime` to an empty GameObject (you can name it whatever you like, here we call it "Runtime").
- Create a new script (you can name it whatever you like, here we call it `MyPointCloud.cs`) and add it to GameObject "Runtime".
- In `MyPointCloud.cs`, We first initialize two varaibles `particleMat` and `visCenter`. Remember to assign these two variables in the inspector.
```c#
public Material particleMat;  // the material of the points
public GameObject visCenter; //The visualization will always follow this GameObject when starting the game. 
``` 
- To load data from binary files, we can simply call `DataMemory.LoadDataByByte(fileName)`. An example code is as follows:
```c#
public class MyPointCloud : MonoBehaviour
{
    public Material particleMat;  // the material of the points
    public GameObject visCenter; //The visualization will always follow the `Vis center` when starting the game. 
    void Start()
    {
        DataMemory.StacksInitialize();  //Initialize
        DataMemory.LoadDataByByte("Flocculentcube2");  //load the data from the the binary file; the input is the name of the binary file
        RenderDataRunTime.visSize = 1f;  //Set the size of the visualization as 1 meter
        RenderDataRunTime.Init(visCenter,particleMat);  // Assign materials and center to the RenderDataRunTime.cs`
    }
}
```
![Image](https://github.com/LixiangZhao98/PointCloud-Visualization-Tool/blob/master/Assets/pic/LoadBinary.png "Image")
- The binary files stores x,y,z coordinates (32bits for each coordinate) for each point in binary located in `Asset/PointCloud-Visualization-Tool/data/data` folder. Here is a full review of the [Point Cloud Dataset included in this repo](https://github.com/LixiangZhao98/PointCloud-Visualization-Tool/blob/master/Assets/files/Data.pdf "Data").

## Load data by point positions
- To load data by point positions, we can build a `Vector3[] vector3Array` and call `DataMemory.LoadDataByVec3s(vector3Array,name)`. An example to generate a group of points in a cubic range is as follows:
```c#
public class MyPointCloud : MonoBehaviour
{
    public Material particleMat;  // the material of the points
    public GameObject visCenter; //The visualization will always follow the `Vis center` when starting the game. 
    void Start()
    {
        Vector3[] v = Generate_Cube();  // Generate random points in Cubic shape
        DataMemory.StacksInitialize();//Initialize
        DataMemory.LoadDataByVec3s(v, "cube");  // the first input is Vector[], the second is the name of the data (you can name it as you like)
        RenderDataRunTime.visSize = 1f;  //the size of the visualization
        RenderDataRunTime.Init(visCenter,particleMat);  // Assign materials and center to the RenderDataRunTime.cs`
    }
    public Vector3[] Generate_Cube()  // Generate random points in Cubic shape
    {
        Random.InitState(2);
        int num = 100000;
        int i = 0;
        Vector3[] v = new Vector3[num];
        while (i < num)
        {
            v[i] = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
            i++;
        }
        return v;
    }
}
```
![Image](https://github.com/LixiangZhao98/PointCloud-Visualization-Tool/blob/master/Assets/pic/LoadVec3s.png "Image")

## Load data from ply files

```c#
public class MyPointCloud : MonoBehaviour
{
    public Material particleMat;  // the material of the points
    public GameObject visCenter; //The visualization will always follow the `Vis center` when starting the game. 
    void Start()
    {
        DataMemory.StacksInitialize();  //Initialize
        DataMemory.LoadDataByPly("dragon_vrip");  //load the data from the the ply file; the input is the name of the binary file
        RenderDataRunTime.visSize = 1f;  //Set the size of the visualization as 1 meter
        RenderDataRunTime.Init(visCenter,particleMat);  // Assign materials and center to the RenderDataRunTime.cs`
    }
}
```
![Image](https://github.com/LixiangZhao98/PointCloud-Visualization-Tool/blob/master/Assets/pic/LoadPly.png "Image")
### Traversal of points
To get information of each point, such as the position. We can simply do as following:
```c#
void Start()
{
for(int i=0;i<DataMemory.allParticle.GetParticlenum();i++)
{
    Debug.Log(DataMemory.allParticle.GetParticlePosition(i));
}
}
```

# Thanks
Many thanks to the authors of open-source repository:
[unity-marching-cubes-gpu](https://github.com/pavelkouril/unity-marching-cubes-gpu "unity-marching-cubes-gpu")





