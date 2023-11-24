using Godot;
using System;

public partial class ik_target : Marker3D
{
	[Export] Node3D stepTarget;
	[Export] float stepDistance = 3;

	[Export] Node3D adjacentTarget;

	[Export] Node3D oppositeTarget;


	Boolean isStepping = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{ 
		if (!isStepping && !(bool)adjacentTarget.Get("isStepping") && Math.Abs(GlobalPosition.DistanceTo(stepTarget.GlobalPosition)) > stepDistance)
		{
			Step();
			oppositeTarget.Call("Step");
		}	
	}
	public void Step()
	{
		Vector3 targetPos = stepTarget.GlobalPosition;
		Vector3 halfWay = (GlobalPosition + stepTarget.GlobalPosition) / 2;
		isStepping = true;

		Tween tween = GetTree().CreateTween();


		tween.TweenProperty(this, "position", halfWay + Basis.Y, 0.1);
		tween.TweenProperty(this, "position", targetPos, 0.1);
		tween.TweenCallback(Callable.From(() => isStepping = false));

		;
	}
}
