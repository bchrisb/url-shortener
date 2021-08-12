using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using Moq;
using Newtonsoft.Json;

namespace UrlShortener.UnitTests
{
    public class TestBase
    {
        private MemoryStream _memoryStream;

        protected HttpRequest CreateMockRequest(
            object body = null,
            Dictionary<string, StringValues> headers = null,
            Dictionary<string, StringValues> queryStringParams = null,
            string contentType = null)
        {
            var mockRequest = new Mock<HttpRequest>();

            if (body != null)
            {
                var json = JsonConvert.SerializeObject(body);
                var byteArray = Encoding.UTF8.GetBytes(json);

                _memoryStream = new MemoryStream(byteArray);
                _memoryStream.Flush();
                _memoryStream.Position = 0;

                mockRequest.Setup(x => x.Body).Returns(_memoryStream);
            }

            if (headers != null)
            {
                mockRequest.Setup(i => i.Headers).Returns(new HeaderDictionary(headers));
            }

            if (queryStringParams != null)
            {
                mockRequest.Setup(i => i.Query).Returns(new QueryCollection(queryStringParams));
            }

            if (contentType != null)
            {
                mockRequest.Setup(i => i.ContentType).Returns(contentType);
            }

            mockRequest.Setup(i => i.HttpContext).Returns(new DefaultHttpContext());

            return mockRequest.Object;
        }
    }
}
