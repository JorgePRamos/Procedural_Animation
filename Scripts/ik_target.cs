using Godot;
using System;

public partial class ik_target : Marker3D
	{//Step target to which calc the distance
	[Export] Node3D stepTarget;
	//Stride distance
	[Export] float stepDistance = 3;
	//Side target
	[Export] Node3D adjacentTarget;
	//Diagonally opposite
	[Export] Node3D oppositeTarget;

	Boolean isStepping = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
		{//Step if distance between target and current is greater that stride
		if (!isStepping && !(bool)adjacentTarget.Get("isStepping") && Math.Abs(GlobalPosition.DistanceTo(stepTarget.GlobalPosition)) > stepDistance)
		{
			Step();
			oppositeTarget.Call("Step");
		}
	}
	public void Step()
	{	//Target position
		Vector3 targetPos = stepTarget.GlobalPosition;
		//Half way yo target from current
		Vector3 halfWay = (GlobalPosition + stepTarget.GlobalPosition) / 2;
		isStepping = true;

		Tween tween = GetTree().CreateTween();

		//Interpolate to half way position + bot.y (1m in the y axes)
		tween.TweenProperty(this, "position", halfWay + Basis.Y, 0.1);
		//Interpolate to target from half way
		tween.TweenProperty(this, "position", targetPos, 0.1);
		tween.TweenCallback(Callable.From(() => isStepping = false));

		;
	}
}
