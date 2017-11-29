using UnityEngine;

namespace Assets.Scripts
{
    public class ObjectParameters
    {
        public Vector3 position;
        public Vector3 scale;
        public Quaternion rotation;
        public Color color;
        public string model;
        public string imageName;

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}, {3}, {4}, {5}",
                imageName, model, position.ToString(),
                rotation.ToString(), scale.ToString(), color.ToString()
                );
        }
    }
}
