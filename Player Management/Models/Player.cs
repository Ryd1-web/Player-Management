namespace Player_Management.Models
{
	public class Player
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Position { get; set; }
		public List<Skill> PlayerSkills { get; set; }
	}

	public class Skill
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int Value { get; set; }
		public int PlayerId { get; set; }
	}
}
