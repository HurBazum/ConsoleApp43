using System.Buffers.Binary;
using System.Buffers;

namespace P43.Lib;
public class Stream_Reader : IDisposable
{
    public int Len { get; private set; } = 0;
    public byte[]? Data { get; private set; } = null;
    
    public Func<RentedBufferOwner, Task>? OnComplete;
    
    public int CurrentPosition { get; private set; } = 0;
    private int _headerPosition = 0;
    private int _bytesReceived = 0;
    private readonly byte[] lengthBuffer = new byte[4];
    
    public State State { get; private set; } = State.ReadingHeader;
    
    public void Parse(ReadOnlySpan<byte> data)
    {
        while(data.Length > 0)
        {
            if(State == State.ReadingHeader)
            {
                int need = 4 - _headerPosition;
                int canTake = Math.Min(need, data.Length);
                
                data.Slice(0, canTake).CopyTo(lengthBuffer.AsSpan(_headerPosition));
                
                _headerPosition += canTake;
                
                data = data.Slice(canTake);
                
                if(_headerPosition == 4)
                {
                    Len = BinaryPrimitives.ReadInt32BigEndian(lengthBuffer);

                    if(Len <= 0 || Len > 1024 * 1024)
                    {
                        break;
                    }

                    Data = ArrayPool<byte>.Shared.Rent(Len);
                    State = State.ReadingBody;
                    _bytesReceived = 0;
                }
            }
            else if(State == State.ReadingBody)
            {
                int need = Len - _bytesReceived;
                int canTake = Math.Min(need, data.Length);
                
                data.Slice(0, canTake).CopyTo(Data.AsSpan(_bytesReceived));
                
                _bytesReceived += canTake;
                
                data = data.Slice(canTake);
                
                if(_bytesReceived == Len)
                {
                    RentedBufferOwner buffer = new(Data, Len);
                    Data = null;
                    OnComplete?.Invoke(buffer);
                        
                    State = State.ReadingHeader;
                    _headerPosition = 0;
                    Len = 0;
                }
            }
        }
    }
    
    public void Dispose()
    {
        OnComplete = null;
        if(Data != null)
        {
            ArrayPool<byte>.Shared.Return(Data);
            Data = null;
        }
    }
}

public struct RentedBufferOwner(byte[] array, int offset) : IMemoryOwner<byte>
{
    private readonly byte[] _array = array;
    private readonly int _offset = offset;

    public readonly Memory<byte> Memory => _array.AsMemory(0, _offset);

    public void Dispose()
    {
        if(_array != null)
        {
            ArrayPool<byte>.Shared.Return(_array);
        }
    }
}