using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace cellCon
{
    class comm_list
    {
        public string[] GetSericalPortName()
        {
            string[] values = null;
            try
            {
                RegistryKey rk = Registry.LocalMachine;
                RegistryKey hs = rk.OpenSubKey(@"HARDWARE\DEVICEMAP\SERIALCOMM");
                values = new string[hs.ValueCount];
                for (int i = 0; i < hs.ValueCount; i++)
                {
                    values[i] = hs.GetValue(hs.GetValueNames()[i]).ToString();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return values;
        }
    }
}
