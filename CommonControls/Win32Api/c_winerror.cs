using System;
using System.Collections.Generic;
using System.Text;

namespace CommonControls.Win32Api
{
    //Constants for errors:
    public enum winerror:uint
    {
        ERROR_SUCCESS = 0,
        ERROR_FILE_NOT_FOUND = 2,
        ERROR_ACCESS_DENIED = 5,

        ERROR_INVALID_PARAMETER = 87,    // dderror
        ERROR_INVALID_NAME = 123,
        ERROR_ALREADY_EXISTS    = 183,

        ERROR_BUFFER_TOO_SMALL = (c_winerror.RASBASE + 3),
        ERROR_NO_CONNECTION = (c_winerror.RASBASE + 68),
        ERROR_USER_DISCONNECTION = (c_winerror.RASBASE + 31),
        
        /// <summary>ReadFile/WriteFile : Overlapped operation is incomplete.</summary>
        ERROR_IO_PENDING = 997,
          
    }

    public class c_winerror
    {
        public const int RASBASE = 600;
    }
}
