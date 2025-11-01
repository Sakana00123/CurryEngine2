using Editor.GameProject;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;

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
        }
    }
}
