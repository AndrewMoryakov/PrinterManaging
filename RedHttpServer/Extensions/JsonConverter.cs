using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Red.Interfaces;

namespace Red.Extensions
{
    /// <summary>
    ///     Very simple JsonConverter plugin using System.Text.Json generic methods
    /// </summary>
    internal sealed class JsonConverter : IJsonConverter, IRedExtension
    {
        /// <inheritdoc />
        public string? Serialize<T>(T obj)
        {
            try
            {
                return JsonSerializer.Serialize(obj);
            }
            catch (JsonException)
            {
                return default;
            }
        }

        /// <inheritdoc />
        public T Deserialize<T>(string jsonData)
            where T : class
        {
            try
            {
                return !string.IsNullOrEmpty(jsonData)
                    ? JsonSerializer.Deserialize<T>(jsonData)
                    : default;
            }
            catch (JsonException)
            {
                return default;
            }
        }

        /// <inheritdoc />
        public async Task<T> DeserializeAsync<T>(Stream jsonStream, CancellationToken cancellationToken = default)
            where T : class
        {
            try
            {
                return await JsonSerializer.DeserializeAsync<T>(jsonStream, cancellationToken: cancellationToken);
            }
            catch (JsonException)
            {
                return default;
            }
        }

        /// <inheritdoc />
        public async Task SerializeAsync<T>(T obj, Stream jsonStream, CancellationToken cancellationToken = default)
        {
            try
            {
                await JsonSerializer.SerializeAsync(jsonStream, obj, cancellationToken: cancellationToken);
            }
            catch (JsonException)
            {
            }
        }

        public void Initialize(RedHttpServer server)
        {
            server.Plugins.Register<IJsonConverter, JsonConverter>(this);
        }
    }
}