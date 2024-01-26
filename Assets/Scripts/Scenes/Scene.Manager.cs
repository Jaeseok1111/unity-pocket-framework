using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SceneManager : MonoBehaviour
{
    private UIPanel _panel;
    private UILoadingBar _loadingWithBar;

    [Inject]
    public void Construct(
        [Inject] UIPanel panel,
        [Inject(Id = "LoadingWithBar", Optional = true)] UILoadingBar loadingWithBar)
    {
        _panel = panel;
        _loadingWithBar = loadingWithBar;
    }

    public void LoadSceneAsync(string sceneName, params SceneJob[] jobs)
    {
        UILoadingBar loading = _loadingWithBar != null
            ? _panel
                .NewPopup(_loadingWithBar.gameObject)
                .GetComponent<UILoadingBar>()
            : null;

        List<SceneJob> jobList = new List<SceneJob>();
        jobList.AddRange(jobs);
        jobList.Add(new LoadSceneJob(sceneName));

        StartCoroutine(ExecuteSceneLoadJobs(loading, jobList));
    }

    private IEnumerator ExecuteSceneLoadJobs(UILoadingBar loading, List<SceneJob> jobs)
    {
        float percent = 0;
        float totalPercent = jobs.Count * 100;

        int index = 0;
        bool rollback = false;

        while (true)
        {
            SceneJob job = jobs[index];

            job.Execute();

            if (job.IsRollback())
            {
                rollback = true;
                break;
            }

            if (job.IsDone())
            {
                percent += 100f;
                loading?.SetPercent((int)((percent / totalPercent) * 100));

                yield return new WaitForSeconds(0.5f);

                ++index;

                if (index == jobs.Count)
                {
                    break;
                }
            }
            else
            {
                loading?.SetPercent((int)((percent + job.GetPercent() / totalPercent) * 100));
            }

            yield return null;
        }

        if (rollback)
        {
            jobs.ForEach(x => x.Rollback());

            _panel.Back();
        }
        else
        {
            jobs.ForEach(x => x.Complete());
        }

        yield return null;
    }
}