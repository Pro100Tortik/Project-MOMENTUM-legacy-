using System;

public class Timer
{
    public event Action OnTimerEnd;

    public float RemainigSeconds { get; private set; }

    public Timer(float duration)
    {
        RemainigSeconds = duration;
    }

    public void Tick(float deltaTime)
    {
        if (RemainigSeconds <= 0f)
            return;

        RemainigSeconds -= deltaTime;

        CheckForTimerEnd();
    }

    private void CheckForTimerEnd()
    {
        if (RemainigSeconds > 0f)
            return;

        RemainigSeconds = 0f;

        OnTimerEnd?.Invoke();
    }
}
