using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Globalization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace RTSPCamViewer
{
    public partial class index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            //Device type isMobile
            var mobile = HttpContext.Current.Request.Browser.IsMobileDevice;

            using (var webClient = new WebClient())
            {
                //Get cam list
                webClient.Encoding = Encoding.GetEncoding("UTF-8");
                string rawJSON = webClient.DownloadString("https://eth.0xgreat.art/camlist.json");
                ParkCollection parkCollection = JsonConvert.DeserializeObject<ParkCollection>(rawJSON);

                Random rand;
                string sliderId = "";
                //Create Carousel/Card-Group/Card
                StringBuilder str = new StringBuilder();
                str.Append("<div class='h1 text-center p-4'>BUCAK KAMERALAR</div>");
                //Total Cam Count
                int[] parkIds = parkCollection.Cameras.GroupBy(x => x.ParkId)
                    .Select(grp => grp.First()).Select(p => p.ParkId)
                    .ToArray();

                //Iterate parks
                for (int i = 0; i < parkIds.Count(); i++)
                {
                    var cardParkCameras = parkCollection.Cameras.Where(x => x.ParkId == parkIds[i]);

                    rand = new Random(DateTime.Now.Millisecond);
                    sliderId = "A" + rand.Next();
                    if (!mobile)
                    {
                        int parkCameraCount = cardParkCameras.Count();
                        int carouselItem = (int)Math.Ceiling((decimal)parkCameraCount / 3);
                        str.Append(desktop(cardParkCameras, carouselItem));
                    }
                    else
                    {
                        str.Append(mobilList(cardParkCameras));
                    }
                }
                tutski.InnerHtml = str.ToString();
            }
        }

        private string desktop(IEnumerable<Camera> cardParkCameras, int carouselItem)
        {
            StringBuilder sb = new StringBuilder();

            int topCamCount = cardParkCameras.Count();
            var cam = cardParkCameras.ToList().FirstOrDefault();
            string parkname = cam.ParkName;
            string sliderId = "A" + cam.Id.ToString();


            sb.Append("<div id='"+ sliderId +"' class='carousel slide p-1' data-ride='carousel'><div class='carousel-inner'>");

            int currentCamera = 0;
            for (int i = 0; i <= carouselItem -1; i++)
            {
                sb.Append("<div class='carousel-item act'><div class='cardWrapper'>");

                for (int j = 0; j <= 2; j++)
                {
                    if (currentCamera == topCamCount)
                        break;
                    sb.Append("<div class='card'><div class='card-body'><h5 class='card-title'>" + parkname +
                        "</h5><span class='float-end text-primary'>" + (currentCamera+1) + "/" + topCamCount  + "</span><h6 class='card-subtitle mb-2 text-muted'>" + cardParkCameras.ToList()[currentCamera].ParkCameraName +
                        "</h6><p class='unclickable'><iframe width='360' height='240' src='" + cardParkCameras.ToList()[currentCamera].ParkCameraUrl +
                        "' frameborder='0' allowfullscreen='true' title='Bucakbel'></iframe></p><div id='btnWatch' class='text-center'>"+
                        "<button class='btn btn-primary openPopup' data-href='watchCamera.aspx?id=" + cardParkCameras.ToList()[currentCamera].Id + "' data-bs-toggle='modal' data-bs-target='#xmodal'>İzle</button></div>" +
                        "</div></div>");
                    if (currentCamera != topCamCount)
                        currentCamera++;
                }
                sb.Append("</div></div>");
            }
            sb.Append("</div><button class='carousel-control-prev' type='button' data-bs-target='#" + sliderId + 
                "' data-bs-slide='prev'><span class='carousel-control-prev-icon' aria-hidden='true'></span>"+
                "<span class='visually-hidden'>Previous</span></button><button class='carousel-control-next' type='button' data-bs-target='#"+ 
                sliderId +"' data-bs-slide='next'><span class='carousel-control-next-icon' aria-hidden='true'>"+
                "</span> <span class='visually-hidden'>Next</span></button></div>");

            int place = sb.ToString().IndexOf("act");
            string result = sb.ToString().Remove(place, "act".Length).Insert(place, "active");
            return result;

        }

        private string mobilList(IEnumerable<Camera> cardParkCameras)
        {
            StringBuilder sb = new StringBuilder();
            int topCamCount = cardParkCameras.Count();
            var cam = cardParkCameras.ToList().FirstOrDefault();
            string sliderId = "A" + cam.Id.ToString();

            string parkname = cam.ParkName;
            sb.Append("<div id='"+ sliderId +"' class='carousel slide p-1' data-ride='carousel'><div class='carousel-inner'>");

            for (int i = 0; i <= topCamCount - 1; i++)
            {
                sb.Append("<div class='carousel-item act'><div class='card'><div class='card-body'><h5 class='card-title'>" + parkname +
                        "</h5><span class='float-end text-primary'>" + (i+1) + "/" + topCamCount  + "</span><h6 class='card-subtitle mb-2 text-muted'>" + cardParkCameras.ToList()[i].ParkCameraName +
                        "</h6><p class='unclickable'><iframe width='360' height='240' src='" + cardParkCameras.ToList()[i].ParkCameraUrl +
                        "' frameborder='0' allowfullscreen='true' title='Bucakbel'></iframe></p><div id='btnWatch' class='text-center'>" +
                        "<button class='btn btn-primary openPopup' data-href='watchCamera.aspx?id=" + cardParkCameras.ToList()[i].Id + "' data-bs-toggle='modal' data-bs-target='#xmodal'>İzle</button></div>" +
                        "</div></div></div>");
            }
            sb.Append("</div><button class='carousel-control-prev' type='button' data-bs-target='#" + sliderId + 
                "' data-bs-slide='prev'><span class='carousel-control-prev-icon' aria-hidden='true'></span>" +
                "<span class='visually-hidden'>Previous</span></button><button class='carousel-control-next' type='button' data-bs-target='#"+
                sliderId+"' data-bs-slide='next'><span class='carousel-control-next-icon' aria-hidden='true'></span>"+ 
                "<span class='visually-hidden'>Next</span></button></div>");

            int place = sb.ToString().IndexOf("act");
            string result = sb.ToString().Remove(place, "act".Length).Insert(place, "active");
            return result;

        }
    }
}