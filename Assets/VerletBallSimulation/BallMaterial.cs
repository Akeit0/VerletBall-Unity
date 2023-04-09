using Unity.Mathematics;
using UnityEngine;

namespace VerletBallSimulation {
    public class BallMaterial {
        static readonly int ScaleID = Shader.PropertyToID("_Scale");
        static readonly int BallScaleID = Shader.PropertyToID("_BallScale");
        static readonly int PosID = Shader.PropertyToID("_Pos");
        public Material BaseMaterial;
        public Material BlurMaterial;
        float2 _position;
        float _scale;
        float _ballScale;
        bool _blur;

        public bool Blur {
            get => _blur;
            set {
                if (_blur != value) {
                    _blur = value;
                    Current.SetVector(PosID, new Vector4(_position.x, _position.y, 0, 1));
                    Current.SetFloat(ScaleID, _scale);
                    Current.SetFloat(BallScaleID, _ballScale);
                }
            }
        }
        public Material Current => _blur ? BlurMaterial : BaseMaterial;
        
        public BallMaterial(Material material,Material blurMaterial) {
            BaseMaterial = material;
            BlurMaterial = blurMaterial;
            var v4=BaseMaterial.GetVector(PosID);
            _position = new float2(v4.x, v4.y);
            _scale = BaseMaterial.GetFloat(ScaleID);
            _ballScale = BaseMaterial.GetFloat(BallScaleID);
        }
        public float2 Position {
            get => _position;
            set {
                if(!_position.Equals(value)) {
                    _position = value;
                    Current.SetVector(PosID, new Vector4(value.x, value.y, 0, 1));
                }
            }
        }
        public float Scale {
            get => _scale;
            set {
                if(_scale!=value) {
                    _scale = value;
                    Current.SetFloat(ScaleID, value);
                }
            }
        }
        public float BallScale {
            get => _ballScale;
            set {
                if(_ballScale!=value) {
                    _ballScale = value;
                    Current.SetFloat(BallScaleID, value);
                }
            }
        }
    }
}