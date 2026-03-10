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
    /// <see cref="Scene"/> クラスは、ゲーム内の特定の環境やレベルを表現します。各シーンは複数のゲームエンティティを含むことができ、これによりシーン内の要素やインタラクションが定義されます。
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
        /// <param name="object"> 追加するゲームオブジェクト。<see langword="null"/> ではなく、コレクションに存在しないこと。</param>
        private void AddGameObject(GameObject @object, int index = -1)
        {
            // ゲームオブジェクトが存在することを確認
            Debug.Assert(!_gameObjects.Contains(@object));
            @object.IsActive = IsActive;
            if (index == -1)
            {
                _gameObjects.Add(@object);
                
            }
            else
            {
                _gameObjects.Insert(index, @object);
                
            }
        }
        /// <summary>
        /// シーンからゲームオブジェクトを削除します。
        /// </summary>
        /// <remarks> このメソッドは UndoRedo 用に内部的に使用されます。通常は <see cref="RemoveGameObjectCommand"/> を使用してください。</remarks>
        /// <param name="gameObject"> 削除するゲームオブジェクト。<see langword="null"/> ではなく、コレクションに存在すること。</param>
        private void RemoveGameObject(GameObject gameObject)
        {
            Debug.Assert(_gameObjects.Contains(gameObject));
            gameObject.IsActive = false;
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
            foreach (var gameObject in _gameObjects)
            {
                gameObject.IsActive = IsActive;
            }

            // Commands
            AddGameObjectCommand = new RelayCommand<GameObject>(x =>
            {
                AddGameObject(x);
                var index = _gameObjects!.Count - 1;

                Project.UndoRedo.Add(new UndoRedoAction(
                    () => RemoveGameObject(x),
                    () => AddGameObject(x, index),
                    $"Add {x.Name} to {Name}"));
            });

            RemoveGameObjectCommand = new RelayCommand<GameObject>(x =>
            {
                var index = _gameObjects!.IndexOf(x);
                RemoveGameObject(x);

                Project.UndoRedo.Add(new UndoRedoAction(
                    () => AddGameObject(x, index),
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
