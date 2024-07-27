using System;
using ExRam.Gremlinq.Core;
using ExRam.Gremlinq.Providers.Core;
using ExRam.Gremlinq.Providers.CosmosDb;
using ExRam.Gremlinq.Support.NewtonsoftJson;
using Shouldly;
using Xunit;

namespace GremlinqGuidDebugIssue.Tests;

public class GuidDebugTests
{
    private readonly IGremlinQuerySource _g;

    public GuidDebugTests()
    {
        _g = GremlinQuerySource.g
            .UseCosmosDb<Vertex, Edge>(
                configurator => configurator
                                .At(new Uri("wss://your.cosmosdb.endpoint/"))
                                .OnDatabase("your database name")
                                .OnGraph("your graph name")
                                .WithPartitionKey(x => x.PartitionKey!)
                                .AuthenticateBy("your auth key")
                                .UseNewtonsoftJson());
    }

    [Fact]
    public void Debug_Fails_To_Enclose_Guid_In_Single_Quotes()
    {
        var expectedQuery = "g.V().hasLabel('Dog').has('ShelterId','b7219a3b-783e-4293-b6d2-ba74f9335bff')";
        var generatedQuery = _g.V<Dog>().Where(dog => dog.ShelterId == Constants.LocalShelterId.ToString()).Debug();
        generatedQuery.ShouldBe(expectedQuery);
    }

    [Fact]
    public void Debug_Correctly_Encloses_Lowercase_Guid_string_In_Single_Quotes()
    {
        var expectedQuery = "g.V().hasLabel('Dog').has('ShelterId','b7219a3b-783e-4293-b6d2-ba74f9335bff')";
        var generatedQuery = _g.V<Dog>().Where(dog => dog.ShelterId == Constants.LocalShelterId.ToString().ToLower()).Debug();
        generatedQuery.ShouldBe(expectedQuery);
    }
}