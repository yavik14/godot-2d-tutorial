using Godot;
using System;

public partial class Main : Node
{
	[Export]
	public PackedScene MobScene { get; set; }
	
	private int _score;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	
	public void GameOver()
	{
		GetNode<Timer>("MobTimer").Stop();
		GetNode<Timer>("ScoreTimer").Stop();
		
		GetNode<HUD>("HUD").ShowGameOver();
		
		GetNode<AudioStreamPlayer>("Music").Stop();
		GetNode<AudioStreamPlayer>("DeathSound").Play();
	}
	
	public void NewGame()
	{
		GetTree().CallGroup("mobs", Node.MethodName.QueueFree);
		
		_score = 0;
		
		var player = GetNode<Player>("Player");
		var startPosition = GetNode<Marker2D>("StartPosition");
		player.Start(startPosition.Position);
		
		GetNode<Timer>("StartTimer").Start();	
		
		var hud = GetNode<HUD>("HUD");
		hud.UpdateScore(_score);
		hud.ShowMessage("Get Ready!");
		
		GetNode<AudioStreamPlayer>("Music").Play();
	}
	
	private void OnStartTimerTimeout()
	{
		GetNode<Timer>("MobTimer").Start();
		GetNode<Timer>("ScoreTimer").Start();
	}

	private void OnScoreTimerTimeout()
	{
		_score++;
		GetNode<HUD>("HUD").UpdateScore(_score);
	}

	private void OnMobTimerTimeout()
	{
		// Create a new instance of the Mob scene.
		Mob mob = MobScene.Instantiate<Mob>();
		
		// Choose a random location on Path2D
		var mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
		mobSpawnLocation.ProgressRatio = GD.Randf();
		
		// Set the momb's direction perpendicular to de path direction
		float direction = mobSpawnLocation.Rotation + Mathf.Pi /2;
		mob.Rotation = direction;
		
		// Set the mob's position to a random location.
		mob.Position = mobSpawnLocation.Position;
		
		// Add some randomness to the direction
		direction += (float)GD.RandRange(-Mathf.Pi / 4, Mathf.Pi/ 4);
		
		// Choos the velocity
		var velocity = new Vector2((float)GD.RandRange(150.0, 250.0), 0);
		mob.LinearVelocity = velocity.Rotated(direction);
		
		// Spawn the mob by adding it to the Main scene.
		AddChild(mob);
	}
	
}
