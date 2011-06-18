using System.IO;
using System.Reactive.Linq;
using Anna.Responses;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace Anna.Tests
{
    [TestFixture]
    public class ResponsesTests
    {
        [Test]
        public void WhenConvertingIntToResponse_ThenIsEmptyResponseWithStatusCode()
        {
            Response response = 404;

            response.Should().Be.OfType<EmptyResponse>()
                    .And
                    .ValueOf.StatusCode.Should().Be.EqualTo(404);
        }

        [Test]
        public void WhenConvertingStringToResponse_ThenIsStringResponse()
        {
            Response response = "hello";
            response.Should().Be.OfType<StringResponse>();
        }
    }
}
