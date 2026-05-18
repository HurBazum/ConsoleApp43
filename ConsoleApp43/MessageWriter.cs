using System.Buffers;
using System.Buffers.Binary;
using ConsoleApp43.Messages;
using System.Net.Sockets;
using System.Text.Json;

namespace ConsoleApp43;
public class MessageWriter
{
    public async Task WriteAsync(Socket socket, MessageBase message)
    {
        var writer = new ArrayBufferWriter<byte>();

        using(Utf8JsonWriter jsonWriter = new(writer))
        {
            JsonSerializer.Serialize(jsonWriter, message);
        };

        var payloadLength = writer.WrittenCount;            
        var buffer = ArrayPool<byte>.Shared.Rent(4 + payloadLength);

        BinaryPrimitives.WriteInt32BigEndian(buffer, payloadLength);

        writer.WrittenSpan.CopyTo(buffer.AsSpan(4));

        try            
        {
            await socket.SendAsync(buffer.AsMemory(0, 4 + payloadLength), SocketFlags.None);
        }
        catch(SocketException ex)
        {
            Console.WriteLine($"Socket error: {ex.Message}");
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }
}