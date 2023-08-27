using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterJump : CharacterAbility
{
	public enum JumpBehaviour
	{
		CanJumpOnGround,
		CanJumpAnyWhere
	}

	[Header("Jump")]
	[SerializeField] private int _numberOfJumps = 2;
	[SerializeField] private float _jumpHeight = 2f;
	[SerializeField] private JumpBehaviour _jumpBehaviour = JumpBehaviour.CanJumpAnyWhere;
	[SerializeField] private bool _canJumpDownOnWayPlatform = true;
	
	[Header("Quality of Life")]
	[SerializeField] private float _coyoteTime;
	[SerializeField] private int _inputBufferFrame;

	
}
