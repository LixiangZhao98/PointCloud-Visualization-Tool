# PointCloud Visualization Tool

Kernel density estimation and interactive visualization toolkit for point cloud data in [Unity](https://unity.com/download).

The project can import, render, process, and export point clouds, including GPU-based kernel density estimation (KDE), Marching Cubes iso-surface reconstruction, density color mapping, and halo visualization.

Pull requests and issues are welcome. For questions about the project or datasets, contact Lixiang Zhao at `Lixiang.Zhao17@student.xjtlu.edu.cn`.

![Point cloud examples](https://raw.githubusercontent.com/LixiangZhao98/asset/master/Project/PointCloud-Visualization-Tool/pic/PointClouds.png)

## Features

- Import and visualize point cloud data from `.bin`, `.ply`, `.pcd`, `.txt`, and `.csv` files.
- Export point clouds to `.bin`, `.ply`, `.pcd`, and `.txt`.
- Generate point clouds from custom mathematical functions.
- Estimate point cloud density on the GPU with KDE.
- Reconstruct iso-surfaces with the Marching Cubes algorithm.
- Color-code point clouds by density.
- Render depth-dependent halo effects for point cloud visualization.

## Requirements

- Recommended Unity version: `2022.3.36f1` LTS.
- Older Unity versions `>= 2019` may work, but the current project settings were saved with Unity `2022.3.36f1`.
- A GPU with compute shader support is recommended for KDE and Marching Cubes demos.

## Quick Start

1. Clone this repository or download the [ZIP archive](https://github.com/LixiangZhao98/PointCloud-Visualization-Tool/archive/refs/heads/master.zip).

   ```bash
   git clone git@github.com:LixiangZhao98/PointCloud-Visualization-Tool.git
   ```

2. Open the project folder in Unity `2022.3.36f1` LTS or a compatible Unity version.
3. Open `Assets/PointCloud-Visualization-Tool/scenes/PointCloudVisualization.unity`.
4. Press Play.
5. Select the `DataObject` in the Hierarchy and choose a dataset from `Dataset in Project`.

To use the point cloud prefab in your own scene, drag `Assets/PointCloud-Visualization-Tool/prefab/DataObject.prefab` into the scene.

If you are new to Unity, see sections 1-4 of this [Unity setup tutorial](https://raw.githubusercontent.com/LixiangZhao98/asset/master/Tutorial/Unity_Setup_General.pdf), then section 6 for opening a project.

## Demos

### Read and Visualize Data

Scene: `Assets/PointCloud-Visualization-Tool/scenes/PointCloudVisualization.unity`

- Select `DataObject` in the Hierarchy.
- Use `Dataset in Project` to switch between data files in the project.
- Enable `Use_Math_Expressed_Dataset` to generate data from a function in `DataGenerator.cs`.
- Use the `Dataset` dropdown to choose the generated dataset.

### Kernel Density Estimation

Scene: `Assets/PointCloud-Visualization-Tool/scenes/KernelDensityEstimation.unity`

This demo estimates point cloud density, reconstructs an iso-surface with Marching Cubes, and maps density from blue (low) to red (high).

To adjust the Marching Cubes threshold, expand `DataObject` in the Hierarchy, select `MarchingCube`, and edit `Mc Threshold` in the Inspector.

![KDE result](https://raw.githubusercontent.com/LixiangZhao98/asset/master/Project/PointCloud-Visualization-Tool/pic/KDE.png)

Volume rendering examples at grid resolutions 128, 256, and 512:

<p>
  <img src="https://raw.githubusercontent.com/LixiangZhao98/asset/master/Project/PointCloud-Visualization-Tool/pic/VolumeRenderingDF128.png" alt="Volume rendering 128" width="32%">
  <img src="https://raw.githubusercontent.com/LixiangZhao98/asset/master/Project/PointCloud-Visualization-Tool/pic/VolumeRenderingDF256.png" alt="Volume rendering 256" width="32%">
  <img src="https://raw.githubusercontent.com/LixiangZhao98/asset/master/Project/PointCloud-Visualization-Tool/pic/VolumeRenderingDF512.png" alt="Volume rendering 512" width="32%">
</p>

### Halo Visualization

Scene: `Assets/PointCloud-Visualization-Tool/scenes/Halo.unity`

This demo replicates depth-dependent halo visualization in Unity, based on [Depth-Dependent Halos](https://ieeexplore.ieee.org/document/5290742).

<p>
  <img src="https://raw.githubusercontent.com/LixiangZhao98/asset/master/Project/PointCloud-Visualization-Tool/pic/ColorHalo.png" alt="Color halo" width="60%">
  <img src="https://raw.githubusercontent.com/LixiangZhao98/asset/master/Project/PointCloud-Visualization-Tool/pic/statuette.png" alt="Statuette halo" width="36%">
</p>

## Point Cloud Data

Place point cloud files in:

```text
Assets/PointCloud-Visualization-Tool/data/data
```

Unity updates the `Dataset in Project` dropdown from the files in this folder.

Supported import formats:

- `.bin`: raw 32-bit floating-point values stored as `x, y, z` triples.
- `.ply`: ASCII PLY and binary big-endian PLY vertex positions.
- `.pcd`: ASCII and binary PCD files with `x y z` fields.
- `.txt`: one point per line, with `x y z` separated by spaces, tabs, or commas.
- `.csv`: loaded through the CSV loader.

Supported export formats from the `DataObject` Inspector:

- `.bin`
- `.ply`
- `.pcd`
- `.txt`

Additional datasets are available in the [Pointcloud Dataset](https://github.com/LixiangZhao98/Pointcloud-Dataset) repository.

## Custom Mathematical Data

To generate point clouds from a mathematical function:

1. Open `Assets/PointCloud-Visualization-Tool/script/dataprocessing/DataGenerator.cs`.
2. Add a public method that returns `Vector3[]`.

   ```csharp
   public Vector3[] MyDataset()
   {
       Vector3[] points = new Vector3[1000];
       // Fill points here.
       return points;
   }
   ```

3. Save the script and return to Unity.
4. Select `DataObject`.
5. Enable `Use_Math_Expressed_Dataset`.
6. Select your method from the `Dataset` dropdown.

## Project Layout

```text
Assets/PointCloud-Visualization-Tool/
  data/       Sample point cloud data and generated data
  material/   Materials used by the renderers
  prefab/     Reusable DataObject prefab
  scenes/     Demo scenes
  script/     Runtime, rendering, data processing, and editor scripts
  shader/     Rendering, KDE, and Marching Cubes shaders
```

## Changelog

- `2025-01-12`: Fixed KDE shared group memory computation on machines where it could fail.
- `2025-01-16`: Enabled `.pcd`, `.ply`, and `.txt` import/export.
- `2025-01-23`: Added editor rendering support.

## Projects Built With This Repository

- [MeTACAST](https://github.com/LixiangZhao98/MeTACAST)

## Acknowledgements

Many thanks to the authors of [unity-marching-cubes-gpu](https://github.com/pavelkouril/unity-marching-cubes-gpu).

## License

This project is licensed under the [MIT License](LICENSE).
