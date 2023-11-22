using Godot;
using System;
public partial class third_person_template : CharacterBody3D
{	
	//Exported variables so we can change them in editor
	[Export(PropertyHint.Range, "0.1,1.0")] float verticalCamSensitivity = 0.7f;
	[Export(PropertyHint.Range, "0.1,1.0")] float horizontalCamSensitivity = 0.3f;
	[Export(PropertyHint.Range, "-90,0,1")] float minCamPitch = -50f;
	[Export(PropertyHint.Range, "0,90,1")] float maxCamPitch = 30f;

	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	//Function runs when player scene its ready
	public override void _Ready()
	{
		base._Ready();
		Console.WriteLine("Hello from Cs with Console");
		//Make mouse not visible and limit it to the game screen
		Input.MouseMode = Input.MouseModeEnum.Captured;

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
	
			RotateY(mathTools.DegToRad(mouseMotion.Relative.X * horizontalCamSensitivity));
			
		}
		
		//Prevents camera from going endlessly around the player
		camRot.X = Mathf.Clamp(camRot.X, minCamPitch, maxCamPitch);
		//Update Camera arm mount current rotation
		GetNode<Node3D>("Camera_Mount").RotationDegrees = camRot;
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
}

