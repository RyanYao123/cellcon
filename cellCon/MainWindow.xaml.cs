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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Interactivity;
using WPFMediaKit.DirectShow.Controls;

namespace cellCon
{
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow : Window
	{
		//业务变量
		Nacelle nacell=new Nacelle();
		flir_serial flir=new flir_serial();
        comm_list comm = new comm_list();

		TestSer test_ser=new TestSer();
		public MainWindow()
		{
			InitializeComponent();
			flir.uart.data_update+=new EventHandler(flir_data_update);
		}
		void flir_data_update(object sender, EventArgs e)
		{

		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			//test_ser.Show();
			DataContext=test_ser;

			//现在电脑上装的所有摄像头中，选择一个摄像头。
			cb.ItemsSource=MultimediaUtil.VideoInputNames;
			//设置第0个摄像头为默认摄像头。
			if(MultimediaUtil.VideoInputNames.Length>0)
			{
				cb.SelectedIndex=0;
				vce.VideoCaptureSource=(string)cb.SelectedItem;
			}
			else
			{
				MessageBox.Show("电脑没有安装任何摄像头");
			}
            string[] commPort = comm.GetSericalPortName();
            string outdata = "发现串口:\r\n";
            for(int i=0; i<commPort.Length; i++){
                outdata = outdata + commPort[i];
                outdata = outdata + "\r\n";
            }
            MessageBox.Show(outdata);
		}
		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Environment.Exit(0);
		}
		private void cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			vce.VideoCaptureSource=(string)cb.SelectedItem;
		}

		#region 红外控制
		private void BT_打开串口2(object sender, RoutedEventArgs e)//云台的打开串口
		{
			var bt=sender as Button;
			if(bt.Content.ToString()=="打开串口")
			{
				flir.start(test_ser.flir_com);
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
		#endregion

		private void Rectangle_MouseUp(object sender, MouseButtonEventArgs e)
		{
			Rectangle r=sender as Rectangle;
			Point p=r.TranslatePoint(new Point(), canvas1);
			Point lefttop=p;
			Point rightbottom=new Point();
			rightbottom.X=(p.X+r.Width)*336/canvas1.Width;
			rightbottom.Y=(p.Y+r.Height)*256/canvas1.Height;
			lefttop.X=lefttop.X*336/canvas1.Width;
			lefttop.Y=lefttop.Y*256/canvas1.Height;
			flir.set_spot((int)lefttop.X, (int)lefttop.Y, (int)rightbottom.X, (int)rightbottom.Y);
		}


	}
}
