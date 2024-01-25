using UnityFramework.Scenes;
using Zenject;

namespace UnityFramework.Samples
{
    public class SampleSceneInitializer : SceneInitializer
    {
        public SampleSceneInitializer([Inject] DiContainer container) 
            : base(container)
        {
        }
    }
}
