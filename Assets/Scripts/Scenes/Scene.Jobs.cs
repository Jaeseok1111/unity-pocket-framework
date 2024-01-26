using UnityEngine;

public abstract class SceneJob
{
    protected bool _rollback = false;
    protected bool _done = false;

    public abstract void Execute();
    public abstract void Rollback();
    public abstract void Complete();
    public abstract float GetPercent();

    public bool IsDone()
    {
        return _done;
    }
    public bool IsRollback()
    {
        return _rollback;
    }
}

public class LoadSceneJob : SceneJob
{
    private readonly string _name;

    private AsyncOperation _operation;

    public LoadSceneJob(string name)
    {
        _name = name;
    }

    public override void Execute()
    {
        if (_operation == null)
        {
            _operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(_name);
            _operation.allowSceneActivation = false;
        }

        if (_operation.progress >= 0.9f)
        {
            _done = true;
        }
    }

    public override void Rollback()
    {
        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(_name);
        _operation = null;
    }

    public override void Complete()
    {
        _operation.allowSceneActivation = true;
    }

    public override float GetPercent()
    {
        float percent01 = (float)(System.Math.Truncate(_operation.progress * 10f) / 10f);

        return percent01 * 100f;
    }
}