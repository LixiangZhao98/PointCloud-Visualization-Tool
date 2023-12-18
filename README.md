PointCloud-Visualization-Tool
======

![Image](https://github.com/LixiangZhao98/PointCloud-Visualization-Tool/blob/master/Assets/pic/teaser.png "Image")
This is a repository for point cloud visualization built on [Unity3D](https://unity3d.com/get-unity/download "Unity download").\
The point cloud data are in format .bin (xyz coordinates in binary format).\
Any pull requests and issues are welcome. If you find it useful, could you please leave a star here? Thanks in advance.

## Projects built based on this repo
[MeTACAST](https://github.com/LixiangZhao98/MeTACAST "MeTACAST")

# Install the project and Play the demo
- Download Unity3D  and Create a new project. Here is a tutorial ([Unity3D Setup](https://github.com/LixiangZhao98/MeTACAST/blob/master/Assets/my/file/UnitySetup.pdf "Unity Setup")).
- Clone the repo with git lfs installed and open the project using Unity (versions higher than 2020.3.38f1 have been tested).
- `Assets/PointCloud-Visualization-Tool/Scenes/demo.unity` is the demo.
- To switch `datasets`, click the gameobject `script/RunTime` in Hierarchy and switch them in the inspector window

# How to integrate into your project
- Copy `Asset/PointCloud-Visualization-Tool` folder in this repo to your `Asset` folder.
- Add `RenderDataRunTime` script to an empty GameObject (you can name it whatever you like, here we call it "scriptObj"). Assign the `particleMat` and `Vis center` in the public field. The visualization will always follow the `Vis center` when start. 
- Create a new script and add it to GamoObject "scriptObj". And we can write the C# code in it. The following are some examples to visualize the point cloud data.
### Load data from binary files and visualize
```c#
void Start()
{
DataMemory.StacksInitialize();  //Initialize
DataMemory.LoadDataByByte("Flocculentcube2");  //load the data from the the binary file; the input is the name of the binary file
RenderDataRunTime.GenerateMesh();  // Draw the pointcloud Mesh and render in `RenderDataRunTime/cs`
}
```
We visualize the pointcloud data in the scene and it follows the movement and the rotation of the GameObject `Vis center` in the public field of `RenderDataRunTime`.
![Image](https://github.com/LixiangZhao98/PointCloud-Visualization-Tool/blob/master/Assets/pic/Flocculentcube2.png "Image")
\ The size of pointcloud data can be set by:
```c#
RenderDataRunTime.visSize=20f;
```
The binary files are stored in `Asset/PointCloud-Visualization-Tool/data/data` folder. They stores x,y,z coordinates for each point in binary. Each coordinate is stored in single (32bits) format. \
Here is a full review of the `binary data included in this repo` ([Point Cloud Data](https://github.com/LixiangZhao98/PointCloud-Visualization-Tool/blob/master/Assets/files/Data.pdf "Data")).
### Load data by point positions and visualize
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
            Vector3 pos = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
            v[i] = pos;
            i++;
        }
        return v;
    }
```






