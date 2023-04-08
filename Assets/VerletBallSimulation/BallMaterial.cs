using Unity.Mathematics;
using UnityEngine;

namespace VerletBallSimulation {
    public class BallMaterial {
        static readonly int ScaleID = Shader.PropertyToID("_Scale");
        static readonly int BallScaleID = Shader.PropertyToID("_BallScale");
        static readonly int PosID = Shader.PropertyToID("_Pos");
        public Material Material;
        float2 _position;
        float _scale;
        float _ballScale;
        
        public BallMaterial(Material material) {
            Material = material;
            var v4=Material.GetVector(PosID);
            _position = new float2(v4.x, v4.y);
            _scale = Material.GetFloat(ScaleID);
            _ballScale = Material.GetFloat(BallScaleID);
        }
        public float2 Position {
            get => _position;
            set {
                if(!_position.Equals(value)) {
                    _position = value;
                    Material.SetVector(PosID, new Vector4(value.x, value.y, 0, 1));
                }
            }
        }
        public float Scale {
            get => _scale;
            set {
                if(_scale!=value) {
                    _scale = value;
                    Material.SetFloat(ScaleID, value);
                }
            }
        }
        public float BallScale {
            get => _ballScale;
            set {
                if(_ballScale!=value) {
                    _ballScale = value;
                    Material.SetFloat(BallScaleID, value);
                }
            }
        }
    }
}