using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;

namespace Craft.Simulation.Reborn.GuiTest
{
    public class SceneListViewModel : ViewModelBase
    {
        private Dictionary<string, Scene> _sceneDictionary;
        private Scene _activeScene;

        public ObservableCollection<Scene> Scenes { get; }

        public Scene ActiveScene
        {
            get { return _activeScene; }
            set
            {
                _activeScene = value;
                RaisePropertyChanged();
            }
        }

        public SceneListViewModel()
        {
            _sceneDictionary = new Dictionary<string, Scene>();
            Scenes = new ObservableCollection<Scene>();
        }

        public void AddScene(
            Scene scene)
        {
            if (string.IsNullOrEmpty(scene.Name) ||
                _sceneDictionary.ContainsKey(scene.Name))
            {
                throw new InvalidOperationException(
                    "A scene must have a unique name in order to be included in the collection");
            }

            _sceneDictionary[scene.Name] = scene;
            Scenes.Add(scene);
        }
    }
}
