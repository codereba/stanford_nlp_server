using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace standfordnlp
{
    class data_io
    {

        static public int send_data(ref Socket connection, ref byte[] data, ref UInt32 return_size)
        {
            int ret = 0;
            int _ret;
            byte[] _data_size;

            do
            {
                return_size = 0;

                _data_size = BitConverter.GetBytes((UInt32)data.Length);

                if (_data_size.Length != sizeof(UInt32))
                {
                    ret = -1;
                    break;
                }

                try
                { 
                _ret = connection.Send(_data_size, _data_size.Length, 0);
                if (_ret <= 0)
                {
                    ret = -1;
                    break;
                }
                }
                catch( Exception )
                {
                    ret = -1;
                    break;
                }

                try
                {
                    _ret = connection.Send(data, data.Length, 0);
                    if (_ret <= 0)
                    {
                        ret = -1;
                        break;
                    }
                }
                catch (Exception)
                {
                    ret = -1;
                    break;
                }

                return_size = (UInt32)_ret;
            } while (false);
            return ret;
        }

        static public int receive_data(ref Socket connection, ref byte[] data, ref UInt32 return_size)
        {
            int ret = 0;
            int _ret;
            UInt32 data_size;
            byte[] _data_size;

            do
            {
                return_size = 0;
                _data_size = new byte[sizeof(UInt32)];
                if (_data_size == null)
                {
                    break;
                }

                try
                {
                    _ret = connection.Receive(_data_size, sizeof(UInt32), 0);
                    if (_ret <= 0)
                    {
                        ret = -1;
                        break;
                    }
                }
                catch (Exception)
                {
                    ret = -1;
                    break; 
                }

                data_size = BitConverter.ToUInt32(_data_size, 0);

                if( data_size == 0)
                {
                    break; 
                }

                if (data_size > data.Length)
                {
                    ret = -1;
                    break;
                }

                try
                {
                    _ret = connection.Receive(data, (int)data_size, 0);
                    if (_ret <= 0)
                    {
                        ret = -1;
                        break;
                    }
                }
                catch(Exception)
                {
                    ret = -1; 
                    break; 
                }

                return_size = (UInt32)_ret;
            } while (false);
            return ret;
        }
    }
}
