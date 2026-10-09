using System.Text.Json;
using System.Text.Json.Serialization;
using EImzo.Client.Serialization;
using Xunit;

namespace EImzo.Client.Tests;

public class FlexibleDateTimeConverterTests
{
    private class TestModel
    {
        [JsonConverter(typeof(FlexibleDateTimeConverter))]
        public DateTime? Date { get; set; }
    }

    [Theory]
    [InlineData("{\"Date\":\"2022-10-06 16:47:29\"}", 2022, 10, 6, 16, 47, 29)]
    [InlineData("{\"Date\":\"2025.10.21 11:13:57\"}", 2025, 10, 21, 11, 13, 57)]
    [InlineData("{\"Date\":\"2022-09-24T17:29:21\"}", 2022, 9, 24, 17, 29, 21)]
    [InlineData("{\"Date\":\"2022-09-24\"}", 2022, 9, 24, 0, 0, 0)]
    public void Read_VariousFormats_ShouldParseCorrectly(string json, int year, int month, int day, int hour, int min, int sec)
    {
        var model = JsonSerializer.Deserialize<TestModel>(json);
        Assert.NotNull(model);
        Assert.NotNull(model.Date);
        Assert.Equal(new DateTime(year, month, day, hour, min, sec), model.Date.Value);
    }

    [Fact]
    public void Read_NullOrEmpty_ShouldReturnNull()
    {
        var model1 = JsonSerializer.Deserialize<TestModel>("{\"Date\":null}");
        Assert.NotNull(model1);
        Assert.Null(model1.Date);

        var model2 = JsonSerializer.Deserialize<TestModel>("{\"Date\":\"\"}");
        Assert.NotNull(model2);
        Assert.Null(model2.Date);
    }
}
