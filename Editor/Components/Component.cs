using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Components
{
    interface IMSComponent { }
    /// <summary>
    /// ゲームオブジェクトに追加される基本的なコンポーネントを表します。
    /// </summary>
    /// <remarks> Component クラスは、ゲームオブジェクトに機能やデータを追加するための基底クラスです。
    /// <see cref="Transform"/> コンポーネントなど、他の具体的なコンポーネントはこのクラスを継承して実装されます。
    /// </remarks>
    [DataContract]
    abstract class Component : ViewModelBase
    {
        public abstract IMSComponent GetMultiSelectionComponent(MSObject msObject);

        [DataMember]
        public GameObject Owner { get; private set; }

        public Component(GameObject owner)
        {
            Debug.Assert(owner != null);
            Owner = owner;
        }
    }

    abstract class MSComponent<T> : ViewModelBase, IMSComponent where T : Component
    {
        private bool _enableUpdates = true;
        public List<T> SelectedComponents { get; }

        // UpdateComponents メソッドは、プロパティの変更に応じて選択されたコンポーネントの状態を更新するための抽象メソッドです。
        protected abstract bool UpdateComponents(string propertyName);
        // UpdateMSComponent メソッドは、MSComponent 自身の状態を更新するための抽象メソッドです。これにより、プロパティの変更に応じて MSComponent の状態も適切に更新されることが保証されます。
        protected abstract bool UpdateMSComponent();

        public void Refresh()
        {
            _enableUpdates = false;
            UpdateMSComponent();
            _enableUpdates = true;
        }

        public MSComponent(MSObject msObject)
        {
            Debug.Assert(msObject?.SelectedGameObjects?.Count != 0);
            SelectedComponents = msObject?.SelectedGameObjects?.Select(x => x.GetComponent<T>()).ToList()!;
            PropertyChanged += (s, e) =>
            {
                if (_enableUpdates) _ = UpdateComponents(e.PropertyName!);
            };
        }
    }


}
