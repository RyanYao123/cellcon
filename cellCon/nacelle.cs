using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.IO.Ports;

namespace cellCon
{
	public struct CELLPACK 
	{
		public byte aa;
		public byte mod;
		public byte fun1;
		public byte fun2;
		public Int16 data;
		public byte ff;
		public CELLPACK(byte m)
		{
			aa=0xaa;
			mod=m;
			fun1=0;
			fun2=0;
			data=0;
			ff=0xff;
		}
	}
	/// <summary>
	/// 吊舱控制类
	/// </summary>
	public class Nacelle
	{
		public uart_dbg uart=new uart_dbg();
		public head组包 pack=new head组包();
		public Thread send_th;
		public EventHandler data_update;
		//待发送指令的队列
		public Queue<CELLPACK> pack_list=new Queue<CELLPACK>(10);
		public Nacelle()
		{
			pack.pack_len=10;
			pack.pre_cb=pre_cb;
			pack.pre_offset=1;
			pack.pro=pro;
			
		}
		//搜索串口
		public void find_uart(string[] com)
		{
			for(int i=0;i<com.Length;i++)
			{
				try
				{
					uart.PortName=com[i];
					uart.ReadTimeout=500;
					uart.Open();
					//uart.Write();
					//uart.Read();
					uart.Close();
				}
				catch
				{
				}
			}
			uart.data_rx+=uart_DataReceived;
		}
		//需由调用方捕获异常,并检查Port的合法性
		public void start(int port)
		{
			uart.open(port);
			send_th=new Thread(send_task);
			send_th.Start();
		}
		public void stop()
		{
			send_th.Abort();
			uart.Close();
		}
		//发送任务
		void send_task(object o)
		{
			while(true)
			{
				//从发送队列中取得一个指令
				try
				{
					if(pack_list.Count>0)
					{
						byte[] buf=Struct_Byte.StructToBytes(pack_list.Dequeue());
						uart.send(buf, buf.Length);
					}
				}
				catch
				{
					//return;
				}
				Thread.Sleep(100);
			}
		}
		//接收函数
		void uart_DataReceived(object sender,EventArgs e)
		{
			byte[] buf=sender as byte[];
			for(int i=0;i<buf.Length;i++)
			{
				pack.rec_byte(buf[i]);
			}
		}
		int pre_cb(byte[] b, int len)
		{
			return 10;
		}
		bool pro(byte[] b,int len)
		{

			return true;
		}
		void enqueue(CELLPACK t)
		{
			if(pack_list.Count<10)
			{
				pack_list.Enqueue(t);
			}
		}
#region 接口指令
		public void cmd_up(int y)
		{
			CELLPACK t1=new CELLPACK(0);
			t1.fun1=1;
			t1.fun2=2;//俯仰
			t1.mod=0;
			t1.data=(Int16)y;
			enqueue(t1);
		}
		public void cmd_left(int x)
		{
			CELLPACK t2=new CELLPACK(0);
			t2.fun1=1;
			t2.fun2=3;//航向
			t2.mod=0;
			t2.data=(Int16)x;
			enqueue(t2);
		}
		public void cmd_zoom(int c)
		{
			CELLPACK t1=new CELLPACK(0);
			t1.fun1=4;
			t1.data=0;
			if(c>0)
			{
				t1.fun2=1;
			}
			else if (c<0)
			{
				t1.fun2=2;
			}
			else
			{
				t1.fun2=3;
			}
			enqueue(t1);
		}
		/// <summary>
		/// 垂直电机力矩
		/// </summary>
		/// <param name="c"></param>
		public void cmd_VerForce(byte c)
		{
			CELLPACK t1=new CELLPACK(0);
			t1.mod=0;
			t1.fun1=6;
			t1.fun2=1;
			t1.data=c;
			enqueue(t1);
		}
		public void cmd_HorForce(byte c)
		{
			CELLPACK t1=new CELLPACK(0);
			t1.mod=0;
			t1.fun1=6;
			t1.fun2=2;
			t1.data=c;
			enqueue(t1);
		}
		public void cmd_HorCali()
		{
			CELLPACK t1=new CELLPACK(0);
			t1.mod=0;
			t1.fun1=7;
			t1.fun2=0;
			t1.data=0;
			enqueue(t1);
		}
		public void cmd_VerCali()
		{
			CELLPACK t1=new CELLPACK(0);
			t1.mod=0;
			t1.fun1=7;
			t1.fun2=1;
			t1.data=0;
			enqueue(t1);
		}
		/// <summary>
		/// 0x00停止模式；
		/// 0x01 手动模式
		/// 0x02 陀螺稳定模式
		/// 0x03 跟踪模式
		/// </summary>
		public void cmd_mode(byte c)
		{
			CELLPACK t1=new CELLPACK(0);
			t1.mod=0;
			t1.fun1=8;
			t1.fun2=1;
			t1.data=c;
			enqueue(t1);
		}
		public void cmd_trace(byte c)
		{
			CELLPACK t1=new CELLPACK(0);
			t1.mod=0;
			t1.fun1=0x11;
			t1.fun2=c;
			t1.data=0;
			enqueue(t1);
		}
		public void cmd_TraceBox(int x, int y)
		{
			CELLPACK t1=new CELLPACK(0);
			CELLPACK t2=new CELLPACK(0);
			t1.fun1=0x10;
			t1.fun2=1;//俯仰
			t1.mod=0;
			t1.data=(Int16)y;
			t2.fun1=0x10;
			t2.fun2=0;//航向
			t2.mod=0;
			t2.data=(Int16)x;
			enqueue(t1);
			enqueue(t2);
		}
#endregion
	}
}
