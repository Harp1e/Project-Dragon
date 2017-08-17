using UnityEditor;
using RPG.Characters;

[CustomEditor (typeof(WeaponPickupPoint))]
public class WeaponPickupPointEditor : Editor 
{
    WeaponPickupPoint myself;

    void OnEnable ()
    {
        myself = target as WeaponPickupPoint;
        if (myself == null || serializedObject == null)
        {
            DestroyImmediate (this);
            return;
        }
    }

    public override void OnInspectorGUI ()
    {
        EditorGUI.BeginChangeCheck ();
        DrawDefaultInspector ();
        if (EditorGUI.EndChangeCheck())
        {
            myself.RefreshPrefab ();
        }
    }
}
