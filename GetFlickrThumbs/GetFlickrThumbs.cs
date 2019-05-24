using FlickrNet;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GetFlickrThumbs
{
    public static class GetFlickrThumbs
    { 
        [FunctionName("GetFlickrThumbs")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            Flickr flickr;
            PhotoCollection photos;
            HttpResponseMessage response;

            try
            {
                string url = "";
                int cnt = 0;

                flickr = new Flickr(user.Default.flickrApiKey);
                photos = new PhotoCollection();

                PhotoSearchOptions opt = new PhotoSearchOptions
                {
                    UserId = user.Default.userId,
                    SortOrder = PhotoSearchSortOrder.DateTakenDescending
                };
                photos = flickr.PhotosSearch(opt);

                foreach (Photo photo in photos)
                {
                    url += CreateThumbnailHtmlTag(photo.WebUrl, photo.SquareThumbnailUrl);
                    cnt += 1;
                    if (cnt >= user.Default.thumbnailCnt)
                    {
                        break;
                    }
                }

                string html = CreateContentHtml(url);
                response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(html)
                };
                response.Content.Headers.ContentType = SetContentType();
                return response;
            }
            catch (Exception)
            {
                response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                throw;
            }
            finally
            {
                GC.Collect();
            }
        }

        [FunctionName("MediaTypeHeaderValue")]
        private static MediaTypeHeaderValue SetContentType()
        {
            try
            {
                return new MediaTypeHeaderValue("text/html");
            }
            catch (Exception)
            {
                throw;
            }
        }

        [FunctionName("CreateThumbnailHtmlTag")]
        private static string CreateThumbnailHtmlTag(string flickrUrl, string thumbnailUrl)
        {
            try
            {
                return @"<a href='" + flickrUrl + "' target='brank'><img src = '" + thumbnailUrl + "'/></a>";
            }
            catch (Exception)
            {
                throw;
            }
        }

        [FunctionName("CreateContentHtml")]
        private static string CreateContentHtml(string url)
        {
            try
            {
                return @"<html><head><meta charset='UTF-8'></head><body>" + url + "</body></html>";
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
