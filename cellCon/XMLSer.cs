using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace cellCon
{
	/// <summary>
	/// XML串行化类，提供静态串行化函数，并可以通过继承的方式提供更方便的串行化方式
	/// </summary>
	public class XMLSer
	{
		/// <summary>
		/// 进行串行化，将内存变量保存到xml文件中
		/// </summary>
		/// <param name="s">文件名</param>
		/// <param name="o">要保存的内存变量</param>
		static public void Serial(string s,object o)
		{
			XmlSerializer ser=new XmlSerializer(o.GetType(),new XmlRootAttribute("root"));//声明对象，并且添加根节点 
			StreamWriter sw=new StreamWriter(s);
			ser.Serialize(sw,o);
			sw.Close();
		}
		static public void Serial(string s, object o,Type[] t)
		{
			XmlSerializer ser=new XmlSerializer(o.GetType(),t);//声明对象，并且添加根节点 
			StreamWriter sw=new StreamWriter(s);
			ser.Serialize(sw, o);
			sw.Close();
		}
		/// <summary>
		/// 反串行化，通过读XML文件构造一个内存变量
		/// </summary>
		/// <param name="s">文件名</param>
		/// <param name="t">构造内存变量的类型</param>
		/// <returns></returns>
		static public object DSerial(string s,Type t)
		{
			FileStream fs=new FileStream(s,FileMode.Open);
			XmlSerializer ser=new XmlSerializer(t,new XmlRootAttribute("root"));
			object o=ser.Deserialize(fs);
			fs.Close();
			return o;
		}
		/// <summary>
		/// 方便数组类型的导入
		/// </summary>
		/// <param name="s"></param>
		/// <param name="t"></param>
		/// <returns></returns>
		static public object DSerial(string s, Type t,Type[] tt)
		{
			FileStream fs=new FileStream(s, FileMode.Open);
			XmlSerializer ser=new XmlSerializer(t,tt);
			object o=ser.Deserialize(fs);
			fs.Close();
			return o;
		}
		public void Serial(string s)//数据对象的串行化存储到文件
		{ 
			Serial(s,this);
		}
	}
	/// <summary>
	/// 二进制串行化类
	/// </summary>
	public class BinSer
	{
		static BinaryFormatter MyBF=new BinaryFormatter();
		static public void Serial(string s,object o)
		{
			Stream fs=new FileStream(s,FileMode.Create,FileAccess.Write,FileShare.None);
			MyBF.Serialize(fs,o);
			fs.Close();
		}
		static public object DSerial(string s)
		{
			FileStream fs=new FileStream(s,FileMode.Open);
			object o=MyBF.Deserialize(fs);
			fs.Close();
			return o;
		}
	}
}
