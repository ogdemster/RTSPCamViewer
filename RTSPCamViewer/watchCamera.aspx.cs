using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RTSPCamViewer
{
    public partial class watchCamera : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            using (var webClient = new WebClient())
            {
                //Get cam list
                webClient.Encoding = Encoding.GetEncoding("UTF-8");
                //string rawJSON = webClient.DownloadString("http://localhost:4000/");
                string rawJSON = webClient.DownloadString("https://eth.0xgreat.art/camlist.json");
                ParkCollection parkCollection = JsonConvert.DeserializeObject<ParkCollection>(rawJSON);

                int id = Convert.ToInt32(Request.QueryString["id"]);
                string parkCameraUrl = "";

                foreach (Camera item in parkCollection.Cameras)
                {
                    if(Convert.ToInt32(item.Id) == id)
                    {
                        int parkcid = item.ParkCameraId;
                        parkCameraUrl = item.ParkCameraUrl;
                    }
                }

                string ifrm = "<iframe width='360' height='240 'src='" + parkCameraUrl + 
                    "' frameborder='0' allowfullscreen='true' title='BucakBelediyesi'></iframe>";

                showIframe.InnerHtml = ifrm;
            }
        }
    }
}