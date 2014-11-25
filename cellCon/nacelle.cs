using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.IO.Ports;
using System.Runtime.InteropServices;


namespace cellCon
{
	[StructLayout(LayoutKind.Explicit, Pack=1)]
	public struct CELLPACK 
	{
		[FieldOffset(0)]
		public byte aa;
		[FieldOffset(1)]
		public byte addr;
		[FieldOffset(2)]
		public byte fun1;
		[FieldOffset(3)]
		public byte fun2;
		[FieldOffset(4)]
		public int data;
		[FieldOffset(4)]
		public float dataf;
		[FieldOffset(8)]
		public byte end;
		public CELLPACK(byte m)
		{
			aa=0xaa;
			addr=m;
			fun1=0;
			fun2=0;
			dataf=0;
			data=0;
			end=0x55;
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
		//public CELLPACK cur_cmd;
		//public bool cur_cmd_avai=false;//指令是否有效
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
			int i;
			for(i=0;i<com.Length;i++)
			{
				try
				{
					uart.PortName=com[i];
					uart.ReadTimeout=500;
					uart.BaudRate=19200;
					uart.Open();

					CELLPACK t1=new CELLPACK(0);
					t1.fun1=10;
					t1.fun2=2;
					t1.addr=1;
					t1.data=0;
					byte[] buf=Struct_Byte.StructToBytes(t1);
					uart.send(buf, buf.Length);	//发送请求
					uart.Read(buf,0,buf.Length);
					if(buf[0]==0xaa)
					{
						break;
					}
					uart.Close();
				}
				catch
				{
				}
			}
			if(i==com.Length)//若没有串口响应
			{
				return;
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
					//if(cur_cmd_avai)//若指令有效
					{
						byte[] buf=Struct_Byte.StructToBytes(pack_list.Dequeue());
						//cur_cmd_avai=false;
						//byte[] buf=Struct_Byte.StructToBytes(cur_cmd);
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
			if(pack_list.Count<3)
			{
				pack_list.Enqueue(t);
			}
			//cur_cmd=t;
			//cur_cmd_avai=true;
		}
#region 接口指令
		public void cmd_up(float y)	//俯仰
		{
			CELLPACK t1=new CELLPACK(0);
			t1.fun1=1;
			t1.fun2=2;//俯仰
			t1.addr=1;
			t1.dataf=y;
			enqueue(t1);
		}
        public void cmd_left(float x)	//航向
		{
			CELLPACK t2=new CELLPACK(0);
			t2.fun1=1;
			t2.fun2=3;//航向
			t2.addr=1;
			t2.dataf=x;
			enqueue(t2);
		}
		public void cmd_zero()	//归零
		{
			CELLPACK t1=new CELLPACK(0);
			t1.fun1=1;
			t1.fun2=4;//归零
			t1.addr=1;
			t1.data=0x0001;
			enqueue(t1);
		}
		public void cmd_zoom(int c)	//设置摄像头缩放
		{
			CELLPACK t1=new CELLPACK(0);
			t1.fun1=4;
			t1.fun2=1;
			t1.data=c;
			enqueue(t1);
		}
		public void cmd_save()	//设置摄像头录像
		{
			CELLPACK t1=new CELLPACK(0);
			t1.fun1=4;
			t1.fun2=2;
			t1.data=1;
			enqueue(t1);
		}
		public void cmd_change()	//视频切换
		{
			CELLPACK t1=new CELLPACK(0);
			t1.fun1=4;
			t1.fun2=4;
			t1.data=1;
			enqueue(t1);
		}
		/// <summary>
		/// 垂直电机力矩
		/// </summary>
		/// <param name="c"></param>
		public void cmd_VerForce(byte c)
		{
			CELLPACK t1=new CELLPACK(0);
			t1.addr=1;
			t1.fun1=6;
			t1.fun2=1;
			t1.data=c;
			enqueue(t1);
		}
		public void cmd_HorForce(byte c)
		{
			CELLPACK t1=new CELLPACK(0);
			t1.addr=1;
			t1.fun1=6;
			t1.fun2=2;
			t1.data=c;
			enqueue(t1);
		}
		public void cmd_HorCali()
		{
			CELLPACK t1=new CELLPACK(0);
			t1.addr=1;
			t1.fun1=7;
			t1.fun2=0;
			t1.data=0;
			enqueue(t1);
		}
		public void cmd_VerCali()
		{
			CELLPACK t1=new CELLPACK(0);
			t1.addr=1;
			t1.fun1=7;
			t1.fun2=1;
			t1.data=0;
			enqueue(t1);
		}
		/// <summary>
		/// 0：演示模式
		/// 1：纯手动
		/// 2：稳定模式
		/// 3：跟踪模式
		/// 4：导航模式
		/// </summary>
		public void cmd_mode(byte c)	//设置模式
		{
			CELLPACK t1=new CELLPACK(0);
			t1.addr=1;
			t1.fun1=5;
			t1.fun2=1;
			t1.data=c;
			enqueue(t1);
		}
		public void cmd_trace(byte c)	//跟踪功能，现在还不对
		{
			CELLPACK t1=new CELLPACK(0);
			t1.addr=1;
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
			t1.addr=1;
			t1.data=(Int16)y;
			t2.fun1=0x10;
			t2.fun2=0;//航向
			t2.addr=1;
			t2.data=(Int16)x;
			enqueue(t1);
			enqueue(t2);
		}
#endregion
	}
}
