using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterState
{
	public enum CharacterCondition
	{
		Normal,
		Paused,
		Dead,
		Stunned
	}

	public enum MovementState
	{
		Null,
		Idle,
		Walking,
		Crouching,
		Dashing,
		Jumping,
		DoubleJumping,
		FollowingPath,
		LedgeClimbing,
		Rolling,
	}
}
