using Godot;
using System;
[Tool]
public partial class start_ik : SkeletonIK3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		Start();	
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
