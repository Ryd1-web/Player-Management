using Azure.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Player_Management.Migrations;
using Player_Management.Models;

namespace Player_Management.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class PlayerController : Controller
	{
		//private static List<Player> players = new List<Player>();
		//private static int playerIdCounter = 1;

		private readonly ApplicationDbContext _dbContext;

		public PlayerController(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}


		[HttpPost("player")]
		public IActionResult CreatePlayer(Player player)
		{
			_dbContext.Players.Add(player);
			_dbContext.SaveChanges();

			return Ok();
		}

		[HttpPut("player/{playerId}")]
		public IActionResult UpdatePlayer(int playerId, Player player)
		{
			var existingPlayer = _dbContext.Players.Find(playerId);

			if (existingPlayer == null)
			{
				return NotFound();
			}

			existingPlayer.Name = player.Name;
			existingPlayer.Position = player.Position;
			existingPlayer.PlayerSkills = player.PlayerSkills;

			_dbContext.SaveChanges();

			return Ok();
		}

		[Authorize]
		[HttpDelete("player/{playerId}")]
		public IActionResult DeletePlayer(int playerId)
		{

			var player = _dbContext.Players.Find(playerId);

			if (player == null)
			{
				return NotFound();
			}

			_dbContext.Players.Remove(player);
			_dbContext.SaveChanges();

			return Ok();
		}

		[HttpGet("player")]
		public IActionResult ListPlayers()
		{
			var players = _dbContext.Players.Include(p => p.PlayerSkills).ToList();

			return Ok(players);
		}

		// Select Best Team
		[HttpPost]
		[Route("api/team/process")]
		public IActionResult SelectBestTeam(TeamSelectionRequest teamSelections)
		{
			List<PlayerSelectionResponse> selectedPlayers = new List<PlayerSelectionResponse>();
			// Implement the selection logic according to the provided requirements
			foreach (var requirement in teamSelections.Requirements)
			{
				var position = requirement.Position;
				var mainSkill = requirement.MainSkill;
				var numberOfPlayers = requirement.NumberOfPlayers;

				var availablePlayers = _dbContext.Players
					.Where(p => p.Position == position)
					.OrderByDescending(p => p.PlayerSkills.FirstOrDefault(p => p.Name == mainSkill))
					.Take(numberOfPlayers)
					.ToList();

				if (availablePlayers.Count < numberOfPlayers)
				{
					return BadRequest($"Insufficient number of players for position: {position}");
				}

				selectedPlayers.AddRange(availablePlayers.Select(player => new PlayerSelectionResponse
				{
					Name = player.Name,
					Position = player.Position,
					PlayerSkills = player.PlayerSkills
				}));
			}

			return Ok(selectedPlayers);
		}
	}


	public class TeamSelectionRequest
	{
		public List<PositionRequirement> Requirements { get; set; }
	}

	public class PositionRequirement
	{
		public string Position { get; set; }
		public string MainSkill { get; set; }
		public int NumberOfPlayers { get; set; }
	}

	public class PlayerSelectionResponse
	{
		public string Name { get; set; }
		public string Position { get; set; }
		public List<Skill> PlayerSkills { get; set; }
	}

}
