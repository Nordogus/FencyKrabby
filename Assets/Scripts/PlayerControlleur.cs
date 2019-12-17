using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

// Camille

public class PlayerControlleur : MonoBehaviour
{
    public int maxPlayer = 4;        // nombre max de joueur dans une partie

    public List<PlayerEntity> entity = new List<PlayerEntity>();    // liste des crabs en jeu
    private List<Player> _rewiredPlayers = new List<Player>();  // liste des controleur
    [SerializeField]private bool pushInContinu = false;

    // Start is called before the first frame update
    void Start()
    {
        if (maxPlayer > entity.Count)
        {
            maxPlayer = entity.Count;
        }

        for (int i = 0; i < maxPlayer; i++)
        {
            _rewiredPlayers.Add(ReInput.players.GetPlayer(i));
        }
    }

    void Update()
    {
        for (int i = 0; i < maxPlayer; i++)
        {
            float dirX = _rewiredPlayers[i].GetAxis("MoveHorizontal");
            float dirY = _rewiredPlayers[i].GetAxis("MoveVertical");
            float clawDirX = _rewiredPlayers[i].GetAxis("ClawMoveHorizontal");
            float clawDirY = _rewiredPlayers[i].GetAxis("ClawMoveVertical");


            entity[i].Move(new Vector2(dirX, dirY));
            entity[i].clawEntity.Move(clawDirY);

            if (entity[i].inOrientation)
            {
                entity[i].SetOrientation(new Vector2(dirX, dirY));
            }

            if (!pushInContinu)
            {
                if (_rewiredPlayers[i].GetButtonDown("Activate"))
                {
                    entity[i].Activate();
                }
            }
            else
            {
                if (_rewiredPlayers[i].GetButtonDown("Activate"))
                {
                    entity[i].Activate();
                }
                else if (_rewiredPlayers[i].GetButtonUp("Activate"))
                {
                    entity[i].Activate(false);
                }
            }



            if (_rewiredPlayers[i].GetButtonDown("Dash"))
            {
                AudioManager.instance.Play("Son_Dash");
                entity[i].Dash();
            }
            if (_rewiredPlayers[i].GetButtonDown("Attaque"))
            {
                entity[i].clawEntity.Attaque();
            }
            if (_rewiredPlayers[i].GetButtonDown("Parry"))
            {
                entity[i].Parry(true);
            }
            else if(_rewiredPlayers[i].GetButtonUp("Parry"))
            {
                entity[i].Parry(false);
            }
        }
    }
}
