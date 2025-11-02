using Editor.Components;
using Editor.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Editor.GameProject
{
    /// <summary>
    /// シーンを表します。
    /// </summary>
    /// <remarks>
    /// <see cref="Scene"/> クラスは、ゲーム内の特定の環境やレベルを表現します。各シーンは複数のゲームオブジェクトを含むことができ、これによりシーン内の要素やインタラクションが定義されます。
    /// </remarks>
    [DataContract]
    class Scene : ViewModelBase
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
        public Project? Project { get; private set; }

        private bool _isActive;
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

        [DataMember(Name = nameof(GameObjects))]
        private readonly ObservableCollection<GameObject> _gameObjects = new ObservableCollection<GameObject>();
        public ReadOnlyObservableCollection<GameObject> GameObjects { get; private set; }

        public ICommand AddGameObjectCommand { get; private set; }
        public ICommand RemoveGameObjectCommand { get; private set; }

        /// <summary>
        /// ゲームオブジェクトをシーンに追加します。
        /// </summary>
        /// <remarks> このメソッドは UndoRedo 用に内部的に使用されます。通常は <see cref="AddGameObjectCommand"/> を使用してください。</remarks>
        /// <param name="gameObject"> 追加するゲームエンティティ。<see langword="null"/> ではなく、コレクションに存在しないこと。</param>
        private void AddGameObject(GameObject gameObject)
        {
            // nullチェック
            Debug.Assert(gameObject != null);
            // ゲームオブジェクトが存在することを確認
            Debug.Assert(!_gameObjects.Contains(gameObject));
            _gameObjects.Add(gameObject);
        }
        /// <summary>
        /// シーンからゲームオブジェクトを削除します。
        /// </summary>
        /// <remarks> このメソッドは UndoRedo 用に内部的に使用されます。通常は <see cref="RemoveGameObjectCommand"/> を使用してください。</remarks>
        /// <param name="gameObject"> 削除するゲームエンティティ。<see langword="null"/> ではなく、コレクションに存在すること。</param>
        private void RemoveGameObject(GameObject gameObject)
        {
            // nullチェック
            Debug.Assert(gameObject != null);
            // ゲームオブジェクトが存在することを確認
            Debug.Assert(_gameObjects.Contains(gameObject));
            _gameObjects.Remove(gameObject);
        }

        /// <summary>
        /// デシリアライズ後の初期化処理を行います。
        /// </summary>
        /// <remarks> このメソッドはデシリアライズ後に自動的に呼び出され、コマンドの初期化や読み取り専用コレクションの設定を行います。</remarks>
        /// <param name="context"> ストリーミングコンテキスト。</param>
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (_gameObjects != null)
            {
                GameObjects = new ReadOnlyObservableCollection<GameObject>(_gameObjects);
                OnPropertyChanged(nameof(GameObjects));
            }

            // Commands
            AddGameObjectCommand = new RelayCommand<GameObject>(x =>
            {
                AddGameObject(x);
                var entityIndex = _gameObjects!.Count - 1;

                Project.UndoRedo.Add(new UndoRedoAction(
                    () => RemoveGameObject(x),
                    () => _gameObjects.Insert(entityIndex, x),
                    $"Add {x.Name} to {Name}"));
            });

            RemoveGameObjectCommand = new RelayCommand<GameObject>(x =>
            {
                var entityIndex = _gameObjects!.IndexOf(x);
                RemoveGameObject(x);

                Project.UndoRedo.Add(new UndoRedoAction(
                    () => _gameObjects.Insert(entityIndex, x),
                    () => RemoveGameObject(x),
                    $"Remove {x.Name}"));
            });
        }

        public Scene(Project project, string name)
        {
            Debug.Assert(project != null);
            Project = project;
            Name = name;
            OnDeserialized(new StreamingContext());
        }
    }
}
