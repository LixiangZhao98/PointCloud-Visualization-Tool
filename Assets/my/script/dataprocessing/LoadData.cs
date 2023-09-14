using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class LoadDataBybyte 
{

    static public Vector3[] StartLoad(string filename)
    {
        return FloatsToVec3s(BytesToFloats(FileToBytes(filename)));
    }
    static byte[] FileToBytes(string filename)
    {
        try
        {
            using (FileStream fs=new FileStream(filename,FileMode.Open,FileAccess.Read))
            {
                byte[] byteArray = new byte[fs.Length];
                fs.Read(byteArray, 0, byteArray.Length);
                return byteArray;
            }
        }
        catch(Exception e)
        {Debug.LogException(e);
            Debug.LogError("load fail");
            return null;
        }
    }

    static float[] BytesToFloats(byte[] bs)
    {
        float[] floatArray = new float[bs.Length / sizeof(float)];
        for(int i=0;i< floatArray.Length;i++)
        {
            byte[] byteArray = new byte[sizeof(float)];
            for(int j=0;j<sizeof(float);j++)
            {
                byteArray[j] = bs[i * sizeof(float) + j];
            }
            floatArray[i] = BitConverter.ToSingle(byteArray, 0);
      
         
        }
      
        return floatArray;
    }
    static Vector3[] FloatsToVec3s(float[] fs)
    {
        Vector3[] vectorArray = new Vector3[fs.Length/3];
        for (int i = 0; i < vectorArray.Length; i++)
        {
            vectorArray[i]= new Vector3(fs[i*3], fs[i*3+1], fs[i*3+2]);
        }
      
        return vectorArray;
    }


    //load flag
    static public int[] StartLoadFlags(string filename)
    {
        return BytesToInts(FileToBytes(filename));
    }

    static int[] BytesToInts(byte[] bs)
    {
        int[] intArray = new int[bs.Length / sizeof(int)];
        for (int i = 0; i < intArray.Length; i++)
        {
            byte[] byteArray = new byte[sizeof(int)];
            for (int j = 0; j < sizeof(int); j++)
            {
                byteArray[j] = bs[i * sizeof(int) + j];
            }
            intArray[i] = BitConverter.ToInt32 (byteArray, 0);


        }

        return intArray;
    }


}

public class csvController
{

    static csvController csv;
    public List<string[]> arrayData;

    private csvController()   //单例，构造方法为私有
    {
        arrayData = new List<string[]>();
    }

    public static csvController GetInstance()   //单例方法获取对象
    {
        if (csv == null)
        {
            csv = new csvController();
        }
        return csv;
    }

    public int loadFile(string fileName)
    {
        arrayData.Clear();
        StreamReader sr = null;
        try
        {
            string file_url =fileName;    //根据路径打开文件
            sr = File.OpenText(file_url);
            Debug.Log("File Find in " + file_url);
        }
        catch
        {
            Debug.Log("File cannot find ! ");
            return 0;
        }

        string line;
        int count = 0;
        while ((line = sr.ReadLine()) != null)   //按行读取
        {
            count++;
            arrayData.Add(line.Split(','));   //每行逗号分隔,split()方法返回 string[]
        }
        sr.Close();
        sr.Dispose();
        return count;
    }

    public string getString(int row, int col)
    {
        return arrayData[row][col];
    }
    public int getInt(int row, int col)
    {
        return int.Parse(arrayData[row][col]);
    }
    public  float getFloat(int row, int col)
    {
        return float.Parse(arrayData[row][col]);
    }

   public Vector3[] StartLoad(string filename)
    {
        //csvController加载csv文件，单例模式，这个类只有一个对象，这个对象只能加载一个csv文件
       int count= csvController.GetInstance().loadFile(filename);
        Vector3[] vs = new Vector3[count];
        //根据索引读取csvController中的list（csv文件的内容）数据
        for (int i=1;i<count;i++)
        {
            vs[i - 1] = new Vector3(csvController.GetInstance().getFloat(i,1), csvController.GetInstance().getFloat(i, 2), csvController.GetInstance().getFloat(i, 3));
        }
        return vs;
    }



    public void WriteCsv(string[] strs, string path)
    {
        if (!File.Exists(path))
        {
            File.Create(path).Dispose();
        }
        //UTF-8方式保存
        using (StreamWriter stream = new StreamWriter(path, false, Encoding.UTF8))
        {
            for (int i = 0; i < strs.Length; i++)
            {
                if (strs[i] != null)
                    stream.WriteLine(strs[i]);
            }
        }
    }
}
