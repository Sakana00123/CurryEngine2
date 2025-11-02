using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Components
{
    /// <summary>
    /// ゲームオブジェクトの位置、回転、スケールを表すコンポーネントです。
    /// </summary>
    /// <remarks> Transform コンポーネントは、ゲームオブジェクトの空間内での位置や向きを管理します。
    /// <see cref="Position"/>、<see cref="Rotation"/>、および <see cref="Scale"/> プロパティを通じてアクセスできます。
    /// </remarks>
    [DataContract]
    class Transform : Component
    {
        private Vector3 _position;
        [DataMember]
        public Vector3 Position
        {
            get => _position;
            set
            {
                if (_position != value)
                {
                    _position = value;
                    OnPropertyChanged(nameof(Position));
                }
            }
        }

        private Vector3 _rotation;
        [DataMember]
        public Vector3 Rotation
        {
            get => _rotation;
            set
            {
                if (_rotation != value)
                {
                    _rotation = value;
                    OnPropertyChanged(nameof(Rotation));
                }
            }
        }

        private Vector3 _scale;
        [DataMember]
        public Vector3 Scale
        {
            get => _scale;
            set
            {
                if (_scale != value)
                {
                    _scale = value;
                    OnPropertyChanged(nameof(Scale));
                }
            }
        }
        public Transform(GameObject owner) : base(owner)
        {
        }
    }
}
