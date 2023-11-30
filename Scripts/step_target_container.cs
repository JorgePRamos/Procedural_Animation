using Godot;
using System;

public partial class step_target_container : Node3D
{   //Movement offset
	[Export] float offset = 20;
	//Parent node to target_step_container --> rob_model
	Node3D parent;
	Vector3 previousPosition;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//On ready get the robots position as the previousPosition
		parent = GetParentNode3D();
		previousPosition = parent.GlobalPosition;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//Velocity = current - previous (position)
		Vector3 velocity = parent.GlobalPosition - previousPosition;
		// Containers globalPos  = parent.Current + calc velocity * offset
		GlobalPosition = parent.GlobalPosition + velocity * offset;
		//Update previousPosition variable for next iteration
		previousPosition = parent.GlobalPosition;

	}
}
