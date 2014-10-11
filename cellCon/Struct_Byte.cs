using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace cellCon
{
	//以下为何丹写的，关于C# byte[]与struct的转换
	public static class Struct_Byte
	{
		//struct转换为byte[]
		public static byte[] StructToBytes(object structObj)
		{
			int size=Marshal.SizeOf(structObj);
			IntPtr buffer=Marshal.AllocHGlobal(size);
			try
			{
				Marshal.StructureToPtr(structObj,buffer,false);
				byte[] bytes=new byte[size];
				Marshal.Copy(buffer,bytes,0,size);
				return bytes;
			}
			finally
			{
				Marshal.FreeHGlobal(buffer);
			}
		}

		//byte[]转换为struct
		public static object BytesToStruct(byte[] bytes,Type strcutType)
		{
			int size=Marshal.SizeOf(strcutType);
			IntPtr buffer=Marshal.AllocHGlobal(size);
			try
			{
				Marshal.Copy(bytes, 0, buffer, size);
				return Marshal.PtrToStructure(buffer, strcutType);
			}
			catch 
			{
				return null;
			}
			finally
			{
				Marshal.FreeHGlobal(buffer);
			}
		}
		//为方便结构体数组的构造
		public static object BytesToStruct(byte[] bytes,int offset,Type strcutType)
		{
			int size=Marshal.SizeOf(strcutType);
			IntPtr buffer=Marshal.AllocHGlobal(size);
			try
			{
				Marshal.Copy(bytes, offset, buffer, size);
				return Marshal.PtrToStructure(buffer, strcutType);
			}
			catch 
			{
				return null;
			}
			finally
			{
				Marshal.FreeHGlobal(buffer);
			}
		}
	}
}
