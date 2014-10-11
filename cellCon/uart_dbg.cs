using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Ports;

namespace cellCon
{
	public class uart_dbg : SerialPort
	{
		public uart_dbg()
		{
			DataReceived+=new SerialDataReceivedEventHandler(uart_dbg_DataReceived);
		}
		public event EventHandler data_update;//数据刷新
		public event EventHandler data_rx;//接收回调函数
		public void open(int c)
		{
			PortName="COM"+c;
			Open();
		}
		void uart_dbg_DataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			int i=BytesToRead;
			byte[] buf=new byte[i];
			try
			{
				Read(buf, 0, i);
				StringBuilder sb=new StringBuilder(i*5+4);
				for(int j=0;j<i;j++)
				{
					sb.Append(string.Format("{0:X2} ", buf[j]));
				}
				//sb.Append("\r\n");
				data_update(sb.ToString(), null);
			}
			catch
			{
				return;
			}
			data_rx(buf, null);//调用客户的处理函数
		}
		public void send(byte[] b, int n)
		{
			Write(b, 0, n);
			StringBuilder sb=new StringBuilder(n*5+10);
			sb.Append("\r\n");
			for(int j=0;j<n;j++)
			{
				sb.Append(string.Format("{0:X2} ", b[j]));
			}
			sb.Append("\r\n");
			data_update(sb.ToString(), null);
		}
	}
}
