namespace ConCord.Models;

public class Channel
{
  public Channel(int id, string name, DateTime created)
  {
    Id = id;
    Name = name;
    Created = created;
  }
  public int Id { get; set; }
  public string Name { get; set; }
  public DateTime Created { get; set; }
  public List<Message>? Messages { get; set; }
}