﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace HR_Prosecutors.Services
{
    static class DbAccess
    {
        public static bool TestConnection(string ip)
        {
            Ping ping = new Ping();
            PingReply reply = ping.Send(IPAddress.Parse(ip));

            if (reply.Status != IPStatus.Success)
                return false;

            return true;
        }
    }
}
