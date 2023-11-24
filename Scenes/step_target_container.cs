using Godot;
using System;

public partial class step_target_container : Node3D
{
	[Export] float offset = 20;
	Node3D parent;
	Vector3 previousPosition;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		parent = GetParentNode3D();
		previousPosition = parent.GlobalPosition;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector3 velocity = parent.GlobalPosition - previousPosition;
		GlobalPosition = parent.GlobalPosition + velocity * offset;
		previousPosition = parent.GlobalPosition;
		
	}
}
