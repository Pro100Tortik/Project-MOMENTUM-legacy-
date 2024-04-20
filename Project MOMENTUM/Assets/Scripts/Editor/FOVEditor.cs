using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyFOV))]
public class FOVEditor : Editor
{
    private void OnSceneGUI()
    {
        EnemyFOV fov = (EnemyFOV)target;

        // Vector3 pos = fov.transform.position + Vector3.up * fov.eyesHeight;
        Vector3 pos = fov.transform.position + Vector3.up * 0.2f;

        Handles.color = Color.white;

        Handles.DrawWireArc(pos, Vector3.up, Vector3.forward, 360, fov.viewRadius);

        Vector3 viewAngleA = fov.DirFromAngle(-fov.viewAngle / 2, false);
        Vector3 viewAngleB = fov.DirFromAngle(fov.viewAngle / 2, false);

        Handles.DrawLine(pos, pos + viewAngleA * fov.viewRadius);
        Handles.DrawLine(pos, pos + viewAngleB * fov.viewRadius);

        Handles.color = Color.red;

        foreach (Transform visibleTarget in fov.visibleTargets)
        {
            Handles.DrawLine(pos, visibleTarget.position);
        }
    }
}
