using System;

[Serializable]
public class MovementsSettings
{
    public float MAX_WALK_SPEED;
    public float WALK_FORCE;
    public float JUMP_FORCE;

    public float GRAVITY;
}

[Serializable]
public class StickSettings
{
    public float ROTATE_SPEED;
    public float DAMPING;
}