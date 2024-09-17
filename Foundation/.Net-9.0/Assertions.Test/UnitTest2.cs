using System;
using Newtonsoft.Json;
using Xunit;

namespace Foundation.Assertions.Test;

public class UnitTest2
{
    [Fact]
    public void Test1()
    {
        // Arrange
        SampleDataStructure sampleDataStructure = new SampleDataStructure(Guid.NewGuid(), DateTime.Today, "content");

        // Act
        string serializedDataStructure = JsonConvert.SerializeObject(sampleDataStructure);
        string serializedDataStructure2 = System.Text.Json.JsonSerializer.Serialize(sampleDataStructure);

        // Assert
        Xunit.Assert.Equal(serializedDataStructure, serializedDataStructure2);
    }
}

public record SampleDataStructure(Guid Id,DateTime Occured, string Content);