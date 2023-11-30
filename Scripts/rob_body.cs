using Godot;
using System;

public partial class rob_body : Node3D
{
	[Export]float hightOffset = 0.6f;
	Marker3D frontLIK;
	Marker3D frontRIK;
	Marker3D backLIK;
	Marker3D backRIK;
	CharacterBody3D charBody;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	//Get the IK markers
		frontLIK = GetNode<Marker3D>("Front_L_MK");
		frontRIK = GetNode<Marker3D>("Front_R_MK");
		backLIK = GetNode<Marker3D>("Back_L_MK");
		backRIK = GetNode<Marker3D>("Back_R_MK");

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{//backLIK,frontLIK,frontRIK
		Plane plane1 = new Plane(backLIK.GlobalPosition,frontLIK.GlobalPosition,frontRIK.GlobalPosition);
		Plane plane2 = new Plane(frontRIK.GlobalPosition,backRIK.GlobalPosition,backLIK.GlobalPosition);
		Vector3 avgNormal = ((plane1.Normal + plane2.Normal)/2).Normalized();

		//charBody = GetNode<CharacterBody3D>("Player");
		//float moveSpeed = (float)charBody.Get("Speed");
		float moveSpeed =7;
		Basis targetBasis = basisFromNormal(avgNormal);
		Basis = Basis.Slerp(targetBasis, (float)(moveSpeed*delta)).Orthonormalized();
		
		Vector3 avg  = (frontLIK.Position + frontRIK.Position +backRIK.Position+ backLIK.Position)/4;
		Vector3 target = avg + Basis.Y * hightOffset;
		float distance = Basis.Y.Dot(target-Position);
		Position = Position.Slerp(Position + Transform.Basis.Y * distance,(float)(moveSpeed*delta));

	}

	public Basis basisFromNormal(Vector3 normal){
		Basis result = new Basis();

		result.X = normal.Cross(Transform.Basis.Z);
		result.Y = normal;
		result.Z = Transform.Basis.X.Cross(normal);

		result = result.Orthonormalized();
		result.X *= Scale.X; 
		result.Y *= Scale.Y;
		result.Z *= Scale.Z;


		return result;
	}
}
