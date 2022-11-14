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
    public partial class ReadJsonFile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            /*
             * [] Mobile device 1x cam and slide if more
             * [] Desktop 3x cam and slide if more jquery slide?
             * [X] button on cameras which redirect to cam view page
             * [x] add whole cams in a jsonfile and embed in your website
             * [x] Cam name, Park name, Camera will be listed
             * [] Dont allow opening more than 1 cam.
             * [x] Bootstrap slide/Card
             * */


            //Device type isMobile
            var mobile = HttpContext.Current.Request.Browser.IsMobileDevice;

            using (var webClient = new WebClient())
            {
                //Get cam list
                webClient.Encoding = Encoding.GetEncoding("UTF-8");
                //string rawJSON = webClient.DownloadString("http://localhost:4000/");
                string rawJSON = webClient.DownloadString("https://eth.0xgreat.art/camlist.json");
                ParkCollection parkCollection = JsonConvert.DeserializeObject<ParkCollection>(rawJSON);


                //Create Carousel/Card-Group/Card
                StringBuilder str = new StringBuilder();
                str.Append("<div class='h1 text-center p-4'>BUCAK KAMERALAR</div>");
                //Total Cam Count
                int[] parkIds = parkCollection.Cameras.GroupBy(x => x.ParkId)
                    .Select(grp => grp.First()).Select(p => p.ParkId)
                    .ToArray();

                //Iterate cams
                for (int i = 0; i < parkIds.Count(); i++)
                {
                    var cardParkCameras = parkCollection.Cameras.Where(x => x.ParkId == parkIds[i]);

                    string parkName = cardParkCameras.FirstOrDefault().ParkName;
                    int parkCameraCount = cardParkCameras.Count();
                    int carouselItem = (int)Math.Ceiling((decimal)parkCameraCount / 3);

                    if (!mobile)
                    {
                        str.Append(desktop(cardParkCameras, carouselItem));
                    }else
                    {
                        str.Append(mobilList(cardParkCameras));
                    }
                    tutski.InnerHtml = str.ToString();
                }

            }
        }

        private string desktop(IEnumerable<Camera> cardParkCameras, int carouselItem)
        {
            StringBuilder sb = new StringBuilder();

            int topCamCount = cardParkCameras.Count();

            IEnumerable<Camera> cameras = cardParkCameras;
            string parkname = cameras.ToList().FirstOrDefault().ParkName;

            sb.Append("<div id='Bucakcaptions' class='carousel slide p-1' data-ride='carousel'><div class='carousel-inner'>");
            //sb.Append("<div id='"+ parkname + "captions' class='carousel slide' data-ride='carousel'><div class='carousel-inner'>"
            //);
            
            int currentCamera = 0;
            for (int i = 0; i <= carouselItem -1; i++)
            {
                //sb.Append("<div class='carousel-item act'><div class='card-group'>");
                sb.Append("<div class='carousel-item act'><div class='cardWrapper'>");

                for (int j = 0; j <= 2; j++)
                {
                    if (currentCamera == topCamCount)
                        break;
                    sb.Append("<div class='card'><div class='card-body'><h5 class='card-title'>" + parkname + 
                        "</h5><h6 class='card-subtitle mb-2 text-muted'>"+ cardParkCameras.ToList()[currentCamera].ParkCameraName +
                        "</h6><p><iframe width='360' height='240' src='" + cardParkCameras.ToList()[currentCamera].ParkCameraUrl +
                        "' frameborder='0' allowfullscreen='true' title='Bucakbel'></iframe></p><a href='#' class='card-link'>İzle</a>"+
                        "</div></div>");
                    //sb.Append(
                    //    "<div class='card'>" +
                    //    "<iframe width='360' height='240' src='" + cardParkCameras.ToList()[currentCamera].ParkCameraUrl + "' frameborder='0' allowfullscreen='true' " +
                    //    "title='Bucakbel'></iframe>" +
                    //    "<div class='card-body'><h5 class='card-title'>" + cardParkCameras.ToList()[currentCamera].ParkCameraName + "</h5></div></div>"
                    //    );
                    if (currentCamera != topCamCount)
                        currentCamera++;
                }
                sb.Append("</div></div>");
                //sb.Append("</div></div>");
            }
            sb.Append("></div><button class='carousel-control-prev' type='button' data-bs-target='#Bucakcaptions' data-bs-slide='prev'><span class='carousel-control-prev-icon' aria-hidden='true'></span><span class='visually-hidden'>Previous</span></button><button class='carousel-control-next' type='button' data-bs-target='#Bucakcaptions' data-bs-slide='next'><span class='carousel-control-next-icon' aria-hidden='true'></span> <span class='visually-hidden'>Next</span></button></div>");
            //sb.Append("<button class='carousel-control-prev' type='button' data-bs-target='#" + parkname +"captions' data-bs-slide='prev'>" + 
            //    "<span class='carousel-control-prev-icon' aria-hidden='true'></span><span class='visually-hidden'>Previous</span></button>" + 
            //    "<button class='carousel-control-next' type='button' data-bs-target='#" + parkname + "captions' data-bs-slide='next'>" + 
            //    "<span class='carousel-control-next-icon' aria-hidden='true'></span><span class='visually-hidden'>Next</span></button></div></div>");

            int place = sb.ToString().IndexOf("act");
            string result = sb.ToString().Remove(place, "act".Length).Insert(place, "active");
            return result;

        }

        private string mobilList(IEnumerable<Camera> cardParkCameras)
        {
            StringBuilder sb = new StringBuilder();

            int topCamCount = cardParkCameras.Count();

            IEnumerable<Camera> cameras = cardParkCameras;
            string parkname = cameras.ToList().FirstOrDefault().ParkName;
            sb.Append("<div class='row'><div class='col-md-12 h2'>" + parkname + "</div></div>");
            sb.Append("<div id='" + parkname + "captions' class='carousel slide' data-ride='carousel'><div class='carousel-inner'>"
                );

            for (int i = 0; i <= topCamCount - 1; i++)
            {
                sb.Append(
                    "<div class='carousel-item act'><div class='card'>" +
                    "<iframe width='360' height='240' src='" + cardParkCameras.ToList()[i].ParkCameraUrl + "' frameborder='0' allowfullscreen='true' " +
                    "title='Bucakbel'></iframe>" +
                    "<div class='card-body'><h5 class='card-title'>" + cardParkCameras.ToList()[i].ParkCameraName + "</h5></div></div></div>"
                    );
            }

            sb.Append("</div><button class='carousel-control-prev' type='button' data-bs-target='#" + parkname + "captions' data-bs-slide='prev'>" +
                "<span class='carousel-control-prev-icon' aria-hidden='true'></span><span class='visually-hidden'>Previous</span></button>" +
                "<button class='carousel-control-next' type='button' data-bs-target='#" + parkname + "captions' data-bs-slide='next'>" +
                "<span class='carousel-control-next-icon' aria-hidden='true'></span><span class='visually-hidden'>Next</span></button></div>");

            int place = sb.ToString().IndexOf("act");
            string result = sb.ToString().Remove(place, "act".Length).Insert(place, "active");
            return result;

        }
    }
}