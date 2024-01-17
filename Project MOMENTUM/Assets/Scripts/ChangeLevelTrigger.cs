using UnityEngine;

public class ChangeLevelTrigger : MonoBehaviour
{
    [SerializeField] private bool index = false;
    [SerializeField] private int levelIndex;
    [SerializeField] private string level;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (index)
            {
                LevelManager.ChangeLevelWithIndex(levelIndex);
            }
            else
            {
                LevelManager.ChangeLevel(level);
            }
        }
    }
}
