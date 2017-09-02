using UnityEditor;
using UnityEngine;
using System.Text.RegularExpressions;
using RPG.Characters;

/// <summary>
/// Goes through all children of a selected object and adds a dominant hand to the left or right hand – if it's there.
/// </summary>

public class AddDominantHand : Editor 
{
    static Regex leftHandRegex = new Regex (@"(hand[.:_]l(eft)?$|l(eft)?[.:_]hand$|L(eft)?hand$)", RegexOptions.IgnoreCase); // Left.Hand, Hand_L, Hand:Left etc.
    static Regex rightHandRegex = new Regex (@"(hand[.:_]r(ight)?$|r(ight)?[.:_]hand$|R(ight)?hand$)", RegexOptions.IgnoreCase); // Right.Hand, Hand_R, Hand:Left etc.

    [MenuItem ("GameObject/RPG/Dominant Hand/Set Left Hand", false, 0)]
    static void SetLeftHandDominant (MenuCommand menuCommand)
    {
        ClearDominantHand (menuCommand); // Keep only new dominant hand

        GameObject selected = menuCommand.context as GameObject;
        if (selected == null) return; // Nothing is selected

        GameObject leftHand = FindHandGameObject (selected, leftHandRegex);
        if (!leftHand)
        {
            Debug.LogWarning ("No hand found on selected object <color=orange>" + selected.name + "</color> or it doesn't use any common name pattern, e.g.:\n" +
                                        "\tHand_L, Left.Hand, Hand:Left, mixamorig_LeftHand\n", selected);
        }
        else
        {
            leftHand.AddComponent<DominantHand> ();
            EditorGUIUtility.PingObject (leftHand);
        }

        GameObject rightHand = FindHandGameObject (selected, rightHandRegex);
        if (!rightHand)
        {
            Debug.LogWarning ("No hand found on selected object <color=orange>" + selected.name + "</color> or it doesn't use any common name pattern, e.g.:\n" +
                             "\tHand_R, Right.Hand, Hand:Right, mixamorig_RightHand\n", selected);
        }
        else
        {
            rightHand.AddComponent<OtherHand> ();
            EditorGUIUtility.PingObject (rightHand);
        }
    }

    [MenuItem ("GameObject/RPG/Dominant Hand/Set Right Hand", false, 0)]
    static void SetRightHandDominant (MenuCommand menuCommand)
    {
        ClearDominantHand (menuCommand); // Keep only new dominant hand

        GameObject selected = menuCommand.context as GameObject;
        if (selected == null) return; // Nothing is selected

        GameObject rightHand = FindHandGameObject (selected, rightHandRegex);
        if (!rightHand)
        {
            Debug.LogWarning ("No hand found on selected object <color=orange>" + selected.name + "</color> or it doesn't use any common name pattern, e.g.:\n" +
                                         "\tHand_R, Right.Hand, Hand:Right, mixamorig_RightHand\n", selected);
        }
        else
        {
            rightHand.AddComponent<DominantHand> ();
            EditorGUIUtility.PingObject (rightHand);
        }

        GameObject leftHand = FindHandGameObject (selected, leftHandRegex);
        if (!leftHand)
        {
            Debug.LogWarning ("No hand found on selected object <color=orange>" + selected.name + "</color> or it doesn't use any common name pattern, e.g.:\n" +
                                        "\tHand_L, Left.Hand, Hand:Left, mixamorig_LeftHand\n", selected);
        }
        else
        {
            leftHand.AddComponent<OtherHand> ();
            EditorGUIUtility.PingObject (leftHand);
        }

    }

    [MenuItem ("GameObject/RPG/Dominant Hand/Clear Dominant Hand", false, 0)]
    static void ClearDominantHand (MenuCommand menuCommand)
    {
        GameObject selected = menuCommand.context as GameObject;
        if (selected == null) return; // Nothing is selected

        var dominantHands = selected.GetComponentsInChildren<DominantHand> ();
        if (dominantHands.Length > 0)
        {
            foreach (var handScript in dominantHands)
            {
                DestroyImmediate (handScript);
            }
        }

        var otherHands = selected.GetComponentsInChildren<OtherHand> ();
        if (otherHands.Length > 0)
        {
            foreach (var handScript in otherHands)
            {
                DestroyImmediate (handScript);
            }
        }
    }

    private static GameObject FindHandGameObject (GameObject searchRoot, Regex handRegex)
    {
        // Tries to find a GameObject named like a hand
        var allChildren = searchRoot.GetComponentsInChildren<Transform> (true);
        foreach (Transform child in allChildren)
        {
            if (handRegex.IsMatch (child.name))
            {
                return child.gameObject;
            }
        }
        return null;
    }
}
