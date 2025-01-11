using System;
using UnityEngine;
using UnityEditor;
using System.IO;

public class ExportTexture3DAsset : MonoBehaviour
{
    private Texture3D texture3D;

    [ContextMenu("Export Texture3D as Asset")]
    private void Export()
    {
        GPUKDECsHelper GCH = this.transform.GetComponentInChildren<GPUKDECsHelper>();
        if (GCH== null)
        {
            Debug.LogError("Density is not estimated.");
            return;
        }
        texture3D = new Texture3D(GCH.densityField.XNUM,GCH.densityField.YNUM, GCH.densityField.ZNUM, TextureFormat.RFloat, false);
        texture3D.wrapMode = TextureWrapMode.Clamp;
        Color []colors_den = new Color[GCH.densityField.XNUM * GCH.densityField.XNUM * GCH.densityField.ZNUM];
        for (int i = 0; i < colors_den.Length; i++) colors_den[i] = Color.black;

        var idx = 0;
        for (var z = 0; z < GCH.densityField.ZNUM; z++)
        {
            for (var y = 0; y < GCH.densityField.YNUM; y++)
            {
                for (var x = 0; x < GCH.densityField.XNUM; x++, idx++)
                {
                    colors_den[idx].r = (float)GCH.densityField.GetNodeDensity(idx);
                }
            }
        }
        texture3D.SetPixels(colors_den);
        texture3D.Apply();
        
        string directoryPath = "Assets/PointCloud-Visualization-Tool/FieldTexture3D";
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        string assetPath = AssetDatabase.GenerateUniqueAssetPath(directoryPath + "/"+this.transform.GetComponentInChildren<DataLoader>().particles.name+"_"+this.transform.GetComponentInChildren<GPUKDECsHelper>().gridNum +".asset");

        Texture3D textureCopy = new Texture3D(texture3D.width, texture3D.height, texture3D.depth, texture3D.format, texture3D.mipmapCount > 1);
        textureCopy.SetPixels(texture3D.GetPixels());
        textureCopy.Apply();

        // AssetDatabase.CreateAsset(textureCopy, assetPath);
        // AssetDatabase.SaveAssets();

        // Debug.Log("Export completed. Asset saved at: " + assetPath);
        
        FileStream fs = new FileStream("Assets/PointCloud-Visualization-Tool/FieldTexture3D/"+this.transform.GetComponentInChildren<DataLoader>().particles.name+".raw", FileMode.Create, FileAccess.Write);
        byte[] byteArray = new byte[sizeof(UInt32)*this.transform.GetComponentInChildren<GPUKDECsHelper>().densityField.GetNodeNum()];
        for (int i = 0; i < this.transform.GetComponentInChildren<GPUKDECsHelper>().densityField.GetNodeNum(); i++)
        {
            byte[] bytex = System.BitConverter.GetBytes((UInt32)this.transform.GetComponentInChildren<GPUKDECsHelper>().densityField.GetNodeDensity(i));
            bytex.CopyTo(byteArray, sizeof(UInt32)* i);
        }
        fs.Write(byteArray, 0, byteArray.Length);
        Debug.Log(".raw export completed.");
    }
}