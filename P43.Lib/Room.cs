namespace P43.Lib;
public class Room
{
    public Guid OwnerId { get; private set; }
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public HashSet<Guid> _members { get; private set; } = new();
    public int MemberCount => _members.Count;


    public static Room Create(string name, Guid ownerId) => new()
    {
        Id = Guid.NewGuid(),
        Name = name,
        OwnerId = ownerId
    };

    public void AddMember(Guid memberId) =>_members.Add(memberId);

    public void RemoveMember(Guid memberId) => _members.Remove(memberId);

    public override bool Equals(object? obj)
    {
        if(obj is not Room room)
        {
            return false;
        }
        if(this.Id == room.Id && this.Name == room.Name)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}