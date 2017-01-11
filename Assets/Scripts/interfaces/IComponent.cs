using System.Collections.Generic;

namespace Cloth3D.Interfaces {
    public interface IComponent {
        string Name { get; set; }
        void Tick();
        bool Init();
    }

    internal class ComponentMgr : Singleton<ComponentMgr> {
        private readonly List<IComponent> _lstComponents = new List<IComponent>();

        public bool Init() {
            foreach (var component in _lstComponents) {
                if (!component.Init())
                    return false;
            }
            return true;
        }

        public void Tick() {
            foreach (var component in _lstComponents) {
                component.Tick();
            }
        }

        public IComponent FindComponent(string name) {
            var ret = _lstComponents.Find(x => {
                if (x.Name == name) {
                    return true;
                }
                return false;
            });
            return ret;
        }

        public bool AddComponent(IComponent component) {
            var foundComponent = FindComponent(component.Name);
            if (foundComponent == null) {
                _lstComponents.Add(component);
                return true;
            }
            return false;
        }
    }
}