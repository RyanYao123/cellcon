using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.IO.Ports;

namespace cellCon
{
	public class flir_serial
	{
		public uart_dbg uart=new uart_dbg();
		public head组包 pack=new head组包();

		public int T { get; set; }//温度

		public flir_serial()
		{
			T=0;

			uart.data_rx+=new EventHandler(uart_data_rx);

			pack.SYNC=new byte[] { 0x6e };
			pack.pack_len=10;
			pack.pre_offset=3;
			pack.pre_cb=pre_cb;
			pack.pro=pro;
		}
		int pre_cb(byte[] b, int len)
		{
			if (b[3]==0x43)
			{
				return 12;
			}
			return 6;
		}
		bool pro(byte[] b, int len)
		{
			UInt16 t=crc_ccitt.cal_crc(b, 10);
			if(b[10]!=(byte)(t>>8)||b[11]!=(byte)(t))
			{
				return false;
			}
			T=b[8]*256+b[9];
			return true;
		}
		void uart_data_rx(object sender, EventArgs e)
		{
			byte[] buf=sender as byte[];
			for(int i=0;i<buf.Length;i++)
			{
				pack.rec_byte(buf[i]);
			}
		}
		public void start(int port)
		{
			uart.BaudRate=57600;
			uart.open(port);
		}
		public void stop()
		{
			uart.Close();
		}

#region 指令

		//设备使用大端存储
		public byte[] temp_buf=new byte[10]{0x6e,0,0,0x2a,0,0,0,0,0,0};//2a或者是43
		public void get_temp()
		{
			UInt16 t=crc_ccitt.cal_crc(temp_buf, 6);
			temp_buf[6]=(byte)(t>>8);
			temp_buf[7]=(byte)(t);
			//发送
			uart.send(temp_buf, temp_buf.Length);
		}
		public byte[] get_cmd2_buf=new byte[12] { 0x6e, 0, 0, 0x43, 0, 2, 0, 0, 1, 0, 0, 0 };
		public void get_cmd2(byte cmd,byte data1,byte data2)
		{
			get_cmd2_buf[3]=cmd;
			UInt16 t=crc_ccitt.cal_crc(get_cmd2_buf, 6);
			get_cmd2_buf[6]=(byte)(t>>8);
			get_cmd2_buf[7]=(byte)(t);

			get_cmd2_buf[8]=data1;
			get_cmd2_buf[9]=data2;

			t=crc_ccitt.cal_crc(get_cmd2_buf, 10);
			get_cmd2_buf[10]=(byte)(t>>8);
			get_cmd2_buf[11]=(byte)(t);
			//发送
			uart.send(get_cmd2_buf, get_cmd2_buf.Length);
		}
		public byte[] get_spot_buf=new byte[12] { 0x6e, 0, 0, 0x43, 0, 2, 0, 0, 1, 0, 0, 0};
		public void get_spot()
		{
			UInt16 t=crc_ccitt.cal_crc(get_spot_buf, 6);
			get_spot_buf[6]=(byte)(t>>8);
			get_spot_buf[7]=(byte)(t);

			get_spot_buf[8]=1;
			get_spot_buf[9]=0;

			t=crc_ccitt.cal_crc(get_spot_buf, 10);
			get_spot_buf[10]=(byte)(t>>8);
			get_spot_buf[11]=(byte)(t);
			//发送
			uart.send(get_spot_buf, get_spot_buf.Length);
		}
		public byte[] spot_buf=new byte[18] {0x6e,0,0,0x43,0,8,0,0, 0,0,0,0, 0,0,0,0, 0,0 };
		public void set_spot(int left, int up, int right, int down)
		{
			UInt16 t=crc_ccitt.cal_crc(spot_buf, 6);
			spot_buf[6]=(byte)(t>>8);
			spot_buf[7]=(byte)(t);

			spot_buf[8]=(byte)(left>>8);
			spot_buf[9]=(byte)(left);
			spot_buf[10]=(byte)(up>>8);
			spot_buf[11]=(byte)(up);
			spot_buf[12]=(byte)(right>>8);
			spot_buf[13]=(byte)(right);
			spot_buf[14]=(byte)(down>>8);
			spot_buf[15]=(byte)(down);

			t=crc_ccitt.cal_crc(spot_buf, 16);
			spot_buf[16]=(byte)(t>>8);
			spot_buf[17]=(byte)(t);
			//发送
			uart.send(spot_buf, spot_buf.Length);
		}
#endregion
	}
}
