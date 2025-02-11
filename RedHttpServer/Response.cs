using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Red.Interfaces;

namespace Red
{
    /// <summary>
    ///     Class representing the response to a clients request
    ///     All
    /// </summary>
    public sealed class Response : InContext
    {
        /// <summary>
        ///     The ASP.NET HttpResponse that is wrapped
        /// </summary>
        public readonly HttpResponse AspNetResponse;

        internal Response(Context context) : base(context)
        {
            AspNetResponse = context.AspNetContext.Response;
        }

        /// <summary>
        ///     The headers for the response
        /// </summary>
        public IHeaderDictionary Headers => AspNetResponse.Headers;

        /// <summary>
        ///     The cancellation token the request being aborted
        /// </summary>
        public CancellationToken Aborted => AspNetResponse.HttpContext.RequestAborted;
        
        /// <summary>
        ///     Obsolete. Please used Headers property instead.
        ///     This method Will be removed in a later version
        /// </summary>
        /// <param name="headerName">The name of the header</param>
        /// <param name="headerValue">The value of the header</param>
        [Obsolete]
        public void AddHeader(string headerName, string headerValue)
        {
            Headers[headerName] = headerValue;
        }

        /// <summary>
        ///     Redirects the client to a given path or url
        /// </summary>
        /// <param name="redirectPath">The path or url to redirect to</param>
        /// <param name="permanent">Whether to respond with a temporary or permanent redirect</param>
        public Task<HandlerType> Redirect(string redirectPath, bool permanent = false)
        {
            AspNetResponse.Redirect(redirectPath, permanent);
            return Handlers.CachedFinalHandlerTask;
        }

        /// <summary>
        ///     Sends data as text
        /// </summary>
        /// <param name="data">The text data to send</param>
        /// <param name="contentType">The mime type of the content</param>
        /// <param name="fileName">If the data represents a file, the filename can be set through this</param>
        /// <param name="attachment">Whether the file should be sent as attachment or inline</param>
        /// <param name="status">The status code for the response</param>
        public Task<HandlerType> SendString(string data, string contentType = "text/plain", string fileName = "",
            bool attachment = false, HttpStatusCode status = HttpStatusCode.OK)
        {
            return SendString(AspNetResponse, data, contentType, fileName, attachment, status, Aborted);
        }

        /// <summary>
        ///     Static helper for use in middleware
        /// </summary>
        public static async Task<HandlerType> SendString(HttpResponse response, string data, string contentType,
            string fileName, bool attachment, HttpStatusCode status, CancellationToken cancellationToken)
        {
            response.StatusCode = (int) status;
            response.ContentType = contentType;
            if (!string.IsNullOrEmpty(fileName))
            {
                var contentDisposition = $"{(attachment ? "attachment" : "inline")}; filename=\"{fileName}\"";
                response.Headers.Add("Content-disposition", contentDisposition);
            }

            await response.WriteAsync(data, cancellationToken);
            return HandlerType.Final;
        }

        /// <summary>
        ///     Send a empty response with a status code
        /// </summary>
        /// <param name="response">The HttpResponse object</param>
        /// <param name="status">The status code for the response</param>
        public static Task<HandlerType> SendStatus(HttpResponse response, HttpStatusCode status, CancellationToken cancellationToken)
        {
            return SendString(response, status.ToString(), "text/plain", "", false, status, cancellationToken);
        }

        /// <summary>
        ///     Send a empty response with a status code
        /// </summary>
        /// <param name="status">The status code for the response</param>
        public Task<HandlerType> SendStatus(HttpStatusCode status)
        {
            return SendString(status.ToString(), status: status);
        }

        /// <summary>
        ///     Sends object serialized to text using the current IJsonConverter plugin
        /// </summary>
        /// <param name="data">The object to be serialized and send</param>
        /// <param name="status">The status code for the response</param>
        public async Task<HandlerType> SendJson<T>(T data, HttpStatusCode status = HttpStatusCode.OK)
        {
            AspNetResponse.StatusCode = (int) status;
            AspNetResponse.ContentType = "application/json";
            await Context.Plugins.Get<IJsonConverter>().SerializeAsync(data, AspNetResponse.Body, Aborted);
            return HandlerType.Final;
        }

        /// <summary>
        ///     Sends object serialized to text using the current IXmlConverter plugin
        /// </summary>
        /// <param name="data">The object to be serialized and send</param>
        /// <param name="status">The status code for the response</param>
        public async Task<HandlerType> SendXml<T>(T data, HttpStatusCode status = HttpStatusCode.OK)
        {
            AspNetResponse.StatusCode = (int) status;
            AspNetResponse.ContentType = "application/xml";
            await Context.Plugins.Get<IXmlConverter>().SerializeAsync(data, AspNetResponse.Body, Aborted);
            return HandlerType.Final;
        }

        /// <summary>
        ///     Sends all data in stream to response
        /// </summary>
        /// <param name="dataStream">The stream to copy data from</param>
        /// <param name="contentType">The mime type of the data in the stream</param>
        /// <param name="fileName">The filename that the browser should see the data as. Optional</param>
        /// <param name="attachment">Whether the file should be sent as attachment or inline</param>
        /// <param name="dispose">Whether to call dispose on stream when done sending</param>
        /// <param name="status">The status code for the response</param>
        public async Task<HandlerType> SendStream(Stream dataStream, string contentType, string fileName = "",
            bool attachment = false, bool dispose = true, HttpStatusCode status = HttpStatusCode.OK)
        {
            AspNetResponse.StatusCode = (int) status;
            AspNetResponse.ContentType = contentType;
            if (!string.IsNullOrEmpty(fileName))
                Headers["Content-disposition"] = $"{(attachment ? "attachment" : "inline")}; filename=\"{fileName}\"";

            await dataStream.CopyToAsync(AspNetResponse.Body);
            if (dispose) dataStream.Dispose();

            return HandlerType.Final;
        }

        /// <summary>
        ///     Sends file as response and requests the data to be displayed in-browser if possible
        /// </summary>
        /// <param name="filePath">The local path of the file to send</param>
        /// <param name="contentType">
        ///     The mime type for the file, when set to null, the system will try to detect based on file
        ///     extension
        /// </param>
        /// <param name="handleRanges">Whether to enable handling of range-requests for the file(s) served</param>
        /// <param name="fileName">Filename to show in header, instead of actual filename</param>
        /// <param name="status">The status code for the response</param>
        public async Task<HandlerType> SendFile(string filePath, string? contentType = null, bool handleRanges = true,
            string? fileName = null, HttpStatusCode status = HttpStatusCode.OK)
        {
            if (handleRanges) Headers["Accept-Ranges"] = "bytes";

            var fileSize = new FileInfo(filePath).Length;
            var range = Context.AspNetContext.Request.GetTypedHeaders().Range;
            var encodedFilename = WebUtility.UrlEncode(fileName ?? Path.GetFileName(filePath));

            Headers["Content-disposition"] = $"inline; filename=\"{encodedFilename}\"";

            if (range != null && range.Ranges.Any())
            {
                var firstRange = range.Ranges.First();
                if (range.Unit != "bytes" || !firstRange.From.HasValue && !firstRange.To.HasValue)
                {
                    await SendStatus(HttpStatusCode.BadRequest);
                    return HandlerType.Error;
                }

                var offset = firstRange.From ?? fileSize - firstRange.To ?? 0;
                var length = firstRange.To.HasValue
                    ? fileSize - offset - (fileSize - firstRange.To.Value)
                    : fileSize - offset;

                AspNetResponse.StatusCode = (int) HttpStatusCode.PartialContent;
                AspNetResponse.ContentType = Handlers.GetMimeType(contentType, filePath);
                AspNetResponse.ContentLength = length;
                Headers["Content-Range"] = $"bytes {offset}-{offset + length - 1}/{fileSize}";
                await AspNetResponse.SendFileAsync(filePath, offset, length, Aborted);
            }
            else
            {
                AspNetResponse.StatusCode = (int) status;
                AspNetResponse.ContentType = Handlers.GetMimeType(contentType, filePath);
                AspNetResponse.ContentLength = fileSize;
                await AspNetResponse.SendFileAsync(filePath, Aborted);
            }

            return HandlerType.Final;
        }

        /// <summary>
        ///     Sends file as response and requests the data to be downloaded as an attachment
        /// </summary>
        /// <param name="filePath">The local path of the file to send</param>
        /// <param name="fileName">The name filename the client receives the file with, defaults to using the actual filename</param>
        /// <param name="contentType">
        ///     The mime type for the file, when set to null, the system will try to detect based on file
        ///     extension
        /// </param>
        /// <param name="status">The status code for the response</param>
        public async Task<HandlerType> Download(string filePath, string? fileName = "", string? contentType = "",
            HttpStatusCode status = HttpStatusCode.OK)
        {
            AspNetResponse.StatusCode = (int) status;
            AspNetResponse.ContentType = Handlers.GetMimeType(contentType, filePath);
            var name = string.IsNullOrEmpty(fileName) ? Path.GetFileName(filePath) : fileName;
            Headers["Content-disposition"] = $"attachment; filename=\"{name}\"";
            await AspNetResponse.SendFileAsync(filePath, Aborted);
            return HandlerType.Final;
        }
    }
}