using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cellCon
{
	static public class crc_ccitt
	{
		//计算CRC16，0x11021
		static UInt16[] crc_ta=	// CRC 余式表
		{
			0x0000,0x1021,0x2042,0x3063,0x4084,0x50a5,0x60c6,0x70e7,
			0x8108,0x9129,0xa14a,0xb16b,0xc18c,0xd1ad,0xe1ce,0xf1ef,
		};

		static public UInt16 cal_crc(byte[] ptr, int len)
		{
			UInt16 crc;
			byte da;
			int i=0;
			crc=0;
			while(len--!=0)
			{
				da=(byte)(((byte)(crc/256))/16);      //暂存 CRC 的高四位
				crc<<=4;                   // CRC 右移 4 位，相当于取 CRC 的低 12 位）
				crc^=crc_ta[da^(ptr[i]/16)]; // CRC 的高 4 位和本字节的前半字节相加后查表计算 CRC，
				//然后加上上一次 CRC 的余数
				da=(byte)(((byte)(crc/256))/16);  // 暂存 CRC 的高 4 位
				crc<<=4;                  //CRC 右移 4 位， 相当于 CRC 的低 12 位）
				crc^=crc_ta[da^(ptr[i]&0x0f)];// CRC 的高 4 位和本字节的后半字节相加后查表计算
				//CRC，然后再加上上一次 CRC 的余数 */
				i++;
			}
			return(crc);
		}
	}
}
