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
        [DataMember]
        public GameObject Owner { get; private set; }

        public Component(GameObject owner)
        {
            Debug.Assert(owner != null);
            Owner = owner;
        }
    }

    abstract class MSComponent<T> : ViewModelBase, IMSComponent where T : Component
    { }


}
