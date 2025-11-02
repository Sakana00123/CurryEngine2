using Editor.GameProject;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Input;
using Editor.Utilities;

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
        private bool _isActive = true;
        [DataMember]
        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    OnPropertyChanged(nameof(IsActive));
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
        private bool? _isActive;
        public bool? IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    OnPropertyChanged(nameof(IsActive));
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

        public List<GameObject> SelectedObjects { get; }

        public static float? GetMixedValue(List<GameObject> objects, Func<GameObject, float> getProperty)
        {
            var value = getProperty(objects.First());
            foreach (var obj in objects.Skip(1))
            {
                if (!value.IsTheSameAs(getProperty(obj)))
                {
                    return null;
                }
            }
            return value;
        }
        public static bool? GetMixedValue(List<GameObject> objects, Func<GameObject, bool> getProperty)
        {
            var value = getProperty(objects.First());
            foreach (var obj in objects.Skip(1))
            {
                if (value != (getProperty(obj)))
                {
                    return null;
                }
            }
            return value;
        }
        public static string? GetMixedValue(List<GameObject> objects, Func<GameObject, string> getProperty)
        {
            var value = getProperty(objects.First());
            foreach (var obj in objects.Skip(1))
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
                case nameof(IsActive): SelectedObjects.ForEach(x => x.IsActive = IsActive!.Value); return true;
                case nameof(Name): SelectedObjects.ForEach(x => x.Name = Name); return true;
            }
            return false;
        }

        protected virtual bool UpdateMSGameObject()
        {
            IsActive = GetMixedValue(SelectedObjects, new Func<GameObject, bool>(x => x.IsActive));
            Name = GetMixedValue(SelectedObjects, new Func<GameObject, string>(x => x.Name));

            return true;
        }

        public void Refresh()
        {
            _enableUpdates = false;
            UpdateMSGameObject();
            _enableUpdates = true;
        }

        public MSObject(List<GameObject> objects)
        {
            Debug.Assert(objects != null && objects.Count > 0);
            Components = new ReadOnlyObservableCollection<IMSComponent>(_components);
            SelectedObjects = objects;
            PropertyChanged += (s, e) => { if (_enableUpdates) UpdateGameObjects(e.PropertyName!); };
        }

    }

    class MSGameObject : MSObject
    {
        public MSGameObject(List<GameObject> objects) : base(objects)
        {
            Refresh();
        }
    }
}
