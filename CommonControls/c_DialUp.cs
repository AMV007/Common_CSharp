using System;
using System.Collections.Generic;
using System.Text;
using CommonControls.Win32Api;
using System.Runtime.InteropServices;

namespace CommonControls
{
    public class c_DialUp
    {   
        private IntPtr ConnPtr = IntPtr.Zero;
        public int LastError;
        public const string EmptyConn = "Нет соединения"; 

        public struct s_ConnState
        {
            public rasapi32.RASDIALPARAMS Param;
            public uint Msg;
            public rasapi32.RASCONNSTATE State;
            public uint Error;
            public string ErrorDescription;
        }

        public readonly rasapi32.RASDIALPARAMS Param;
        public bool Connected
        {
            get
            {
                if (ConnPtr == IntPtr.Zero) return false;
                rasapi32.RASCONNSTATUS State = new rasapi32.RASCONNSTATUS();
                LastError = rasapi32.RasGetConnectStatus(ConnPtr, State);
                if (LastError != 0) return false;
                if (State.rasconnstate == rasapi32.RASCONNSTATE.Connected) return true;
                return false;
            }
        }

        public event EventHandler ConnStateChanged = null;
        private void OnConnStateChanged(s_ConnState State)
        {
            if (ConnStateChanged != null) ConnStateChanged.Invoke(State, EventArgs.Empty);
        }

        delegate void d_RasDialCallBack(uint Msg, rasapi32.RASCONNSTATE State, uint Error);
        d_RasDialCallBack RasDialCallBack;

        public c_DialUp()
        {
            Param = new rasapi32.RASDIALPARAMS();
            Param.szEntryName = EmptyConn;
            RasDialCallBack = new d_RasDialCallBack(CallBack);
        }

        public bool Dial(string ConnName, string Number, string Login, string Pass)
        {
            HangUp();
            
            Param.szEntryName = ConnName;
            Param.szPhoneNumber = Number;
            Param.szUserName = Login;
            Param.szPassword = Pass;

            if ((LastError=(int)rasapi32.RasDial(null, null, Param, 0, RasDialCallBack, ref ConnPtr)) != (int)winerror.ERROR_SUCCESS) return false;
            return true;
        }

        void CallBack(uint Msg, rasapi32.RASCONNSTATE State, uint Error)
        {
            s_ConnState c_State = new s_ConnState();
            c_State.Msg = Msg;
            c_State.State = State;
            c_State.Error = Error;
            c_State.Param = Param;
            c_State.ErrorDescription = GetErrorDescription((int)Error);
            OnConnStateChanged(c_State);
        }

        public bool HangUp()
        {
            bool Result = true;
            if (ConnPtr != IntPtr.Zero)
            {
                if ((LastError=(int)rasapi32.RasHangUp(ConnPtr)) != (int)winerror.ERROR_SUCCESS) Result = false;
                ConnPtr = IntPtr.Zero;
            }
            return Result;
        }

        public string GetErrorDescription(int Error)
        {
            StringBuilder sb = new StringBuilder(512);            
            rasapi32.RasGetErrorString((uint)Error, sb, sb.Capacity);
            return sb.ToString();
        }

        public string GetLastErrorDescription()
        {
            return GetErrorDescription(LastError);
        }
    }
}
