using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IVEFModels.BodyObj;

namespace IVEFModels
{
    public class Body
    {
        public LoginRequest LoginRequest { get; set; }
        public LoginResponse LoginResponse { get; set; }
        public string Logout { get; set; }
        public ObjectData[] ObjectDatas { get; set; }
        public Ping Ping { get; set; }
        public Pong Pong { get; set; }
        public ServerStatus ServerStatus { get; set; }
        public ServiceRequest ServiceRequest { get; set; }
        public ServiceRequestResponse ServiceRequestResponse { get; set; }
    }
}
