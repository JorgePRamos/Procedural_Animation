using Godot;
using System;

public partial class step_ray : RayCast3D
{
	[Export] Node3D stepTarget;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		Vector3 hitPoint = GetCollisionPoint();
		stepTarget.GlobalPosition = hitPoint;


	}
}
