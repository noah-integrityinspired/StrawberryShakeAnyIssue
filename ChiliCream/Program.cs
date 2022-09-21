using Microsoft.Extensions.DependencyInjection;
using Test;
using StrawberryShake;
using StrawberryShake.Serialization;
using System.Text.Json;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection
            .AddConferenceClient()
            .ConfigureHttpClient(client => client.BaseAddress = new Uri("https://workshop.chillicream.com/graphql"));

        IServiceProvider services = serviceCollection.BuildServiceProvider();

        IConferenceClient client = services.GetRequiredService<IConferenceClient>();

        var result = await client.GetSessions.ExecuteAsync();
        result.EnsureNoErrors();

        foreach (var session in result.Data.Sessions.Nodes)
        {
            Console.WriteLine(session.Title);
        }

        Console.ReadLine();
    }

    public class MyJsonSerializer : ScalarSerializer<JsonElement, object>
    {
        public MyJsonSerializer(string typeName = BuiltInScalarNames.Any)
            : base(typeName)
        {
        }
        public override object Parse(JsonElement serializedValue)
        {
            return serializedValue.Deserialize<object>();
        }
        protected override JsonElement Format(object runtimeValue)
        {
            return System.Text.Json.JsonSerializer.SerializeToElement(runtimeValue);
        }
    }
}