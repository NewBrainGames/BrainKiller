using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Linq;

namespace UnityVolumeRendering
{
    /// <summary>
    /// Editor window for importing datasets.
    /// </summary>

    public static class Extensions
    {
        public static T[] SubArray<T>(this T[] array, int offset, int length)
        {
            T[] result = new T[length];
            Array.Copy(array, offset, result, 0, length);
            return result;
        }
    }

    public class V3DRawDatasetImporterEditorWindow : EditorWindow
    {
        private string fileToImport;
        private int dimX;
        private int dimY;
        private int dimZ;
        private int bytesToSkip = 0;
        private DataContentFormat dataFormat = DataContentFormat.Uint8;
        private Endianness endianness = Endianness.LittleEndian;

        private int totalHeaderBytes = 43;

        public V3DRawDatasetImporterEditorWindow(string filePath)
        {
            fileToImport = filePath;

            if (Path.GetExtension(fileToImport) == ".ini")
                fileToImport = fileToImport.Replace(".ini", ".raw");

            // Try parse ini file (if available)
            DatasetIniData initData = DatasetIniReader.ParseIniFile(fileToImport + ".ini");
            if (initData != null)
            {
                dimX = initData.dimX;
                dimY = initData.dimY;
                dimZ = initData.dimZ;
                bytesToSkip = initData.bytesToSkip;
                dataFormat = initData.format;
                endianness = initData.endianness;
            }

            string formatKey = "raw_image_stack_by_hpeng";
            byte[] byteArray = File.ReadAllBytes(fileToImport);
            byte[] stringByteArray = byteArray.Take(24).ToArray();
            string readFormatKey = System.Text.Encoding.UTF8.GetString(stringByteArray);
            Debug.Log("reading header...");
            if (readFormatKey != formatKey)
            {
                Debug.Log(readFormatKey);
                Debug.LogError("This format is not supported");
            }
            string end = Convert.ToChar(byteArray[25]).ToString();
            if (end == "L"){
                endianness = Endianness.LittleEndian;
            }else if(end == "B"){
                endianness = Endianness.BigEndian;
            }else{
               Debug.Log(end);
               Debug.LogError("This endianness is not supported"); 
            }
            // byte dataType = [25];
            switch (byteArray[25])
            {
                case 1:
                    dataFormat = DataContentFormat.Uint8;
                    break;
                case 2:
                    dataFormat = DataContentFormat.Uint16;
                    break;
                default:
                    Debug.Log("unsupported data type");
                    break;
            }
            Int32[] size = new Int32[4];
            for (int i = 0; i < 4; i++)
            {
                int offset = 27+i*4;
                int length = 4;
                byte[] subArray = byteArray.SubArray(offset,length);
                // if (BitConverter.IsLittleEndian)
                    // Array.Reverse(subArray);
                Int32 number = BitConverter.ToInt32(subArray,0);
                size[i] = number;
            }
            dimX = size[0];
            dimY = size[1];
            dimZ = size[2];
            Debug.Log(readFormatKey);
            Debug.Log(end);
            Debug.Log(size);
            this.minSize = new Vector2(300.0f, 200.0f);
        }

        private void ImportDataset()
        {
            V3DRawDatasetImporter importer = new V3DRawDatasetImporter(fileToImport, dimX, dimY, dimZ, dataFormat, endianness, totalHeaderBytes);
            VolumeDataset dataset = importer.Import();

            if (dataset != null)
            {
                if (EditorPrefs.GetBool("DownscaleDatasetPrompt"))
                {
                    if (EditorUtility.DisplayDialog("Optional DownScaling",
                        $"Do you want to downscale the dataset? The dataset's dimension is: {dataset.dimX} x {dataset.dimY} x {dataset.dimZ}", "Yes", "No"))
                    {
                        dataset.DownScaleData();
                    }
                }
                VolumeRenderedObject obj = VolumeObjectFactory.CreateObject(dataset);
            }
            else
            {
                Debug.LogError("Failed to import datset");
            }

            this.Close();
        }

        private void OnGUI()
        {
            dimX = EditorGUILayout.IntField("X dimension", dimX);
            dimY = EditorGUILayout.IntField("Y dimension", dimY);
            dimZ = EditorGUILayout.IntField("Z dimension", dimZ);
            bytesToSkip = EditorGUILayout.IntField("Bytes to skip", totalHeaderBytes);
            dataFormat = (DataContentFormat)EditorGUILayout.EnumPopup("Data format", dataFormat);
            endianness = (Endianness)EditorGUILayout.EnumPopup("Endianness", endianness);

            if (GUILayout.Button("Import"))
                ImportDataset();

            if (GUILayout.Button("Cancel"))
                this.Close();
        }
    }
}

