using System;

public abstract class Constants {
	public const string AI_HEALTH_VAR = "health"; //current health
	public const string AI_PLAYER_TARGET_VAR = "playerTarget"; //current player target
	public const string AI_MELEE_TARGET_VAR = "meleeTarget"; //current melee target
	public const string AI_LAST_TARGET_POS_VAR = "lastTargetPos"; //last known position of playerTarget
	public const string AI_GOT_HIT_VAR = "gotHit"; //true after getting hit
	public const string AI_LAST_HIT_POS_VAR = "lastHitPos"; //position from which the last hit came
	public const string AI_LAST_HIT_SOURCE_VAR = "lastHitSource"; //source from which the last hit came
	public const string AI_DEAD_VAR = "dead"; //true when the ai is dead
	
	public const string ASPECT_PLAYER = "player";
	public const string ASPECT_DEAD_PLAYER = "deadPlayer";
}

