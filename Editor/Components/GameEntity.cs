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
    /// <see cref="GameEntity"/> クラスは、シーン内の個々のオブジェクトを表現します。各ゲームオブジェクトは複数のコンポーネントを持つことができ、これによりその振る舞いや特性が定義されます。
    /// </remarks>
    [DataContract]
    [KnownType(typeof(Transform))]
    class GameEntity : ViewModelBase
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
                        EntityId = EngineAPI.CreateGameEntity(this);
                        Debug.Assert(ID.IsValid(EntityId));
                    }
                    else
                    {
                        EngineAPI.RemoveGameEntity(this);
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

        public GameEntity(Scene scene)
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

    abstract class MSEntity : ViewModelBase
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

        public List<GameEntity> SelectedEntities { get; }

        public static float? GetMixedValue(List<GameEntity> entities, Func<GameEntity, float> getProperty)
        {
            var value = getProperty(entities.First());
            foreach (var obj in entities.Skip(1))
            {
                if (!value.IsTheSameAs(getProperty(obj)))
                {
                    return null;
                }
            }
            return value;
        }
        public static bool? GetMixedValue(List<GameEntity> entities, Func<GameEntity, bool> getProperty)
        {
            var value = getProperty(entities.First());
            foreach (var obj in entities.Skip(1))
            {
                if (value != (getProperty(obj)))
                {
                    return null;
                }
            }
            return value;
        }
        public static string? GetMixedValue(List<GameEntity> entities, Func<GameEntity, string> getProperty)
        {
            var value = getProperty(entities.First());
            foreach (var obj in entities.Skip(1))
            {
                if (value != (getProperty(obj)))
                {
                    return null;
                }
            }
            return value;
        }
        protected virtual bool UpdateGameObjects(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(IsEnabled): SelectedEntities.ForEach(x => x.IsEnabled = IsEnabled!.Value); return true;
                case nameof(Name): SelectedEntities.ForEach(x => x.Name = Name); return true;
            }
            return false;
        }

        protected virtual bool UpdateMSGameObject()
        {
            IsEnabled = GetMixedValue(SelectedEntities, new Func<GameEntity, bool>(x => x.IsEnabled));
            Name = GetMixedValue(SelectedEntities, new Func<GameEntity, string>(x => x.Name));

            return true;
        }

        public void Refresh()
        {
            _enableUpdates = false;
            UpdateMSGameObject();
            _enableUpdates = true;
        }

        public MSEntity(List<GameEntity> entities)
        {
            Debug.Assert(entities != null && entities.Count > 0);
            Components = new ReadOnlyObservableCollection<IMSComponent>(_components);
            SelectedEntities = entities;
            PropertyChanged += (s, e) => { if (_enableUpdates) UpdateGameObjects(e.PropertyName!); };
        }

    }

    class MSGameEntity : MSEntity
    {
        public MSGameEntity(List<GameEntity> entities) : base(entities)
        {
            Refresh();
        }
    }
}
