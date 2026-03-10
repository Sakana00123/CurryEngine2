using Editor.GameProject;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Input;
using Editor.Utilities;
using Editor.DllWrappers;

namespace Editor.Components
{
    /// <summary>
    /// ゲームオブジェクトを表します。
    /// </summary>
    /// <remarks>
    /// <see cref="GameObject"/> クラスは、シーン内の個々のオブジェクトを表現します。各ゲームオブジェクトは複数のコンポーネントを持つことができ、これによりその振る舞いや特性が定義されます。
    /// </remarks>
    [DataContract]
    [KnownType(typeof(Transform))]
    class GameObject : ViewModelBase
    {
        private int _entityId = ID.INVALID_ID;
        public int EntityId
        {
            get => _entityId; 
            set
            {
                if (_entityId != value)
                {
                    _entityId = value;
                    OnPropertyChanged(nameof(EntityId));
                }
            }
        }
        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    if (_isActive)
                    {
                        EntityId = EngineAPI.CreateGameObject(this);
                        Debug.Assert(ID.IsValid(EntityId));
                    }
                    else
                    {
                        EngineAPI.RemoveGameObject(this);
                        EntityId = ID.INVALID_ID;
                    }
                    OnPropertyChanged(nameof(IsActive));
                }
            }
        }

        private bool _isEnabled = true;
        [DataMember]
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged(nameof(IsEnabled));
                }
            }
        }

        private string _name;
        [DataMember]
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        [DataMember]
        public Scene ParentScene { get; private set; }

        [DataMember(Name = nameof(Components))]
        private readonly ObservableCollection<Component> _components = new ObservableCollection<Component>();
        public ReadOnlyObservableCollection<Component> Components { get; private set; }

        public Component? GetComponent(Type type) => Components.FirstOrDefault(c => c.GetType() == type);
        public T? GetComponent<T>() where T : Component => GetComponent(typeof(T)) as T;

        [OnDeserialized]
        void OnDeserialized(StreamingContext context)
        {
            if (_components != null)
            {
                Components = new ReadOnlyObservableCollection<Component>(_components);
                OnPropertyChanged(nameof(Components));
            }
        }

        public GameObject(Scene scene)
        {
            // nullチェック
            Debug.Assert(scene != null);
            // コンポーネントの読み取り専用コレクションを初期化
            ParentScene = scene;
            // 必須コンポーネントの追加
            _components.Add(new Transform(this));
            OnDeserialized(new StreamingContext());
        }
    }

    abstract class MSObject : ViewModelBase
    {
        // _enableUpdates フラグは、プロパティの更新中に変更通知を一時的に無効にするために使用されます。
        private bool _enableUpdates = true;
        private bool? _isEnabled;
        public bool? IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged(nameof(IsEnabled));
                }
            }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        private readonly ObservableCollection<IMSComponent> _components = new ObservableCollection<IMSComponent>();
        public ReadOnlyObservableCollection<IMSComponent> Components { get; }

        public List<GameObject> SelectedGameObjects { get; }

        private void MakeComponentList()
        {
            _components.Clear();
            var firstGameObject = SelectedGameObjects.First();
            if (firstGameObject == null) return;

            foreach (var component in firstGameObject.Components)
            {
                var type = component.GetType();
                if (!SelectedGameObjects.Skip(1).Any(gameObject => gameObject.GetComponent(type) == null))
                {
                    Debug.Assert(Components.FirstOrDefault(x => x.GetType() == type) == null);
                    _components.Add(component.GetMultiSelectionComponent(this));
                }
            }
        }

        public static float? GetMixedValue<T>(List<T> objects, Func<T, float> getProperty)
        {
            var value = getProperty(objects.First());
            return objects.Skip(1).Any(x => !getProperty(x).IsTheSameAs(value)) ? null : value;
        }
        public static bool? GetMixedValue<T>(List<T> objects, Func<T, bool> getProperty)
        {
            var value = getProperty(objects.First());
            return objects.Skip(1).Any(x => getProperty(x) != value) ? null : value;
        }
        public static string? GetMixedValue<T>(List<T> objects, Func<T, string> getProperty)
        {
            var value = getProperty(objects.First());
            return objects.Skip(1).Any(x => getProperty(x) != value) ? null : value;
        }
        protected virtual bool UpdateGameObjects(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(IsEnabled): SelectedGameObjects.ForEach(x => x.IsEnabled = IsEnabled!.Value); return true;
                case nameof(Name): SelectedGameObjects.ForEach(x => x.Name = Name); return true;
            }
            return false;
        }

        protected virtual bool UpdateMSGameObject()
        {
            IsEnabled = GetMixedValue(SelectedGameObjects, new Func<GameObject, bool>(x => x.IsEnabled));
            Name = GetMixedValue(SelectedGameObjects, new Func<GameObject, string>(x => x.Name));

            return true;
        }

        public void Refresh()
        {
            _enableUpdates = false;
            UpdateMSGameObject();
            MakeComponentList();
            _enableUpdates = true;
        }

        public MSObject(List<GameObject> gameObjects)
        {
            Debug.Assert(gameObjects != null && gameObjects.Count > 0);
            Components = new ReadOnlyObservableCollection<IMSComponent>(_components);
            SelectedGameObjects = gameObjects;
            PropertyChanged += (s, e) => { if (_enableUpdates) UpdateGameObjects(e.PropertyName!); };
        }

    }

    class MSGameObject : MSObject
    {
        public MSGameObject(List<GameObject> gameObjects) : base(gameObjects)
        {
            Refresh();
        }
    }
}
