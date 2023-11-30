using Godot;
using System;
public partial class third_person_template : CharacterBody3D
{
	//Exported variables so we can change them in editor
	[Export(PropertyHint.Range, "0.1,1.0")] float verticalCamSensitivity = 0.7f;
	[Export(PropertyHint.Range, "0.1,1.0")] float horizontalCamSensitivity = 0.3f;
	[Export(PropertyHint.Range, "-90,0,1")] float minCamPitch = -50f;
	[Export(PropertyHint.Range, "0,90,1")] float maxCamPitch = 30f;

	//Body distance from ground
	[Export] float hightOffset = 0.5f;
	//Store the ik leg nodes
	Marker3D frontLIK;
	Marker3D frontRIK;
	Marker3D backLIK;
	Marker3D backRIK;
	CharacterBody3D charBody;
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	//Function runs when player scene its ready
	public override void _Ready()
	{
		base._Ready();
		//Make mouse not visible and limit it to the game screen
		Input.MouseMode = Input.MouseModeEnum.Captured;
		//Fetch IK targets from nested scene
		frontLIK = GetNode<Node3D>("Visuals").GetNode<Node3D>("rob_model").GetNode<Marker3D>("Front_L_MK");
		frontRIK = GetNode<Node3D>("Visuals").GetNode<Node3D>("rob_model").GetNode<Marker3D>("Front_R_MK");
		backLIK = GetNode<Node3D>("Visuals").GetNode<Node3D>("rob_model").GetNode<Marker3D>("Back_L_MK");
		backRIK = GetNode<Node3D>("Visuals").GetNode<Node3D>("rob_model").GetNode<Marker3D>("Back_R_MK");

	}
	//Accumulators
	private float _rotationX = 0f;
	private float _rotationY = 0f;

	//Mouse rotation
	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		//Get Camera arm mount current rotation
		Vector3 camRot = GetNode<Node3D>("Camera_Mount").RotationDegrees;

		if (@event is InputEventMouseMotion mouseMotion)
		{
			//Orbit character
			//camRot.Y -= mouseMotion.Relative.X * camSensitivity;
			camRot.X -= mouseMotion.Relative.Y * verticalCamSensitivity;
			//Rotation in Y negative to follow mouse
			RotateY(mathTools.DegToRad(-(mouseMotion.Relative.X * horizontalCamSensitivity)));

		}

		//Prevents camera from going endlessly around the player
		camRot.X = Mathf.Clamp(camRot.X, minCamPitch, maxCamPitch);
		//Update Camera arm mount current rotation
		GetNode<Node3D>("Camera_Mount").RotationDegrees = camRot;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		//Create 2 planes defined by legs IK positions
		Plane plane1 = new Plane(backLIK.GlobalPosition, frontLIK.GlobalPosition, frontRIK.GlobalPosition);
		Plane plane2 = new Plane(frontRIK.GlobalPosition, backRIK.GlobalPosition, backLIK.GlobalPosition);
		//Find average between the two to get body target orientation 
		Vector3 avgNormal = ((plane1.Normal + plane2.Normal) / 2).Normalized();

		//Convert normal in to a transformation based expression
		Basis targetBasis = basisFromNormal(avgNormal);
		Basis = Basis.Slerp(targetBasis, (float)(Speed * delta)).Orthonormalized();

		//Average between legs
		Vector3 avg = (frontLIK.Position + frontRIK.Position + backRIK.Position + backLIK.Position) / 4;
		//Calculate target by adding the body y direction times the set offset
		Vector3 target = avg + Basis.Y * hightOffset;
		
		//Distance between current and target
		float distance = Basis.Y.Dot(target - Position);

		//Interpolate current position and target
		Vector3 targetPosition = Position.Slerp(Position + Transform.Basis.Y * distance, (float)(Speed * delta));
		Position = targetPosition;


	}
	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.Y = JumpVelocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("Left", "Right", "Forward", "Back");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
	//Translates normal to basis
	public Basis basisFromNormal(Vector3 normal)
	{
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

