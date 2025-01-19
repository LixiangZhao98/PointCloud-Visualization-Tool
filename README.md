PointCloud-Visualization-Tool
======


Kernel density estimation algorithm for point cloud visualization in [Unity3D](https://unity3d.com/get-unity/download "Unity download").\
Any pull requests and issues are welcome. If you have any questions about the project or the data, please feel free to email me (Lixiang.Zhao17@student.xjtlu.edu.cn).

# Features
- Import/export and visualize the point cloud data in bin/ply/pcd/txt format
- Define your point cloud data with mathematical equation easily
- Kernal Density Estimation (KDE) of the point cloud density on GPU
- Iso-surface construction with Marching-Cube algorithm
- Color-coded based on point cloud density
- Halo Visualization for point cloud data

# Requirement
Unity version >=2019

# Version Update Info
- 2025/1/12: Fix bug of KDE computation with shared group memory (the program will be broke on some machine)
- 2025/1/16: Enable import/export pcd/ply/txt files


# How to use

## Install the project
- Clone the repo using command following, or download the [archive](https://github.com/LixiangZhao98/PointCloud-Visualization-Tool/archive/refs/heads/master.zip "archive") directly
```bash
git clone git@github.com:LixiangZhao98/PointCloud-Visualization-Tool.git
```
- Open the project using Unity (versions >= 2019). If you are new to Unity, refer to sec.1-4 in [tutorial](https://raw.githubusercontent.com/LixiangZhao98/asset/master/Tutorial/Unity_Setup_General.pdf) for Unity setup and sec.6 to open a project.
- Drag the DataObject prefab `Assets\PointCloud-Visualization-Tool\Prefab\DataObject.prefab` into your scene.

## Demo1: Read and visualize data
- Run the demo in `Assets/PointCloud-Visualization-Tool/Scenes/PointCloudVisualization.unity`
- To switch the dataset, click the DataObject in hierarchy and change variable `datasets in project` in the inspector window. 
- Enable `Use_Function_Defined_Yourself` to use the function defined by yourself to generate the data.
- To add new data files or write mathematical equations of data, please refer to the Data section in the following.
![Image](https://github.com/LixiangZhao98/asset/blob/master/Project/PointCloud-Visualization-Tool/pic/PointClouds.png "Image")

## Demo2: Kernel Density Estimation
- Run the demo in `Assets/PointCloud-Visualization-Tool/Scenes/KernelDensityEstimation.unity`
- The density estimation results are shown by iso-surface reconstruction (MarchingCube) and color encoding from blue (low density) to red (high density).
- To change MarchingCube threshold, unfold the DataObject in hierarchy, click `MarchingCube` and adjust the variable `MC Threshold` in the inspector window.
![Image](https://github.com/LixiangZhao98/asset/blob/master/Project/PointCloud-Visualization-Tool/pic/KDE.png "Image")

## Demo3: Halo visualization
- Run the demo in `Assets/PointCloud-Visualization-Tool/Scenes/Halo.unity`
- This is a replication of halo visualization ([10.1109/TVCG.2009.138](https://ieeexplore.ieee.org/document/5290742 "Depth-Dependent Halos")) in Unity 


<div style="display: flex; justify-content: space-between; align-items: center;">

  <img src="https://github.com/LixiangZhao98/asset/blob/master/Project/PointCloud-Visualization-Tool/pic/ColorHalo.png" alt="Image 1" style="width: 60%;"/>
  <img src="https://github.com/LixiangZhao98/asset/blob/master/Project/PointCloud-Visualization-Tool/pic/statuette.png" alt="Image 2" style="width: 36%;"/>

</div>


# Data
- The repo supports to read bin/ply/pcd/txt data files. To add data files, you just need to place it to `Assets\PointCloud-Visualization-Tool\data\data`, and the project identifies the file automatically.
- To write your own mathematical equation of data, you need (1) go to `Assets\PointCloud-Visualization-Tool\script\dataprocessing\DataGenerator.cs`, (2) add a new function with an output type of `Vector3[]` (for instance, static public CubicArea(){}), (3) enable `Use_Function_Defined_Yourself` and then you can find CubicArea in Drop-down box `Customized Dataset`.
- Refer to [Pointcloud Dataset](https://github.com/LixiangZhao98/Pointcloud-Dataset) for more data.
- If you want to use the .bin data outside this project, first you need to convert them to `single-precision floating-point` format. Three single-precision floats consist a 3D coordinate of one point.

# Projects built based on this repo
[MeTACAST](https://github.com/LixiangZhao98/MeTACAST "MeTACAST")

[//]: # (- The .ply files can be downloaded from [https://graphics.stanford.edu/data/3Dscanrep/]&#40;https://graphics.stanford.edu/data/3Dscanrep/&#41;. The .bin files can be downloaded from the repo &#40;TODO&#41;)

[//]: # (# Scripting)

[//]: # ()
[//]: # (The following are all in `Assets/PointCloud-Visualization-Tool/Scenes/MyPointCloud.unity`.)

[//]: # (  )
[//]: # (## Load data from binary files)

[//]: # (- Add `RenderDataRunTime` to an empty GameObject &#40;you can name it whatever you like, here we call it "Runtime"&#41;.)

[//]: # (- Create a new script &#40;you can name it whatever you like, here we call it `MyPointCloud.cs`&#41; and add it to GameObject "Runtime".)

[//]: # (- In `MyPointCloud.cs`, We first initialize two varaibles `particleMat` and `visCenter`. Remember to assign these two variables in the inspector.)

[//]: # (```c#)

[//]: # (public Material particleMat;  // the material of the points)

[//]: # (public GameObject visCenter; //The visualization will always follow this GameObject when starting the game. )

[//]: # (``` )

[//]: # (- To load data from binary files, we can simply call `DataMemory.LoadDataByByte&#40;fileName&#41;`. An example code is as follows:)

[//]: # (```c#)

[//]: # (public class MyPointCloud : MonoBehaviour)

[//]: # ({)

[//]: # (    public Material particleMat;  // the material of the points)

[//]: # (    public GameObject visCenter; //The visualization will always follow the `Vis center` when starting the game. )

[//]: # (    void Start&#40;&#41;)

[//]: # (    {)

[//]: # (        DataStorage.StacksInitialize&#40;&#41;;  //Initialize)

[//]: # (        DataStorage.LoadByte&#40;"Flocculentcube2"&#41;;  //load the data from the the binary file; the input is the name of the binary file)

[//]: # (        RenderDataRunTime.visSize = 1f;  //the size of the visualization)

[//]: # (        RenderDataRunTime.Init&#40;visCenter, particleMat&#41;;  // Assign materials and center to the RenderDataRunTime.cs`)

[//]: # (    })

[//]: # (})

[//]: # (```)

[//]: # (![Image]&#40;https://github.com/LixiangZhao98/asset/blob/master/Project/PointCloud-Visualization-Tool/pic/LoadBinary.png "Image"&#41;)

[//]: # (- The data files stores x,y,z coordinates in binary format located in `Asset/PointCloud-Visualization-Tool/data/data` folder. To use them, you need to convert the binary sequence to single-precision floating-point &#40;32bits&#41; sequence. Then, the 1st, 2nd, and 3rd floats are the x,y, and z coordinates for the first point. The 4th, 5th and 6th floats are the x,y, and z coordinates for the second point...  Here is a full list of the [Point Cloud Dataset]&#40;https://raw.githubusercontent.com/LixiangZhao98/asset/master/Project/PointCloud-Visualization-Tool/files/Data.pdf "Data"&#41;. Some are not in this repo. If you need them, please feel free to email me.)

[//]: # ()
[//]: # (## Load data by point positions)

[//]: # (- To load data by point positions, we can build a `Vector3[] vector3Array` and call `DataMemory.LoadDataByVec3s&#40;vector3Array,name&#41;`. An example to generate a group of points in a cubic range is as follows:)

[//]: # (```c#)

[//]: # (public class MyPointCloud : MonoBehaviour)

[//]: # ({)

[//]: # (    public Material particleMat;  // the material of the points)

[//]: # (    public GameObject visCenter; //The visualization will always follow the `Vis center` when starting the game. )

[//]: # (    void Start&#40;&#41;)

[//]: # (    {)

[//]: # (        Vector3[] v = Generate_Cube&#40;&#41;;  // Generate random points in Cubic shape)

[//]: # (        DataStorage.StacksInitialize&#40;&#41;;//Initialize)

[//]: # (        DataStorage.LoadVec3s&#40;v, "cube"&#41;;  // the first input is Vector[], the second is the name of the data &#40;you can name it as you like&#41;)

[//]: # (        RenderDataRunTime.visSize = 1f;  //the size of the visualization)

[//]: # (        RenderDataRunTime.Init&#40;visCenter,particleMat&#41;;  // Assign materials and center to the RenderDataRunTime.cs`)

[//]: # (    })

[//]: # (    public Vector3[] Generate_Cube&#40;&#41;  // Generate random points in Cubic shape)

[//]: # (    {)

[//]: # (        Random.InitState&#40;2&#41;;)

[//]: # (        int num = 100000;)

[//]: # (        int i = 0;)

[//]: # (        Vector3[] v = new Vector3[num];)

[//]: # (        while &#40;i < num&#41;)

[//]: # (        {)

[//]: # (            v[i] = new Vector3&#40;Random.Range&#40;-1.0f, 1.0f&#41;, Random.Range&#40;-1.0f, 1.0f&#41;, Random.Range&#40;-1.0f, 1.0f&#41;&#41;;)

[//]: # (            i++;)

[//]: # (        })

[//]: # (        return v;)

[//]: # (    })

[//]: # (})

[//]: # (```)

[//]: # (![Image]&#40;https://github.com/LixiangZhao98/asset/blob/master/Project/PointCloud-Visualization-Tool/pic/LoadVec3s.png "Image"&#41;)

[//]: # ()
[//]: # (## Load data from ply files)

[//]: # ()
[//]: # (```c#)

[//]: # (public class MyPointCloud : MonoBehaviour)

[//]: # ({)

[//]: # (    public Material particleMat;  // the material of the points)

[//]: # (    public GameObject visCenter; //The visualization will always follow the `Vis center` when starting the game. )

[//]: # (    void Start&#40;&#41;)

[//]: # (    {)

[//]: # (        DataStorage.StacksInitialize&#40;&#41;;  //Initialize)

[//]: # (        DataStorage.LoadPly&#40;"dragon_vrip"&#41;;  //load the data from the the ply file; the input is the name of the binary file)

[//]: # (        RenderDataRunTime.visSize = 1f;)

[//]: # (        RenderDataRunTime.Init&#40;visCenter, particleMat&#41;;  // Assign materials and center to the RenderDataRunTime.cs`)

[//]: # (    })

[//]: # (})

[//]: # (```)

[//]: # (![Image]&#40;https://github.com/LixiangZhao98/asset/blob/master/Project/PointCloud-Visualization-Tool/pic/LoadPly.png "Image"&#41;)

[//]: # (### Traversal of points)

[//]: # (To get information of each point, such as the position. We can simply do as following:)

[//]: # (```c#)

[//]: # (void Start&#40;&#41;)

[//]: # ({)

[//]: # (for&#40;int i=0;i<DataMemory.allParticle.GetParticlenum&#40;&#41;;i++&#41;)

[//]: # ({)

[//]: # (    Debug.Log&#40;DataMemory.allParticle.GetParticlePosition&#40;i&#41;&#41;;)

[//]: # (})

[//]: # (})

[//]: # (```)

# Thanks
Many thanks to the authors of open-source repository:
[unity-marching-cubes-gpu](https://github.com/pavelkouril/unity-marching-cubes-gpu "unity-marching-cubes-gpu")





