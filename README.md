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
- To calculate the density field of point cloud by KDE and generate iso-surface, click the gameobject `script/RunTime` in Hierarchy and set `CalculateDensity` as true in the inspector window. To change threshold, click the gameobject `script/RenderDataRunTime` in Hierarchy and adjust `MCGPUThreshold` in the inspector window. Then you can see the iso-surface enclosing the region with density higher than `MCGPUThreshold` just as follows.
![Image](https://github.com/LixiangZhao98/PointCloud-Visualization-Tool/blob/master/Assets/pic/marchingcube.png "Image")
- `Assets/PointCloud-Visualization-Tool/Scenes/ColorMapping.unity` is a demo to show color mapping based on density information. In this demo, `CalculateDensity` is set to true by default. The effect is as follows:
![Image](https://github.com/LixiangZhao98/PointCloud-Visualization-Tool/blob/master/Assets/pic/FieldColor.png "Image")
- `Assets/PointCloud-Visualization-Tool/Scenes/Halo.unity` is a code replication of halo visualization in Unity for point cloud data([10.1109/TVCG.2009.138](https://ieeexplore.ieee.org/document/5290742 "Depth-Dependent Halos")). The .ply can be downloaded from 
[https://graphics.stanford.edu/data/3Dscanrep/](https://graphics.stanford.edu/data/3Dscanrep/). In this demo, `CalculateDensity` is set to false by default.
![Image](https://github.com/LixiangZhao98/PointCloud-Visualization-Tool/blob/master/Assets/pic/halos.png "Image")


# Scripting
- Copy `Asset/PointCloud-Visualization-Tool` folder in this repo to your `Asset` folder.
- Add `RenderDataRunTime` script to an empty GameObject (you can name it whatever you like, here we call it "scriptObj"). Assign the `particleMat` and `Vis center` in the public field. The visualization will always follow the `Vis center` when start. 
- Create a new script and add it to GamoObject "scriptObj". And we can write the C# code in it. The following are some examples to visualize the point cloud data.
### Load data from binary files and visualize
To load data from binary files, we can simply call `DataMemory.LoadDataByByte(fileName)`. An example is as follows:
```c#
void Start()
{
DataMemory.StacksInitialize();  //Initialize
DataMemory.LoadDataByByte("Flocculentcube2");  //load the data from the the binary file; the input is the name of the binary file
RenderDataRunTime.GenerateMesh();  // Draw the pointcloud Mesh and render in `RenderDataRunTime/cs`
}
```
The visualization follows the movement and the rotation of the GameObject `Vis center`, which is assigned in the public field of `RenderDataRunTime`.\
The size of pointcloud data can be set by:
```c#
RenderDataRunTime.visSize=20f;
```
The data files are stored in `Asset/PointCloud-Visualization-Tool/data/data` folder. The .bin format stores x,y,z coordinates for each point in binary. Each coordinate is stored in single (32bits) format. \
Here is a full review of the [Point Cloud Dataset included in this repo](https://github.com/LixiangZhao98/PointCloud-Visualization-Tool/blob/master/Assets/files/Data.pdf "Data").

### Load data by point positions and visualize
To load data by point positions, we can build a `Vector3[]` and call `DataMemory.LoadDataByVec3s(vector3Array,name)`. An example to generate a group of points in a cubic range is as follows:
```c#
void Start()
{
Vector3[] v= Generate_Cube();  // Generate random points in Cubic shape
DataMemory.StacksInitialize();//Initialize
DataMemory.LoadDataByVec3s(v,"cube");  // the first input is Vector[], the second is the name of the data (you can name it as you like)
RenderDataRunTime.GenerateMesh();
}
public Vector3[] Generate_Cube()  // Generate random points in Cubic shape
    {
        Random.InitState(2);
        int num = 10000;
        int i = 0;
        Vector3[] v = new Vector3[num];
        while (i < num)
        {
            v[i] = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
            i++;
        }
        return v;
    }
```
### Load data from ply files and visualize
```c#
void Start()
{
DataMemory.StacksInitialize();  //Initialize
DataMemory.LoadDataByPly("dragon_vrip");  //load the data from the the ply file; the input is the name of the binary file
RenderDataRunTime.GenerateMesh();  // Draw the pointcloud Mesh and render in `RenderDataRunTime/cs`
}
```

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





