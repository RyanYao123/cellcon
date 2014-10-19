using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace cellCon
{
	/// <summary>
	/// TestSer.xaml 的交互逻辑
	/// </summary>
	public partial class TestSer : Window
	{
		//业务变量
		Nacelle nacell=new Nacelle();
		flir_serial flir=new flir_serial();

		//显示变量
		public int cell_com { set; get; }
		public int cell_x { get; set; }
		public int cell_y { get; set; }

		//红外部分
		public int flir_com { set; get; }
		public int flir_left { get; set; }
		public int flir_up { get; set; }
		public int flir_right { get; set; }
		public int flir_down { get; set; }

		public TestSer()
		{
			InitializeComponent();
			UInt16 t=crc_ccitt.cal_crc(new byte[]{0x6e,0,0,0x0b,0,2},6);
			flir_org_text.Text=t.ToString("X");

			//初始化
			nacell.uart.data_update+=new EventHandler(nacell_data_update);
			flir.uart.data_update+=new EventHandler(flir_data_update);
			//界面初始化
			//云台部分
			cell_com=14;
			cell_x=0;
			cell_y=10;

			//红外部分
			flir_com=6;
			flir_left=168-20;
			flir_up=128-20;
			flir_right=168+20;
			flir_down=128+20;

			DataContext=this;
		}
		object[] delegate_obj=new object[2];
		void nacell_data_update(object sender, EventArgs e)
		{
			Dispatcher.BeginInvoke((EventHandler)delegate
			{
				cell_org_text.AppendText(sender as string);
			}, delegate_obj);
		}
		void flir_data_update(object sender, EventArgs e)
		{
			Dispatcher.BeginInvoke((EventHandler)delegate
			{
				flir_org_text.AppendText(sender as string);
			}, delegate_obj);
		}
		private void text_TextChanged(object sender, TextChangedEventArgs e)
		{
			TextBox tb=sender as TextBox;
			tb.ScrollToEnd();
		}
		#region 云台控制
		private void BT_打开串口1(object sender, RoutedEventArgs e)//云台的打开串口
		{
			var bt=sender as Button;
			if(bt.Content.ToString()=="打开串口")
			{
				try
				{
					nacell.start(cell_com);
				}
				catch
				{
					
				}
				bt.Content="关闭串口";
			}
			else
			{
				nacell.stop();
				bt.Content="打开串口";
			}
		}

		private void slider_Ver_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			nacell.cmd_up(cell_y);
		}
		private void slider_Hor_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			nacell.cmd_left(cell_x);
		}
		#endregion

		#region 红外控制
		private void BT_打开串口2(object sender, RoutedEventArgs e)//云台的打开串口
		{
			var bt=sender as Button;
			if(bt.Content.ToString()=="打开串口")
			{
				try
				{
					flir.start(flir_com);
				}
				catch
				{

				}
				bt.Content="关闭串口";
			}
			else
			{
				flir.stop();
				bt.Content="打开串口";
			}
		}
		private void bt_flir_get(object sender, RoutedEventArgs e)
		{
			flir.get_temp();
		}
		private void bt_flir_get_spot(object sender, RoutedEventArgs e)
		{
			flir.get_spot();
		}
		private void flir_set_spot(object sender, RoutedEventArgs e)
		{
			flir.set_spot(flir_left,flir_up,flir_right,flir_down);
		}
		private void bt_flir_test_cmd(object sender, RoutedEventArgs e)
		{
			flir.get_cmd2(0x43, 0,1);
		}
		#endregion


	}
}
