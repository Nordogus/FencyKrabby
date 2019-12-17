using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Camille

public enum turtelStartOrient
{
    Random,
    North,
    South,
    East,
    West
}

public class Turtel : MonoBehaviour
{
    public turtelStartOrient startOrient = turtelStartOrient.Random;
    public List<Transform> path = new List<Transform>();
    private int nextPos = 1;

    private float load = 0f;
    public float moveSpeed = 1f;
    public float timeOfImobility = 5f;

    public bool canMove = false;

    public bool debugMode = false;

    // Start is called before the first frame update
    void Start()
    {
        Party.instance.entity.Add(gameObject);

        switch (startOrient)
        {
            case turtelStartOrient.Random:
                Rotat(Random.Range(0, 3));
                break;
            case turtelStartOrient.North:
                Rotat(0);
                break;
            case turtelStartOrient.South:
                Rotat(1);
                break;
            case turtelStartOrient.East:
                Rotat(2);
                break;
            case turtelStartOrient.West:
                Rotat(3);
                break;
        }

        if (!canMove) return;
        transform.position = path[0].transform.position;
        if (path.Count < 2)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove && load < 1f)
        {
            _MoveTo(path[nextPos].transform);
        }
    }

    private void _MoveTo(Transform _transform)
    {
        load += moveSpeed * Time.deltaTime;
        transform.position =  Vector3.Lerp(transform.position, _transform.position, load);
        if (load >= 1f)
        {
            nextPos++;
            if (nextPos >= path.Count)
            {
                nextPos = 0;
            }

            StartCoroutine(TimePose());
        }
    }

    private IEnumerator TimePose()
    {
        yield return new WaitForSeconds(timeOfImobility);
        load = 0f;
    }

    public bool GetImobility()
    {
        if (!canMove || load >= 1f)
        {
            return true;
        }
        return false;
    }

    public void Rotat(PlayerEntity player)
    {
        switch (player.clawDirection)
        {
            case PlayerClawDirection.North:
                transform.rotation = Quaternion.Euler(-25, 45, -25);
                break;
            case PlayerClawDirection.South:
                transform.rotation = Quaternion.Euler(25, 220, 25);
                break;
            case PlayerClawDirection.East:
                transform.rotation = Quaternion.Euler(20, 130, -20);
                break;
            case PlayerClawDirection.West:
                transform.rotation = Quaternion.Euler(-20, 310, 20);
                break;
            default:
                transform.rotation = Quaternion.Euler(-25, 45, -25);
                break;
        }
    }

    public void Rotat(int direct)
    {
        switch (direct)
        {
            case 0:
                transform.rotation = Quaternion.Euler(-25, 45, -25);
                break;
            case 1:
                transform.rotation = Quaternion.Euler(25, 220, 25);
                break;
            case 2:
                transform.rotation = Quaternion.Euler(20, 130, -20);
                break;
            case 3:
                transform.rotation = Quaternion.Euler(-20, 310, 20);
                break;
            default:
                transform.rotation = Quaternion.Euler(-25, 45, -25);
                break;
        }
    }

    private void OnGUI()
    {
        if (!debugMode) { return; }

        GUILayout.BeginVertical();
        GUILayout.Label("                        load : " + load);
        GUILayout.EndVertical();
    }
}
