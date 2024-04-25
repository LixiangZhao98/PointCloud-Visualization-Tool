PointCloud-Visualization-Tool
======

![Image](https://github.com/LixiangZhao98/PointCloud-Visualization-Tool/blob/master/Assets/pic/teaser.png "Image")
This is a repository for point cloud visualization built on [Unity3D](https://unity3d.com/get-unity/download "Unity download").\
The point cloud data can be loaded in format .bin (xyz coordinates in binary format) and .ply.\
Any pull requests and issues are welcome. If you find it useful, could you please leave a star here? Thanks in advance.

### Projects built based on this repo
[MeTACAST](https://github.com/LixiangZhao98/MeTACAST "MeTACAST")

# Features
- pointcloud data reader in .bin(xyz coordinates in binary format) and .ply
- Kernal Density Estimation(KDE) of the point cloud density
- Iso-surface construction with Marching-Cube algrithm based on point cloud density
- Color-coded based on point cloud density
- Halo Visualization for point cloud

# Install the project and Play the demo
- Download Unity3D  and Create a new project. Here is a tutorial for [Unity3D Setup](https://github.com/LixiangZhao98/MeTACAST/blob/master/Assets/my/file/UnitySetup.pdf "Unity Setup").
- Clone the repo with git lfs installed or download the archive [https://github.com/LixiangZhao98/PointCloud-Visualization-Tool/archive/refs/heads/master.zip](https://github.com/LixiangZhao98/PointCloud-Visualization-Tool/archive/refs/heads/master.zip "archive") and open the project using Unity (versions higher than 2020.3.38f1 have been tested).
- `Assets/PointCloud-Visualization-Tool/Scenes/PointCloudVisualization.unity` is a demo to read, visualize and calculate the field density of point cloud data. 
- To switch datasets, click the gameobject `script/RunTime` in Hierarchy and change `datasets` in the inspector window. 
- To calculate the density field of point cloud by the modified Breiman kernel density estimation with a finite-support adaptive Epanechnikov kernel, click the gameobject `script/RunTime` in Hierarchy and set `CalculateDensity` as true in the inspector window before running the game. 
- To generate iso-surface and change threshold, click the gameobject `script/RunTime` in Hierarchy and adjust `MCGPUThreshold` in the inspector window. Then you can see the iso-surface enclosing the region with density higher than `MCGPUThreshold` just as follows.
![Image](https://github.com/LixiangZhao98/PointCloud-Visualization-Tool/blob/master/Assets/pic/marchingcube.png "Image")
- `Assets/PointCloud-Visualization-Tool/Scenes/ColorMapping.unity` is a demo to show color mapping based on density information. In this demo, `CalculateDensity` is set to true by default. The effect is as follows:
![Image](https://github.com/LixiangZhao98/PointCloud-Visualization-Tool/blob/master/Assets/pic/FieldColor.png "Image")
- `Assets/PointCloud-Visualization-Tool/Scenes/Halo.unity` is a code replication of halo visualization in Unity for point cloud data([10.1109/TVCG.2009.138](https://ieeexplore.ieee.org/document/5290742 "Depth-Dependent Halos")). The .ply can be downloaded from 
[https://graphics.stanford.edu/data/3Dscanrep/](https://graphics.stanford.edu/data/3Dscanrep/). In this demo, `CalculateDensity` is set to false by default.
![Image](https://github.com/LixiangZhao98/PointCloud-Visualization-Tool/blob/master/Assets/pic/halos.png "Image")


# Scripting
The demo is shown in `Assets/PointCloud-Visualization-Tool/Scenes/MyPointCloud.unity`.
## Load data
- Add `RenderDataRunTime` to an empty GameObject (you can name it whatever you like, here we call it "Runtime"). Create a new script (you can name it whatever you like, here we call it `MyPointCloud.cs`) and add it to GameObject "Runtime". We can write the C# code in MyPointCloud.cs. The following are some examples to read and visualize the point cloud data.
#### Load data from binary files and visualize
We first initialize two varaibles `particleMat` and `visCenter`. Remember to assign these two variables in the inspector.
```c#
public Material particleMat;  // the material of the points
public GameObject visCenter; //The visualization will always follow this GameObject when starting the game. 
``` 
To load data from binary files, we can simply call `DataMemory.LoadDataByByte(fileName)`. An example code is as follows:
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
The data files are stored in `Asset/PointCloud-Visualization-Tool/data/data` folder. The .bin format stores x,y,z coordinates for each point in binary. Each coordinate is stored in single (32bits) format. \
Here is a full review of the [Point Cloud Dataset included in this repo](https://github.com/LixiangZhao98/PointCloud-Visualization-Tool/blob/master/Assets/files/Data.pdf "Data").

#### Load data by point positions and visualize
To load data by point positions, we can build a `Vector3[]` and call `DataMemory.LoadDataByVec3s(vector3Array,name)`. An example to generate a group of points in a cubic range is as follows:
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
#### Load data from ply files and visualize

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





