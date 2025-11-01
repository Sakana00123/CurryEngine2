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
    public class GameObject : ViewModelBase
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

        private string? _name;
        [DataMember]
        public string? Name
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
        public Scene? ParentScene { get; private set; }

        [DataMember(Name = nameof(Components))]
        private readonly ObservableCollection<Component> _components = new ObservableCollection<Component>();
        public ReadOnlyObservableCollection<Component> Components { get; private set; }

        public ICommand RenameCommand { get; private set; }
        public ICommand IsActiveCommand { get; private set; }

        [OnDeserialized]
        void OnDeserialized(StreamingContext context)
        {
            if (_components != null)
            {
                Components = new ReadOnlyObservableCollection<Component>(_components);
                OnPropertyChanged(nameof(Components));
            }

            RenameCommand = new RelayCommand<string>(x =>
            {
                var oldName = Name;
                Name = x;

                Project.UndoRedo.Add(new UndoRedoAction(nameof(Name), this, 
                    oldName!, x, $"Rename GameObject '{oldName}' to '{x}'"));
            }, x => x != _name);

            IsActiveCommand = new RelayCommand<bool>(x =>
            {
                var oldValue = IsActive;
                IsActive = x;

                Project.UndoRedo.Add(new UndoRedoAction(nameof(IsActive), this,
                    oldValue!, x, x ? $"Active {Name}" : $"Deactive {Name}"));
            });
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
}
