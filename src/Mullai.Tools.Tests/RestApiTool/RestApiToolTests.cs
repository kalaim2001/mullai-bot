using Moq;
using Mullai.Tools.RestApiTool;
using RestApiTool = Mullai.Tools.RestApiTool.RestApiTool;
using Mullai.Tools.RestApiTool.Models;
using System.Net;

namespace Mullai.Tools.Tests.RestApi;

public class RestApiToolTests
{
    private readonly Mock<Mullai.Tools.RestApiTool.RestApiProvider> _providerMock;
    private readonly Mullai.Tools.RestApiTool.RestApiTool _tool;

    public RestApiToolTests()
    {
        // RestApiProvider needs an HttpClient in the constructor, 
        // but we are mocking the whole provider.
        _providerMock = new Mock<Mullai.Tools.RestApiTool.RestApiProvider>(new HttpClient());
        _tool = new Mullai.Tools.RestApiTool.RestApiTool(_providerMock.Object);
    }

    [Fact]
    public async Task SendRequestAsync_CallsProvider()
    {
        // Arrange
        var url = "https://api.example.com";
        var method = "GET";
        var expectedResponse = new RestApiResponse
        {
            IsSuccess = true,
            StatusCode = HttpStatusCode.OK,
            Content = "test content"
        };

        _providerMock.Setup(p => p.SendAsync(It.IsAny<RestApiRequest>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _tool.SendRequestAsync(url, method);

        // Assert
        Assert.Equal(expectedResponse.Content, result.Content);
        _providerMock.Verify(p => p.SendAsync(It.Is<RestApiRequest>(r => 
            r.Url == url && r.Method == method)), Times.Once);
    }
}
