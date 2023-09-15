using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityVolumeRendering;

public struct pbdInfo
{
    public string formatKey;
    public Endianness endianness;
    public int sizeX;
    public int sizeY;
    public int sizeZ;
    public int bytesToSkip;
}
public class PbdDecompresser 
{

    public readonly string pbdFormatKey = "v3d_volume_pkbitdf_encod";
    public readonly string v3drawFormatKey = "raw_image_stack_by_hpeng";
    public Nullable<pbdInfo> readHeader(byte[] imageData)
    {
        byte[] stringByteArray = imageData.Take(24).ToArray();
        string readedFormatKey = System.Text.Encoding.UTF8.GetString(stringByteArray);

        if (readedFormatKey != pbdFormatKey)
        {
            Debug.LogError("This format is not supported");
            return null;
        }

        string endiean = Convert.ToChar(imageData[24]).ToString();
        Endianness endianness = Endianness.LittleEndian;
        if (endiean == "L")
        {
            endianness = Endianness.LittleEndian;
        }else if (endiean == "L")
        {
            endianness = Endianness.BigEndian;
        }
        else
        {
            Debug.LogError("this endianness is not supported");
        }

        DataContentFormat dataFormat = DataContentFormat.Uint8;
        switch (imageData[25])
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
            byte[] subArray = imageData.SubArray(offset,length);
            Int32 number = BitConverter.ToInt32(subArray,0);
            size[i] = number;
        }

        pbdInfo info = new pbdInfo();
        info.formatKey = readedFormatKey;
        info.endianness = endianness;
        info.sizeX = size[0];
        info.sizeY = size[0];
        info.sizeZ = size[2];
        info.bytesToSkip = 43;
        return info;
    }

    public byte[] decompressPbdBytes(pbdInfo info,byte[] imageData)
    {
        int cp = 0;
        int dp = 0;
        byte mask = 0x03;
        byte p0, p1, p2, p3;
        byte value;
        byte pva, pvb;
        byte sourceChar;
        byte pointer = 0;
        int pbdPointer = info.bytesToSkip;

        byte[] v3drawBuffer = new byte[43 + info.sizeX * info.sizeY * info.sizeZ];
        int v3drawPointer = 43;
        byte v3drawPointerValue = 0;

        byte[] formatKeyBuffer = Encoding.UTF8.GetBytes(v3drawFormatKey);
        for (int i = 0; i < 24; i++)
        {
            v3drawBuffer[i] = formatKeyBuffer[i];
        }
        
        // copy header
        for (int i = 24; i < info.bytesToSkip; i++)
        {
            v3drawBuffer[i] = imageData[i];
        }
        
        while (cp < imageData.Length - 43)
        {
            value = imageData[pbdPointer + cp];

            if (value < 33)
            {
                int count = value + 1;
                for (int j = cp+1; j <= cp+count; j++)
                {
                    v3drawBuffer[v3drawPointer + dp] = imageData[j + pbdPointer];
                    dp += 1;
                }
                cp += count + 1;
                v3drawPointerValue = v3drawBuffer[dp - 1 + v3drawPointer];
            }else if (value < 128)
            {
                int leftToFill = value - 32;
                while (leftToFill > 0)
                {
                    int fillNumber = Math.Min(leftToFill, 4);
                    cp += 1;
                    sourceChar = imageData[cp + pbdPointer];
                    int toFill = v3drawPointer + dp;
                    p0 = (byte)(sourceChar & mask);
                    sourceChar = (byte)(sourceChar >> 2);
                    p1 = (byte)(sourceChar & mask);
                    sourceChar = (byte)(sourceChar >> 2);
                    p2 = (byte)(sourceChar & mask);
                    sourceChar = (byte)(sourceChar >> 2);
                    p3 = (byte)(sourceChar & mask);

                    pva = handleDifference(p0, v3drawPointerValue);

                    v3drawBuffer[toFill] = pva;
                    if (fillNumber > 1)
                    {
                        toFill += 1;
                        pvb = handleDifference(p1, pva);
                        v3drawBuffer[toFill] = pvb;
                        if (fillNumber > 2)
                        {
                            toFill += 1;
                            pva = handleDifference(p2, pvb);
                            v3drawBuffer[toFill] = pva;
                            if (fillNumber > 3)
                            {
                                toFill += 1;
                                v3drawBuffer[toFill] = handleDifference(p3, pva);
                            }
                        }
                    }

                    v3drawPointerValue = v3drawBuffer[toFill];
                    dp += (int)fillNumber;
                    leftToFill -= fillNumber;
                }

                cp += 1;
            }
            else
            {
                int repestCount = value - 127;
                cp += 1;
                byte repeatValue = imageData[cp + pbdPointer];
                for (int i = 0; i < repestCount; i++)
                {
                    v3drawBuffer[v3drawPointer + dp] = repeatValue;
                    dp += 1;
                }

                v3drawPointerValue = repeatValue;
                cp += 1;
            }
        }
        return v3drawBuffer;
    }


    private byte handleDifference(byte diff,byte value)
    {
        if (diff == 3)
        {
            if (value == 0)
            {
                return (byte)0;
            }
            else
            {
                return (byte)(value - 1);
            }
        }
        else
        {
            if ((int)diff + (value) > 255)
            {
                return (byte)0;
            }
            else
            {
                return (byte)(diff + value);
            }
        } 
    }
    
    
}
