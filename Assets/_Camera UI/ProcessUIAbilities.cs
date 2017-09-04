using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Characters;

namespace RPG.CameraUI
{
    public class ProcessUIAbilities : MonoBehaviour
    {
        PlayerControl player;

        void Start ()
        {
            player = FindObjectOfType<PlayerControl> ();
        }

        public void TriggerAbility (int ability)
        {
            player.TriggerSpecialAbility (ability);
        }
    }
}