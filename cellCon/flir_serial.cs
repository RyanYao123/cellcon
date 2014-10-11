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
		public flir_serial()
		{
			uart.data_rx+=new EventHandler(uart_data_rx);
		}

		void uart_data_rx(object sender, EventArgs e)
		{
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
	}
}
