using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveHorisontal
{
    East,
    West
}

public enum MoveVertical
{
    North,
    South
}

public class Sponge : MonoBehaviour
{
    public MoveHorisontal moveHorisontal;
    public MoveVertical moveVertical;
    [HideInInspector]
    public PlayerClawDirection[] orients = new PlayerClawDirection[2];

    public void Start()
    {
        if (moveHorisontal == MoveHorisontal.East)
        {
            orients[0] = PlayerClawDirection.East;
        }
        else
        {
            orients[0] = PlayerClawDirection.West;
        }

        if (moveVertical == MoveVertical.North)
        {
            orients[1] = PlayerClawDirection.North;
        }
        else
        {
            orients[1] = PlayerClawDirection.South;
        }
    }

    public PlayerClawDirection newOrientClaw(PlayerClawDirection clawDirection)
    {
        if (clawDirection == PlayerClawDirection.North || clawDirection == PlayerClawDirection.South)
        {
            if (moveHorisontal == MoveHorisontal.East)
            {
                return PlayerClawDirection.East;
            }
            else
            {
                return PlayerClawDirection.West;
            }
        }
        else
        {
            if (moveVertical == MoveVertical.North)
            {
                return PlayerClawDirection.North;
            }
            else
            {
                return PlayerClawDirection.South;
            }
        }
    }

    public bool Stop(Vector2 currentPos)
    {
        Vector2 tmpPos = currentPos;
        tmpPos.x -= transform.position.x;
        tmpPos.y -= transform.position.y;

        float ABDotAC;

        if (moveVertical == MoveVertical.North)
        {
            if (moveHorisontal == MoveHorisontal.East)
            {
                ABDotAC = Vector2.Dot(Vector2.right, tmpPos);
            }
            else
            {
                ABDotAC = Vector2.Dot(Vector2.up, tmpPos);
            }
        }
        else
        {
            if (moveHorisontal == MoveHorisontal.East)
            {
                ABDotAC = Vector2.Dot(Vector2.down, tmpPos);
            }
            else
            {
                ABDotAC = Vector2.Dot(Vector2.left, tmpPos);
            }
        }

        if (ABDotAC > 0)
        {
            return false;
        }
        return true;
    }
}
