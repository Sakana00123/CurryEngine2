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

        [DataMember(Name = nameof(GameEntities))]
        private readonly ObservableCollection<GameEntity> _gameEntities = new ObservableCollection<GameEntity>();
        public ReadOnlyObservableCollection<GameEntity> GameEntities { get; private set; }

        public ICommand AddGameEntityCommand { get; private set; }
        public ICommand RemoveGameEntityCommand { get; private set; }

        /// <summary>
        /// ゲームエンティティをシーンに追加します。
        /// </summary>
        /// <remarks> このメソッドは UndoRedo 用に内部的に使用されます。通常は <see cref="AddGameEntityCommand"/> を使用してください。</remarks>
        /// <param name="entity"> 追加するゲームエンティティ。<see langword="null"/> ではなく、コレクションに存在しないこと。</param>
        private void AddGameEntity(GameEntity entity, int index = -1)
        {
            // ゲームエンティティが存在することを確認
            Debug.Assert(!_gameEntities.Contains(entity));
            entity.IsActive = IsActive;
            if (index == -1)
            {
                _gameEntities.Add(entity);
                
            }
            else
            {
                _gameEntities.Insert(index, entity);
                
            }
        }
        /// <summary>
        /// シーンからゲームエンティティを削除します。
        /// </summary>
        /// <remarks> このメソッドは UndoRedo 用に内部的に使用されます。通常は <see cref="RemoveGameEntityCommand"/> を使用してください。</remarks>
        /// <param name="gameEntity"> 削除するゲームエンティティ。<see langword="null"/> ではなく、コレクションに存在すること。</param>
        private void RemoveGameEntity(GameEntity gameEntity)
        {
            Debug.Assert(_gameEntities.Contains(gameEntity));
            gameEntity.IsActive = false;
            _gameEntities.Remove(gameEntity);
        }

        /// <summary>
        /// デシリアライズ後の初期化処理を行います。
        /// </summary>
        /// <remarks> このメソッドはデシリアライズ後に自動的に呼び出され、コマンドの初期化や読み取り専用コレクションの設定を行います。</remarks>
        /// <param name="context"> ストリーミングコンテキスト。</param>
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (_gameEntities != null)
            {
                GameEntities = new ReadOnlyObservableCollection<GameEntity>(_gameEntities);
                OnPropertyChanged(nameof(GameEntities));
            }
            foreach (var entity in _gameEntities)
            {
                entity.IsActive = IsActive;
            }

            // Commands
            AddGameEntityCommand = new RelayCommand<GameEntity>(x =>
            {
                AddGameEntity(x);
                var entityIndex = _gameEntities!.Count - 1;

                Project.UndoRedo.Add(new UndoRedoAction(
                    () => RemoveGameEntity(x),
                    () => AddGameEntity(x, entityIndex),
                    $"Add {x.Name} to {Name}"));
            });

            RemoveGameEntityCommand = new RelayCommand<GameEntity>(x =>
            {
                var entityIndex = _gameEntities!.IndexOf(x);
                RemoveGameEntity(x);

                Project.UndoRedo.Add(new UndoRedoAction(
                    () => AddGameEntity(x, entityIndex),
                    () => RemoveGameEntity(x),
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
