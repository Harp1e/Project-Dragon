using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.Core
{
    public class LevelExit : MonoBehaviour
    {
        [SerializeField] int nextLevel;

        void OnTriggerEnter (Collider other)
        {
            if (other.tag != "Player") { return; }
            ChangeLevel ();
        }

        void ChangeLevel ()
        {
            Debug.Log ("Player has reached exit point - moving to next level: " + nextLevel);
            StartCoroutine (DissolveToNewLevel ());
        }

        IEnumerator DissolveToNewLevel ()
        {
            yield return new WaitForSecondsRealtime (0.5f);
            SceneManager.LoadScene (nextLevel);

        }
    }
}