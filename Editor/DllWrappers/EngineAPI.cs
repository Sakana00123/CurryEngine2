using Editor.Components;
using Editor.EngineAPIStructs;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace Editor.EngineAPIStructs
{
    [StructLayout(LayoutKind.Sequential)]
    class TransformComponent
    {
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale = new Vector3(1, 1, 1);
    }

    [StructLayout(LayoutKind.Sequential)]
    class GameObjectDescriptor
    {
        public TransformComponent Transform = new TransformComponent();
    }
}


namespace Editor.DllWrappers
{
    static class EngineAPI
    {
        private const string _dllName = "EngineDLL.dll";

        [DllImport(_dllName)]
        private static extern int CreateGameObject(GameObjectDescriptor desc);
        public static int CreateGameObject(GameObject @object)
        {
            GameObjectDescriptor desc = new GameObjectDescriptor();

            // Transform Component
            {
                var c = @object.GetComponent<Transform>();
                desc.Transform.Position = c.Position;
                desc.Transform.Rotation = c.Rotation;
                desc.Transform.Scale = c.Scale;
            }

            return CreateGameObject(desc);
        }

        [DllImport(_dllName)]
        private static extern void RemoveGameObject(int id);
        public static void RemoveGameObject(GameObject @object)
        {
            RemoveGameObject(@object.EntityId);
        }
    }
}
