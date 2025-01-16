using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class LoadData 
{
    

    
    static public Vector3[] LoadPly(string filename)
    {
        List<Vector3> pointList = new List<Vector3>();
        try
          {
            if (!File.Exists(filename))
            {
                Debug.LogError("file does not exist: " + filename);
                return null;
            }

            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                using (BinaryReader br = new BinaryReader(fs,System.Text. Encoding.ASCII))
                {
                    int vertexCount = 0;
                    bool isBinary = false;

                    // Read and parse the header
                    int headerCount = 0;
                    while (true)
                    {
                        string line = ReadLine(br);
                        headerCount++;
                        if (line.StartsWith("format"))
                        {
                            if (line.Contains("binary_big_endian 1.0"))
                                isBinary = true;
                            else if (line.Contains("ascii"))
                                isBinary = false;
                            else
                                throw new System.Exception("Unsupported PLY format");
                        }
                        else if (line.StartsWith("element vertex"))
                        {
                            var tokens = line.Split(' ');
                            vertexCount = int.Parse(tokens[2]);
                        }
                        else if (line.StartsWith("end_header"))
                        {
                            break;
                        }

                        
                    }

                    if (isBinary)
                    {
                        // Process binary format
                        for (int i = 0; i < vertexCount; i++)
                        {
                            float x = ReadBigEndianFloat(br);
                            float y = ReadBigEndianFloat(br);
                            float z = ReadBigEndianFloat(br);
                            pointList.Add(new Vector3(x, y, z));
                        }
                    }
                    else
                    {
                        // Process ASCII format
                        br.BaseStream.Seek(0, SeekOrigin.Begin); // Reset stream to beginning
                        using (StreamReader sr = new StreamReader(fs, System.Text.Encoding.ASCII))
                        {
                            // Skip header lines
                            for (int i = 0; i < vertexCount + headerCount; i++)
                            {
                                string line = sr.ReadLine();
                                if (i >= headerCount) // Start reading vertices after header
                                {
                                    var tokens = line.Split(' ');
                                    float x = float.Parse(tokens[0]);
                                    float y = float.Parse(tokens[1]);
                                    float z = float.Parse(tokens[2]);
                                    pointList.Add(new Vector3(x, y, z));
                                }
                            }
                        }
                    }
                }
            }

            return pointList.ToArray();
          }
          catch (Exception e)
          {
              Debug.Log(e);
              return null;
          }
    }
 
    static public Vector3[] LoadPcd(string filename)
    {
        if (!File.Exists(filename))
        {
            throw new FileNotFoundException($"File not found: {filename}");
        }

        List<Vector3> points = new List<Vector3>();
        bool isAscii = false;
        bool isBinary = false;
        int pointCount = 0;
        int headerLength = 0;

        using (StreamReader reader = new StreamReader(filename))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                headerLength += line.Length + 1; // 加1是因为换行符
                line = line.Trim();

                if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                {
                    continue;
                }

                if (line.StartsWith("DATA ascii"))
                {
                    isAscii = true;
                    break;
                }
                else if (line.StartsWith("DATA binary"))
                {
                    isBinary = true;
                    break;
                }

                // 获取点的数量
                if (line.StartsWith("POINTS"))
                {
                    string[] parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2 && int.TryParse(parts[1], out int count))
                    {
                        pointCount = count;
                    }
                }
            }
        }

        using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
        {
            if (isAscii)
            {
                // ASCII 格式处理
                fs.Seek(headerLength, SeekOrigin.Begin);
                using (StreamReader reader = new StreamReader(fs))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                        if (parts.Length < 3)
                        {
                            continue;
                        }

                        if (float.TryParse(parts[0], out float x) &&
                            float.TryParse(parts[1], out float y) &&
                            float.TryParse(parts[2], out float z))
                        {
                            points.Add(new Vector3(x, y, z));
                        }
                    }
                }
            }
            else if (isBinary)
            {
                // Binary 格式处理
                fs.Seek(headerLength, SeekOrigin.Begin);
                byte[] buffer = new byte[pointCount * 12]; // 每个点包含3个float，每个float占4字节
                fs.Read(buffer, 0, buffer.Length);

                for (int i = 0; i < pointCount; i++)
                {
                    float x = BitConverter.ToSingle(buffer, i * 12);
                    float y = BitConverter.ToSingle(buffer, i * 12 + 4);
                    float z = BitConverter.ToSingle(buffer, i * 12 + 8);

                    points.Add(new Vector3(x, y, z));
                }
            }
            else
            {
                throw new FormatException("Unsupported PCD data format: Only ASCII or binary formats are supported.");
            }
        }

        return points.ToArray();
    }
    
    static public Vector3[] LoadBin(string filename)
    {
        return FloatsToVec3s(BytesToFloats(LoadBytes(filename)));
    }
    /// <summary>
    /// 从TXT文件加载点云数据
    /// </summary>
    /// <param name="filename">TXT文件路径</param>
    /// <returns>点云数据数组</returns>
    public static Vector3[] LoadTxt(string filename)
    {
        if (!File.Exists(filename))
        {
            throw new FileNotFoundException($"File not found: {filename}");
        }

        List<Vector3> points = new List<Vector3>();

        using (StreamReader reader = new StreamReader(filename))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                line = line.Trim();
                if (string.IsNullOrEmpty(line))
                {
                    continue; // 跳过空行
                }

                string[] parts = line.Split(new[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length < 3)
                {
                    continue; // 如果行不包含至少三个值，跳过
                }

                if (float.TryParse(parts[0], out float x) &&
                    float.TryParse(parts[1], out float y) &&
                    float.TryParse(parts[2], out float z))
                {
                    points.Add(new Vector3(x, y, z));
                }
            }
        }

        return points.ToArray();
    }

    static public int[] LoadFlags(string filename)
    {
        return BytesToInts(LoadBytes(filename));
    }
    
    
    
    
    
    
    static byte[] LoadBytes(string filename)
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
        {
            Debug.LogException(e);
            
            return new byte[0];
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
    
    static string ReadLine(BinaryReader br)
    {
        List<byte> byteList = new List<byte>();
        byte readByte;
        while ((readByte = br.ReadByte()) != '\n')
        {
            if (readByte != '\r') // Ignore carriage return if present
            {
                byteList.Add(readByte);
            }
        }
        return System.Text.Encoding.ASCII.GetString(byteList.ToArray());
    }
    static float ReadBigEndianFloat(BinaryReader br)
    {
        byte[] bytes = br.ReadBytes(4);
        System.Array.Reverse(bytes); // Convert to little endian
        return System.BitConverter.ToSingle(bytes, 0);
    }



}

public class csvController
{

    static csvController csv;
    public List<string[]> arrayData;

    private csvController()   
    {
        arrayData = new List<string[]>();
    }

    public static csvController GetInstance()   
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
            string file_url =fileName;    
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
        while ((line = sr.ReadLine()) != null)   
        {
            count++;
            arrayData.Add(line.Split(','));   
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
       int count= csvController.GetInstance().loadFile(filename);
        Vector3[] vs = new Vector3[count];
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
