using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RTSPCamViewer
{
    public class Camera
    {
        public int Id { get; set; }
        public int ParkId { get; set; }
        public string ParkName { get; set; }
        public int ParkCameraId { get; set; }

        public string ParkCameraName { get; set; }

        public string ParkCameraUrl { get; set; }
    }
}